using System;
using System.Drawing;

namespace NewGame
{
	public static class Helpers
	{
		public static byte LinearInterpolation(byte a, byte b, float ratio)
		{
			if (ratio < 0 || 255 < ratio) throw new ArgumentOutOfRangeException(nameof(ratio), ratio, "ratio < 0 || 255 < ratio");
			return (byte)(a < b ? ratio * (b - a) + a : (1 - ratio) * (a - b) + b);
		}

		public static Color LinearInterpolation(Color a, Color b, float ratio)
		{
			if (ratio < 0 || 255 < ratio) throw new ArgumentOutOfRangeException(nameof(ratio), ratio, "ratio < 0 || 255 < ratio");
			return Color.FromArgb(
				LinearInterpolation(a.R, b.R, ratio),
				LinearInterpolation(a.G, b.G, ratio),
				LinearInterpolation(a.B, b.B, ratio));
		}

		public static void Write<T>(this Span<T> buffer, ref int i, T value)
		{
			buffer[i++] = value;
		}

		public static void Write<T>(this Span<T> buffer, ref int i, ReadOnlySpan<T> span)
		{
			foreach (T t in span)
			{
				buffer[i++] = t;
			}
		}

		public static void Write(this Span<char> buffer, ref int i, byte @byte)
		{
			@byte.TryFormat(buffer[i..], out int chars);
			i += chars;
		}

		public static void Write(this Span<char> buffer, ref int i, int @int)
		{
			@int.TryFormat(buffer[i..], out int chars);
			i += chars;
		}
	}
}
