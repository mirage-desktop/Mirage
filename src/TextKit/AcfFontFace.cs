/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System.IO;
using System;
using System.Text;

namespace Mirage.TextKit
{
    /// <summary>
    /// An ACF (Advanced Cosmos Font Format) font face.
    /// </summary>
    public class AcfFontFace : FontFace
    {
        /// <summary>
        /// Initialise a new ACF (Advanced Cosmos Font Format) font face.
        /// </summary>
        /// <param name="stream">The data of the .acf font file.</param>
        public AcfFontFace(Stream stream)
        {
            _stream = stream;

            ParseMagic();
            ParseVersion();
            ParsePixelFormat();
            ParseHeight();
            ParseKerning();
            ParseMetadata();
            ParseGlyphs();
            ParseEndingMagic();
        }

        /// <summary>
        /// Check if a set of magic bytes are valid for the ACF format.
        /// </summary>
        /// <param name="magic">The magic bytes.</param>
        /// <returns>If the magic bytes are valid.</returns>
        private static bool AreMagicBytesValid(byte[] magic)
        {
            return magic[0] == 0x41 && magic[1] == 0x43 && magic[2] == 0x46;
        }

        /// <summary>
        /// Check if a set of ending magic bytes are valid for the ACF format.
        /// </summary>
        /// <param name="magic">The ending magic bytes.</param>
        /// <returns>If the ending magic bytes are valid.</returns>
        private static bool AreEndingMagicBytesValid(byte[] magic)
        {
            return magic[0] == 0x46 && magic[1] == 0x43 && magic[2] == 0x41;
        }

        /// <summary>
        /// Parse the magic bytes at the beginning of the ACF font face's stream.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown when the magic bytes are not valid.</exception>
        private void ParseMagic()
        {
            byte[] magicBuf = new byte[3];
            _stream.Read(magicBuf, 0, 3);
            if (!AreMagicBytesValid(magicBuf))
                throw new InvalidDataException("Invalid magic bytes.");
        }

        /// <summary>
        /// Parse the magic bytes at the end of the ACF font face's stream.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown when the ending magic bytes are not valid.</exception>
        private void ParseEndingMagic()
        {
            byte[] endingMagicBuf = new byte[3];
            _stream.Read(endingMagicBuf, 0, 3);
            if (!AreEndingMagicBytesValid(endingMagicBuf))
                throw new InvalidDataException("Invalid ending magic bytes.");
        }

        /// <summary>
        /// Parse the format version of the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the version is not supported by the parser.</exception>
        private void ParseVersion()
        {
            _version = (byte)_stream.ReadByte();
            if (_version != 0)
                throw new NotSupportedException("Unuspported ACF version.");
        }

        /// <summary>
        /// Parse the pixel format of the ACF font face.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the pixel format is not supported by the parser.</exception>
        private void ParsePixelFormat()
        {
            _pixelFormat = (byte)_stream.ReadByte();
            if (_pixelFormat != 0)
                throw new NotSupportedException("Unsupported pixel format.");
        }

        /// <summary>
        /// Parse the line height of the ACF font face.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown when the line height is zero.</exception>
        private void ParseHeight()
        {
            _height = (byte)_stream.ReadByte();
            if (_height == 0)
                throw new InvalidDataException("Invalid font height.");
        }

        /// <summary>
        /// Parse the kerning information of the ACF font face.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when kerning information is in the stream,
        /// as the parser does not currently support this feature.</exception>
        private void ParseKerning()
        {
            byte a = (byte)_stream.ReadByte();
            byte b = (byte)_stream.ReadByte();
            ushort kerningPairCount = (ushort)(a | (b << 8));
            if (kerningPairCount > 0)
                throw new NotSupportedException("ACF kerning is not supported.");
        }

        /// <summary>
        /// Parse a Pascal (length-prefixed) string from the data stream.
        /// </summary>
        /// <returns>The parsed string.</returns>
        private string ParsePascalString()
        {
            byte length = (byte)_stream.ReadByte();
            byte[] buffer = new byte[length];
            _stream.Read(buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Parse the ACF metadata (family and style names) from the stream.
        /// </summary>
        private void ParseMetadata()
        {
            _familyName = ParsePascalString();
            _styleName = ParsePascalString();
        }

        /// <summary>
        /// Parses the glyphs in the ACF font face from the stream.
        /// </summary>
        private void ParseGlyphs()
        {
            for (int i = 0; i < 256; i++)
                _glyphs[i] = ParseGlyph();
        }

        /// <summary>
        /// Parses a single glyph in the ACF font face from the stream.
        /// </summary>
        private Glyph ParseGlyph()
        {
            byte width = (byte)_stream.ReadByte();
            byte height = (byte)_stream.ReadByte();
            sbyte left = unchecked((sbyte)_stream.ReadByte());
            sbyte top = unchecked((sbyte)_stream.ReadByte());
            byte advanceX = (byte)_stream.ReadByte();
            byte[] bitmap = new byte[width * height];

            if (bitmap.Length > 0)
            {
                _stream.Read(bitmap, 0, width * height);
            }

            Glyph glyph = new Glyph(left, top, advanceX, width, height, bitmap);
            return glyph;
        }

        public override string GetFamilyName() => _familyName;

        public override string GetStyleName() => _styleName;

        public override int GetHeight() => _height;

        public override Glyph? GetGlyph(char c)
        {
            if (c < 0 || c > 255)
            {
                return null;
            }

            return _glyphs[c];
        }

        /// <summary>
        /// The stream to read the ACF font face's data from.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// The format version of the ACF file.
        /// </summary>
        private byte _version;

        /// <summary>
        /// The pixel format ID of the ACF file.
        /// </summary>
        private byte _pixelFormat;

        /// <summary>
        /// The line height of the font face.
        /// </summary>
        private byte _height;

        /// <summary>
        /// The name of the font family.
        /// </summary>
        private string _familyName = string.Empty;

        /// <summary>
        /// The name of the font style.
        /// </summary>
        private string _styleName = string.Empty;

        /// <summary>
        /// The glyphs of the font face in ASCII.
        /// </summary>
        private readonly Glyph[] _glyphs = new Glyph[256];
    }
}
