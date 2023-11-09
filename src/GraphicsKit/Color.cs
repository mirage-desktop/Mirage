/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using System;
using System.Globalization;

namespace Mirage.GraphicsKit;

/// <summary>
/// Color class, used for drawing.
/// </summary>
public struct Color
{
	#region Constructors

	/// <summary>
	/// Creates a new instance of the <see cref="Color"/> class with 4 channels specified.
	/// </summary>
	/// <param name="A">The Alpha channel.</param>
	/// <param name="R">The Red channel.</param>
	/// <param name="G">The Green channel.</param>
	/// <param name="B">The Blue channel.</param>
	public Color(byte A, byte R, byte G, byte B)
	{
		// Initialize fields.
		_ARGB = GetPacked(A, R, G, B);
		_A = A;
		_R = R;
		_G = G;
		_B = B;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="Color"/> class with 3 channels specified.
	/// </summary>
	/// <param name="R">The Red channel.</param>
	/// <param name="G">The Green channel.</param>
	/// <param name="B">The Blue channel.</param>
	public Color(byte R, byte G, byte B)
	{
		// Initialize fields.
		_ARGB = GetPacked(255, R, G, B);
		_A = 255;
		_R = R;
		_G = G;
		_B = B;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="Color"/> class using an input string.
	/// <list type="table">
	/// <item>cymk(float, float, float, float)</item>
	/// <item>argb(float, float, float, float)</item>
	/// <item>argb(byte, byte, byte, byte)</item>
	/// <item>argb(uint)</item>
	/// <item>rgb(float, float, float)</item>
	/// <item>rgb(byte, byte, byte)</item>
	/// <item>hsl(float, float, float)</item>
	/// <item>#XXXXXXXX</item>
	/// <item>#XXXXXX</item>
	/// <item>Web color name</item>
	/// </list>
	/// </summary>
	/// <param name="ColorInfo">The string to read.</param>
	public Color(string ColorInfo)
	{
		// Initialize the values.
		_ARGB = 0;
		_A = 0;
		_R = 0;
		_G = 0;
		_B = 0;

		// Check if input is invalid.
		if (string.IsNullOrEmpty(ColorInfo))
		{
			return;
		}

		// Get CYMK color.
		if (ColorInfo.StartsWith("cymk("))
		{
			// Get individual components.
			string[] Components = ColorInfo[5..].Split(',');

			// Parse component data.
			byte C = byte.Parse(Components[0]);
			byte Y = byte.Parse(Components[1]);
			byte M = byte.Parse(Components[2]);
			byte K = byte.Parse(Components[3]);

			// Alpha is always 255 with CYMK.
			_A = 255;

			if (K != 255)
			{
				_R = (byte)((255 - C) * (255 - K) / 255);
				_G = (byte)((255 - M) * (255 - K) / 255);
				_B = (byte)((255 - Y) * (255 - K) / 255);
			}
			else
			{
				_R = (byte)(255 - C);
				_G = (byte)(255 - M);
				_B = (byte)(255 - Y);
			}

			// Assign the ARGB value.
			_ARGB = GetPacked(_A, _R, _G, _B);

			return;
		}

		// Get ARGB color.
		if (ColorInfo.StartsWith("argb("))
		{
			// Check if value is packed.
			if (!ColorInfo.Contains(','))
			{
				ARGB = uint.Parse(ColorInfo[5..]);
				return;
			}

			// Get individual components.
			string[] Components = ColorInfo[5..].Split(',');

			// Parse component data.
			try
			{
				_A = byte.Parse(Components[0]);
				_R = byte.Parse(Components[1]);
				_G = byte.Parse(Components[2]);
				_B = byte.Parse(Components[3]);
			}
			catch
			{
				_A = (byte)(float.Parse(Components[0]) * 255);
				_R = (byte)(float.Parse(Components[1]) * 255);
				_G = (byte)(float.Parse(Components[2]) * 255);
				_B = (byte)(float.Parse(Components[3]) * 255);
			}

			// Assign the ARGB value.
			_ARGB = GetPacked(_A, _R, _G, _B);

			return;
		}

		// Get RGB color.
		if (ColorInfo.StartsWith("rgb("))
		{
			// Get individual components.
			string[] Components = ColorInfo[5..].Split(',');

			// Alpha is always 255 with RGB.
			_A = 255;

			// Parse component data.
			try
			{
				_R = byte.Parse(Components[0]);
				_G = byte.Parse(Components[1]);
				_B = byte.Parse(Components[2]);
			}
			catch
			{
				_R = (byte)(float.Parse(Components[0]) * 255);
				_G = (byte)(float.Parse(Components[1]) * 255);
				_B = (byte)(float.Parse(Components[2]) * 255);
			}

			// Assign the ARGB value.
			_ARGB = GetPacked(255, _R, _G, _B);

			return;
		}

		// Get HSV color.
		if (ColorInfo.StartsWith("hsl("))
		{
			// Get individual components.
			string[] Components = ColorInfo[5..].Split(',');

			// Alpha is always 100% with HSL.
			_A = 255;

			float H = float.Parse(Components[0]);
			float S = float.Parse(Components[1]);
			float L = float.Parse(Components[2]);

			S = (float)Math.Clamp(S, 0.0, 1.0);
			L = (float)Math.Clamp(L, 0.0, 1.0);

			// Zero-saturation optimization.
			if (S == 0)
			{
				_R = (byte)L;
				_G = (byte)L;
				_B = (byte)L;

				_ARGB = GetPacked(_A, _R, _G, _B);
				return;
			}

			float Q = L < 0.5 ? (L * S) + L : L + S - (L * S);
			float P = (2 * L) - Q;

			_R = (byte)FromHue(P, Q, H + (1 / 3));
			_G = (byte)FromHue(P, Q, H);
			_B = (byte)FromHue(P, Q, H - (1 / 3));

			// Assign the ARGB value.
			_ARGB = GetPacked(_A, _R, _G, _B);
			return;
		}

		// Get hex color.
		if (ColorInfo.StartsWith('#'))
		{
			// Get color with correct hex length.
			switch (ColorInfo.Length)
			{
				case 9:
					_A = byte.Parse(ColorInfo[1..3], NumberStyles.HexNumber);
					_R = byte.Parse(ColorInfo[3..5], NumberStyles.HexNumber);
					_G = byte.Parse(ColorInfo[5..7], NumberStyles.HexNumber);
					_B = byte.Parse(ColorInfo[7..9], NumberStyles.HexNumber);
					break;
				case 7:
					_A = 255;
					_R = byte.Parse(ColorInfo[1..3], NumberStyles.HexNumber);
					_G = byte.Parse(ColorInfo[3..5], NumberStyles.HexNumber);
					_B = byte.Parse(ColorInfo[5..7], NumberStyles.HexNumber);
					break;
				default:
					throw new FormatException("Hex value is not in correct format!");
			}

			// Assign the ARGB value.
			_ARGB = GetPacked(_A, _R, _G, _B);
			return;
		}

		// Assume input is a color name.
		ARGB = ColorInfo switch
		{
			"AliceBlue" => 0xFFF0F8FF,
			"AntiqueWhite" => 0xFFFAEBD7,
			"Aqua" => 0xFF00FFFF,
			"Aquamarine" => 0xFF7FFFD4,
			"Azure" => 0xFFF0FFFF,
			"Beige" => 0xFFF5F5DC,
			"Bisque" => 0xFFFFE4C4,
			"Black" => 0xFF000000,
			"BlanchedAlmond" => 0xFFFFEBCD,
			"Blue" => 0xFF0000FF,
			"BlueViolet" => 0xFF8A2BE2,
			"Brown" => 0xFFA52A2A,
			"BurlyWood" => 0xFFDEB887,
			"CadetBlue" => 0xFF5F9EA0,
			"Chartreuse" => 0xFF7FFF00,
			"Chocolate" => 0xFFD2691E,
			"Coral" => 0xFFFF7F50,
			"CornflowerBlue" => 0xFF6495ED,
			"Cornsilk" => 0xFFFFF8DC,
			"Crimson" => 0xFFDC143C,
			"Cyan" => 0xFF00FFFF,
			"DarkBlue" => 0xFF00008B,
			"DarkCyan" => 0xFF008B8B,
			"DarkGoldenRod" => 0xFFB8860B,
			"DarkGray" => 0xFFA9A9A9,
			"DarkGrey" => 0xFFA9A9A9,
			"DarkGreen" => 0xFF006400,
			"DarkKhaki" => 0xFFBDB76B,
			"DarkMagenta" => 0xFF8B008B,
			"DarkOliveGreen" => 0xFF556B2F,
			"DarkOrange" => 0xFFFF8C00,
			"DarkOrchid" => 0xFF9932CC,
			"DarkRed" => 0xFF8B0000,
			"DarkSalmon" => 0xFFE9967A,
			"DarkSeaGreen" => 0xFF8FBC8F,
			"DarkSlateBlue" => 0xFF483D8B,
			"DarkSlateGray" => 0xFF2F4F4F,
			"DarkSlateGrey" => 0xFF2F4F4F,
			"DarkTurquoise" => 0xFF00CED1,
			"DarkViolet" => 0xFF9400D3,
			"DeepPink" => 0xFFFF1493,
			"DeepSkyBlue" => 0xFF00BFFF,
			"DimGray" => 0xFF696969,
			"DimGrey" => 0xFF696969,
			"DodgerBlue" => 0xFF1E90FF,
			"FireBrick" => 0xFFB22222,
			"FloralWhite" => 0xFFFFFAF0,
			"ForestGreen" => 0xFF228B22,
			"Fuchsia" => 0xFFFF00FF,
			"Gainsboro" => 0xFFDCDCDC,
			"GhostWhite" => 0xFFF8F8FF,
			"Gold" => 0xFFFFD700,
			"GoldenRod" => 0xFFDAA520,
			"Gray" => 0xFF808080,
			"Grey" => 0xFF808080,
			"Green" => 0xFF008000,
			"GreenYellow" => 0xFFADFF2F,
			"HoneyDew" => 0xFFF0FFF0,
			"HotPink" => 0xFFFF69B4,
			"IndianRed" => 0xFFCD5C5C,
			"Indigo" => 0xFF4B0082,
			"Ivory" => 0xFFFFFFF0,
			"Khaki" => 0xFFF0E68C,
			"Lavender" => 0xFFE6E6FA,
			"LavenderBlush" => 0xFFFFF0F5,
			"LawnGreen" => 0xFF7CFC00,
			"LemonChiffon" => 0xFFFFFACD,
			"LightBlue" => 0xFFADD8E6,
			"LightCoral" => 0xFFF08080,
			"LightCyan" => 0xFFE0FFFF,
			"LightGoldenRodYellow" => 0xFFFAFAD2,
			"LightGray" => 0xFFD3D3D3,
			"LightGrey" => 0xFFD3D3D3,
			"LightGreen" => 0xFF90EE90,
			"LightPink" => 0xFFFFB6C1,
			"LightSalmon" => 0xFFFFA07A,
			"LightSeaGreen" => 0xFF20B2AA,
			"LightSkyBlue" => 0xFF87CEFA,
			"LightSlateGray" => 0xFF778899,
			"LightSlateGrey" => 0xFF778899,
			"LightSteelBlue" => 0xFFB0C4DE,
			"LightYellow" => 0xFFFFFFE0,
			"Lime" => 0xFF00FF00,
			"LimeGreen" => 0xFF32CD32,
			"Linen" => 0xFFFAF0E6,
			"Magenta" => 0xFFFF00FF,
			"Maroon" => 0xFF800000,
			"MediumAquaMarine" => 0xFF66CDAA,
			"MediumBlue" => 0xFF0000CD,
			"MediumOrchid" => 0xFFBA55D3,
			"MediumPurple" => 0xFF9370DB,
			"MediumSeaGreen" => 0xFF3CB371,
			"MediumSlateBlue" => 0xFF7B68EE,
			"MediumSpringGreen" => 0xFF00FA9A,
			"MediumTurquoise" => 0xFF48D1CC,
			"MediumVioletRed" => 0xFFC71585,
			"MidnightBlue" => 0xFF191970,
			"MintCream" => 0xFFF5FFFA,
			"MistyRose" => 0xFFFFE4E1,
			"Moccasin" => 0xFFFFE4B5,
			"NavajoWhite" => 0xFFFFDEAD,
			"Navy" => 0xFF000080,
			"OldLace" => 0xFFFDF5E6,
			"Olive" => 0xFF808000,
			"OliveDrab" => 0xFF6B8E23,
			"Orange" => 0xFFFFA500,
			"OrangeRed" => 0xFFFF4500,
			"Orchid" => 0xFFDA70D6,
			"PaleGoldenRod" => 0xFFEEE8AA,
			"PaleGreen" => 0xFF98FB98,
			"PaleTurquoise" => 0xFFAFEEEE,
			"PaleVioletRed" => 0xFFDB7093,
			"PapayaWhip" => 0xFFFFEFD5,
			"PeachPuff" => 0xFFFFDAB9,
			"Peru" => 0xFFCD853F,
			"Pink" => 0xFFFFC0CB,
			"Plum" => 0xFFDDA0DD,
			"PowderBlue" => 0xFFB0E0E6,
			"Purple" => 0xFF800080,
			"RebeccaPurple" => 0xFF663399,
			"Red" => 0xFFFF0000,
			"RosyBrown" => 0xFFBC8F8F,
			"RoyalBlue" => 0xFF4169E1,
			"SaddleBrown" => 0xFF8B4513,
			"Salmon" => 0xFFFA8072,
			"SandyBrown" => 0xFFF4A460,
			"SeaGreen" => 0xFF2E8B57,
			"SeaShell" => 0xFFFFF5EE,
			"Sienna" => 0xFFA0522D,
			"Silver" => 0xFFC0C0C0,
			"SkyBlue" => 0xFF87CEEB,
			"SlateBlue" => 0xFF6A5ACD,
			"SlateGray" => 0xFF708090,
			"SlateGrey" => 0xFF708090,
			"Snow" => 0xFFFFFAFA,
			"SpringGreen" => 0xFF00FF7F,
			"SteelBlue" => 0xFF4682B4,
			"Tan" => 0xFFD2B48C,
			"Teal" => 0xFF008080,
			"Thistle" => 0xFFD8BFD8,
			"Tomato" => 0xFFFF6347,
			"Turquoise" => 0xFF40E0D0,
			"Violet" => 0xFFEE82EE,
			"Wheat" => 0xFFF5DEB3,
			"White" => 0xFFFFFFFF,
			"WhiteSmoke" => 0xFFF5F5F5,
			"Yellow" => 0xFFFFFF00,
			"YellowGreen" => 0xFF9ACD32,
			_ => throw new($"Color '{ColorInfo}' does not exist!"),
		};

		return;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="Color"/> class.
	/// </summary>
	/// <param name="ARGB">A 32-bit packed ARGB value.</param>
	public Color(uint ARGB)
	{
		// Initialize values.
		_ARGB = ARGB;
		_A = 0;
		_R = 0;
		_G = 0;
		_B = 0;

		this.ARGB = ARGB;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Property used to get the overall brightness of the color.
	/// </summary>
	public float Brightness => (Max(this) + Min(this)) / (byte.MaxValue * 2f);

	/// <summary>
	/// Packed ARGB value of the color.
	/// </summary>
	public uint ARGB
	{
		readonly get => _ARGB;
		set
		{
			_ARGB = value;
			_A = (byte)((value >> 24) & 255);
			_R = (byte)((value >> 16) & 255);
			_G = (byte)((value >> 8) & 255);
			_B = (byte)(value & 255);
		}
	}

	/// <summary>
	/// Alpha channel of the color.
	/// </summary>
	public byte A
	{
		readonly get => _A;
		set
		{
			_A = Math.Clamp(value, (byte)0, (byte)255);
			_ARGB = GetPacked(_A, _R, _G, _B);
		}
	}

	/// <summary>
	/// Red channel of the color.
	/// </summary>
	public byte R
	{
		readonly get => _R;
		set
		{
			_R = Math.Clamp(value, (byte)0, (byte)255);
			_ARGB = GetPacked(_A, _R, _G, _B);
		}
	}

	/// <summary>
	/// Green channel of the color.
	/// </summary>
	public byte G
	{
		readonly get => _G;
		set
		{
			_G = Math.Clamp(value, (byte)0, (byte)255);
			_ARGB = GetPacked(_A, _R, _G, _B);
		}
	}

	/// <summary>
	/// Blue channel of the color.
	/// </summary>
	public byte B
	{
		readonly get => _B;
		set
		{
			_B = Math.Clamp(value, (byte)0, (byte)255);
			_ARGB = GetPacked(_A, _R, _G, _B);
		}
	}

	#endregion

	#region Operators

	public static Color operator +(Color Original, Color Value) => new(
		(byte)Math.Clamp(Original._A + Value._A, 0, 255),
		(byte)Math.Clamp(Original._R + Value._R, 0, 255),
		(byte)Math.Clamp(Original._G + Value._G, 0, 255),
		(byte)Math.Clamp(Original._B + Value._B, 0, 255));
	public static Color operator -(Color Original, Color Value) => new(
		(byte)Math.Clamp(Original._A - Value._A, 0, 255),
		(byte)Math.Clamp(Original._R - Value._R, 0, 255),
		(byte)Math.Clamp(Original._G - Value._G, 0, 255),
		(byte)Math.Clamp(Original._B - Value._B, 0, 255));
	public static Color operator *(Color Original, Color Value) => new(
		(byte)Math.Clamp(Original._A * Value._A, 0, 255),
		(byte)Math.Clamp(Original._R * Value._R, 0, 255),
		(byte)Math.Clamp(Original._G * Value._G, 0, 255),
		(byte)Math.Clamp(Original._B * Value._B, 0, 255));
	public static Color operator /(Color Original, Color Value) => new(
		(byte)Math.Clamp(Original._A / Value._A, 0, 255),
		(byte)Math.Clamp(Original._R / Value._R, 0, 255),
		(byte)Math.Clamp(Original._G / Value._G, 0, 255),
		(byte)Math.Clamp(Original._B / Value._B, 0, 255));

	public static Color operator +(Color Original, float Value) => new(
		(byte)Math.Clamp(Original.A + (Value * 255), 0, 255),
		(byte)Math.Clamp(Original.R + (Value * 255), 0, 255),
		(byte)Math.Clamp(Original.G + (Value * 255), 0, 255),
		(byte)Math.Clamp(Original.B + (Value * 255), 0, 255));
	public static Color operator -(Color Original, float Value) => new(
		(byte)Math.Clamp(Original.A - (Value * 255), 0, 255),
		(byte)Math.Clamp(Original.R - (Value * 255), 0, 255),
		(byte)Math.Clamp(Original.G - (Value * 255), 0, 255),
		(byte)Math.Clamp(Original.B - (Value * 255), 0, 255));
	public static Color operator *(Color Original, float Value) => new(
		(byte)Math.Clamp(Original.A * Value, 0, 255),
		(byte)Math.Clamp(Original.R * Value, 0, 255),
		(byte)Math.Clamp(Original.G * Value, 0, 255),
		(byte)Math.Clamp(Original.B * Value, 0, 255));
	public static Color operator /(Color Original, float Value) => new(
		(byte)Math.Clamp(Original.A / Value, 0, 255),
		(byte)Math.Clamp(Original.R / Value, 0, 255),
		(byte)Math.Clamp(Original.G / Value, 0, 255),
		(byte)Math.Clamp(Original.B / Value, 0, 255));

	public static Color operator +(float Value, Color Original) => new(
		(byte)Math.Clamp((Value * 255) + Original.A, 0, 255),
		(byte)Math.Clamp((Value * 255) + Original.R, 0, 255),
		(byte)Math.Clamp((Value * 255) + Original.G, 0, 255),
		(byte)Math.Clamp((Value * 255) + Original.B, 0, 255));
	public static Color operator -(float Value, Color Original) => new(
		(byte)Math.Clamp((Value * 255) - Original.A, 0, 255),
		(byte)Math.Clamp((Value * 255) - Original.R, 0, 255),
		(byte)Math.Clamp((Value * 255) - Original.G, 0, 255),
		(byte)Math.Clamp((Value * 255) - Original.B, 0, 255));
	public static Color operator *(float Value, Color Original) => new(
		(byte)Math.Clamp(Value * 255 * Original.A, 0, 255),
		(byte)Math.Clamp(Value * 255 * Original.R, 0, 255),
		(byte)Math.Clamp(Value * 255 * Original.G, 0, 255),
		(byte)Math.Clamp(Value * 255 * Original.B, 0, 255));
	public static Color operator /(float Value, Color Original) => new(
		(byte)Math.Clamp(Value * 255 / Original.A, 0, 255),
		(byte)Math.Clamp(Value * 255 / Original.R, 0, 255),
		(byte)Math.Clamp(Value * 255 / Original.G, 0, 255),
		(byte)Math.Clamp(Value * 255 / Original.B, 0, 255));

	public static bool operator ==(Color C1, Color C2)
	{
		return C1.ARGB == C2.ARGB;
	}
	public static bool operator !=(Color C1, Color C2)
	{
		return C1.ARGB != C2.ARGB;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Blends two colors together based on their alpha values.
	/// </summary>
	/// <param name="Background">The background color.</param>
	/// <param name="Foreground">The foreground color.</param>
	/// <returns>Mixed color.</returns>
	public static Color AlphaBlend(Color Background, Color Foreground)
	{
		if (Foreground._A == 255)
		{
			return Foreground;
		}
		if (Foreground._A == 0)
		{
			return Background;
		}

		// TODOTODO port over alpha background support from canvas

		byte alpha = Foreground.A;
    	int invAlpha = 256 - Foreground.A;
		return new()
		{
			A = 255,
			R = (byte)((alpha * Foreground._R + invAlpha * Background._R) >> 8),
			G = (byte)((alpha * Foreground._G + invAlpha * Background._G) >> 8),
			B = (byte)((alpha * Foreground._B + invAlpha * Background._B) >> 8)
		};
	}

	/// <summary>
	/// Converts an ARGB color to it's packed ARGB format.
	/// </summary>
	/// <param name="A">Alpha channel.</param>
	/// <param name="R">Red channel.</param>
	/// <param name="G">Green channel.</param>
	/// <param name="B">Blue channel.</param>
	/// <returns>Packed value.</returns>
	private static uint GetPacked(float A, float R, float G, float B)
	{
		return BitConverter.ToUInt32(new byte[] { (byte)B, (byte)G, (byte)R, (byte)A });
	}

	/// <summary>
	/// Normalizes the color to be between 0.0 and 1.0.
	/// </summary>
	/// <returns>A normalized color.</returns>
	public static Color Normalize(Color ToNormalize)
	{
		// Don't use operators as to preserve the alpha value.
		return ToNormalize / 255;
	}

	/// <summary>
	/// Internal method, used by <see cref="FromHSL(float, float, float)"/>./>
	/// See: <seealso cref="https://github.com/CharlesStover/hsl2rgb-js/blob/master/src/hsl2rgb.js"/>
	/// </summary>
	/// <param name="P">Unknown.</param>
	/// <param name="Q">Unknown.</param>
	/// <param name="T">Unknown.</param>
	/// <returns>Unknown.</returns>
	private static float FromHue(float P, float Q, float T)
	{
		if (T < 0)
		{
			T++;
		}
		if (T > 1)
		{
			T--;
		}
		if (T < 1 / 6)
		{
			return P + ((Q - P) * 6 * T);
		}
		if (T < 0.5)
		{
			return Q;
		}
		if (T < 2 / 3)
		{
			return P + ((Q - P) * ((2 / 3) - T) * 6);
		}

		return P;
	}

	/// <summary>
	/// Inverts the specified color.
	/// </summary>
	/// <param name="ToInvert">The color that will be inverted.</param>
	/// <returns>An inverted variant of the input.</returns>
	public static Color Invert(Color ToInvert)
	{
		return White - ToInvert;
	}

	/// <summary>
	/// The function to linearly interpolate between 2 colors. (32-bit)
	/// </summary>
	/// <param name="StartValue">The color to start with.</param>
	/// <param name="EndValue">The color to end with.</param>
	/// <param name="Index">Any number between 0.0 and 1.0.</param>
	/// <returns>The value between 'StartValue' and 'EndValue' as marked by 'Index'.</returns>
	public static Color Lerp(Color StartValue, Color EndValue, float Index)
	{
		// Ensure 'Index' is between 0.0 and 1.0.
		Index = (float)Math.Clamp(Index, 0.0, 1.0);

		return new()
		{
			A = (byte)Math.Clamp(StartValue.A + ((EndValue.A - StartValue.A) * Index), 0, 255),
			R = (byte)Math.Clamp(StartValue.R + ((EndValue.R - StartValue.R) * Index), 0, 255),
			G = (byte)Math.Clamp(StartValue.G + ((EndValue.G - StartValue.G) * Index), 0, 255),
			B = (byte)Math.Clamp(StartValue.B + ((EndValue.B - StartValue.B) * Index), 0, 255),
		};
	}

	/// <summary>
	/// Gets the value of the channel with the most value.
	/// </summary>
	/// <param name="Color">The color to calculate.</param>
	/// <returns><see cref="R"/> if <see cref="R"/> is more than <see cref="G"/> and <see cref="B"/>, etc...</returns>
	public static float Max(Color Color)
	{
		// Get the minimum value of each channel.
		return MathF.Max(Color.R, MathF.Max(Color.G, Color.B));
	}

	/// <summary>
	/// Gets the value of the channel with the least value.
	/// </summary>
	/// <param name="Color">The color to calculate.</param>
	/// <returns><see cref="R"/> if <see cref="R"/> is less than <see cref="G"/> and <see cref="B"/>, etc...</returns>
	public static float Min(Color Color)
	{
		// Get the minimum value of each channel.
		return MathF.Min(Color.R, MathF.Min(Color.G, Color.B));
	}

	/// <summary>
	/// Converts the color to be only in grayscale.
	/// </summary>
	/// <returns>Grayscale color.</returns>
	public Color ToGrayscale()
	{
		// TODOTODO: Is this correct?
		return new(255, (byte)(Brightness * 255), (byte)(Brightness * 255), (byte)(Brightness * 255));
	}

	#endregion

	#region Fields

	#region Extended Colors

	public static readonly Color White = new(255, 255, 255, 255);
	public static readonly Color Black = new(255, 0, 0, 0);
	public static readonly Color Cyan = new(255, 0, 255, 255);
	public static readonly Color Red = new(255, 255, 0, 0);
	public static readonly Color Green = new(255, 0, 255, 0);
	public static readonly Color Blue = new(255, 0, 0, 255);
	public static readonly Color CoolGreen = new(255, 54, 94, 53);
	public static readonly Color Magenta = new(255, 255, 0, 255);
	public static readonly Color Yellow = new(255, 255, 255, 0);
	public static readonly Color HotPink = new(255, 230, 62, 109);
	public static readonly Color UbuntuPurple = new(255, 66, 5, 22);
	public static readonly Color GoogleBlue = new(255, 66, 133, 244);
	public static readonly Color GoogleGreen = new(255, 52, 168, 83);
	public static readonly Color GoogleYellow = new(255, 251, 188, 5);
	public static readonly Color GoogleRed = new(255, 234, 67, 53);
	public static readonly Color DeepOrange = new(255, 255, 64, 0);
	public static readonly Color RubyRed = new(255, 204, 52, 45);
	public static readonly Color Transparent = new(0, 0, 0, 0);
	public static readonly Color StackOverflowOrange = new(255, 244, 128, 36);
	public static readonly Color StackOverflowBlack = new(255, 34, 36, 38);
	public static readonly Color StackOverflowWhite = new(255, 188, 187, 187);
	public static readonly Color DeepGray = new(255, 25, 25, 25);
	public static readonly Color LightGray = new(255, 125, 125, 125);
	public static readonly Color SuperOrange = new(255, 255, 99, 71);
	public static readonly Color FakeGrassGreen = new(255, 60, 179, 113);
	public static readonly Color DeepBlue = new(255, 51, 47, 208);
	public static readonly Color BloodOrange = new(255, 255, 123, 0);
	public static readonly Color LightBlack = new(255, 25, 25, 25);
	public static readonly Color LighterBlack = new(255, 50, 50, 50);
	public static readonly Color ClassicBlue = new(255, 52, 86, 139);
	public static readonly Color LivingCoral = new(255, 255, 111, 97);
	public static readonly Color UltraViolet = new(255, 107, 91, 149);
	public static readonly Color Greenery = new(255, 136, 176, 75);
	public static readonly Color Emerald = new(255, 0, 155, 119);
	public static readonly Color LightPurple = new(0xFFA0A5DD);
	public static readonly Color Minty = new(0xFF74C68B);
	public static readonly Color SunsetRed = new(0xFFE07572);
	public static readonly Color LightYellow = new(0xFFF9C980);

	#endregion

	private uint _ARGB;
	private byte _A;
	private byte _R;
	private byte _G;
	private byte _B;

	#endregion
}
