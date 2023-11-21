# This file is part of the Mirage Desktop Environment.
# github.com/mirage-desktop/Mirage
import sys
try:
    import freetype
except ImportError:
    print('error: freetype-py is missing\nhint: pip install -r requirements.txt', file=sys.stderr)
    sys.exit(1)
import argparse
import pathlib
import struct

def _build_acf(filename: str, face: freetype.Face) -> None:
    f = open(filename, 'wb')

    height: int = face.size.height >> 6 # 64th to px

    f.write(
        struct.pack(f'<3sBBBH{len(face.family_name) + 1}p{len(face.style_name) + 1}p',
                    bytes([0x41, 0x43, 0x46]), # magic
                    0x0, # version - byte
                    0x0, # pixel format - byte (0x0 = alpha (1 pixel per byte), 0x1 = mono (8 pixels per byte))
                    height, # font height in pixels - byte
                    0x0, # amount of kerning pairs - ushort
                    face.family_name, # font family name - pascal byte[]
                    face.style_name, # font style name - pascal byte[]
                    ))

    for n in range(0, 256):
        c = chr(n)

        face.load_char(c)
        glyph: freetype.GlyphSlot = face.glyph

        bitmap: freetype.Bitmap = glyph.bitmap

        f.write(struct.pack('BBbbB',
                            0 if bitmap is None else bitmap.width, # bitmap width - byte
                            0 if bitmap is None else bitmap.rows, # bitmap height - byte
                            glyph.bitmap_left, # bitmap left - sbyte
                            glyph.bitmap_top, # bitmap top - sbyte
                            glyph.advance.x >> 6)) # advance x - byte
        
        if bitmap is not None:
            f.write(bytes(bitmap.buffer)) # bitmap data - byte[]

    f.write(bytes([0x46, 0x43, 0x41])) # ending magic (the starting magic in reverse)

    f.close()

def _get_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
                        prog='acfconv',
                        description='generate .acf font files from .ttf or .otf files')

    parser.add_argument('filename', help='input filename')
    parser.add_argument('-o', '--output', help='output filename', action='store')
    parser.add_argument('-s', '--size', help='font size in pixels (default = 16)', action='store', type=int, default=16)

    return parser

def _main() -> None:
    parser = _get_parser()

    args = parser.parse_args()

    if args.output is None:
        args.output = pathlib.Path(args.filename).stem + '.acf'

    assert args.size > 0

    face = freetype.Face(args.filename)
    face.set_pixel_sizes(args.size, args.size)

    _build_acf(args.output, face)

if __name__ == '__main__':
    _main()
