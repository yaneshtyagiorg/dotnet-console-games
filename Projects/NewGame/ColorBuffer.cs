using System;
using System.Drawing;

namespace NewGame
{
	public class ColorBuffer
	{
		private Color[] Pixels { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public Color this[int left, int top]
		{
			get => Pixels[left + top * Width];
			set => Pixels[left + top * Width] = value;
		}

		public int MaxConsoleBufferLength() => MaxConsoleBufferLength(this);

		public static int MaxConsoleBufferLength(ColorBuffer colorBuffer) =>
			colorBuffer.Width * colorBuffer.Height * 19 + colorBuffer.Height * Environment.NewLine.Length;

		public int WriteConsoleBuffer(Span<char> charBuffer) => WriteConsoleBuffer(this, charBuffer);

		public static int WriteConsoleBuffer(ColorBuffer colorBuffer, Span<char> charBuffer)
		{
			int i = 0;
			Color? previousColor = null;
			foreach (Color color in colorBuffer.Pixels)
			{
				if (previousColor is null || previousColor != color)
				{
					if (charBuffer.Length <= i + 19)
					{
						return i;
					}
					// ANSI escape codes: https://en.wikipedia.org/wiki/ANSI_escape_code
					charBuffer.Write(ref i, "\u001b[48;2;");
					charBuffer.Write(ref i, color.R);
					charBuffer.Write(ref i, ';');
					charBuffer.Write(ref i, color.G);
					charBuffer.Write(ref i, ';');
					charBuffer.Write(ref i, color.B);
					charBuffer.Write(ref i, 'm');
				}
				charBuffer.Write(ref i, ' ');
			}
			return i;
		}

		public static int WriteChangesConsoleBuffer(ColorBuffer previous, ColorBuffer current, Span<char> charBuffer)
		{
			if (previous is null || previous.Width != current.Width || previous.Height != current.Height)
			{
				return WriteConsoleBuffer(current, charBuffer);
			}
			int i = 0;

			for (int h = 0; h < current.Height; h++)
			{
				for (int w = 0; w < current.Width; w++)
				{
					if (previous[w, h] != current[w, h])
					{
						// ANSI escape codes: https://en.wikipedia.org/wiki/ANSI_escape_code
						Color color = current[w, h];
						charBuffer.Write(ref i, "\u001b[");
						charBuffer.Write(ref i, h);
						charBuffer.Write(ref i, ';');
						charBuffer.Write(ref i, w);
						charBuffer.Write(ref i, 'H');
						charBuffer.Write(ref i, "\u001b[48;2;");
						charBuffer.Write(ref i, color.R);
						charBuffer.Write(ref i, ';');
						charBuffer.Write(ref i, color.G);
						charBuffer.Write(ref i, ';');
						charBuffer.Write(ref i, color.B);
						charBuffer.Write(ref i, 'm');
						charBuffer.Write(ref i, ' ');
					}
				}
			}
			return i;
		}

		public void Fill(Color color) => Fill(this, color);

		public static void Fill(ColorBuffer buffer, Color color)
		{
			Array.Fill(buffer.Pixels, color);
		}

		public static void Render(ColorBuffer source, ColorBuffer target, int left, int top)
		{
			for (int h = top < 0 ? -top : 0; h < source.Height && h + top < target.Height; h++)
			{
				for (int w = left < 0 ? -left : 0; w < source.Width && w + left < target.Width; w++)
				{
					Color sourceColor = source[w, h];
					byte alpha = Math.Clamp(source[w, h].A, (byte)0, (byte)255);
					switch (alpha)
					{
						case 255: target[w + left, h + top] = sourceColor; break;
						case 0: break;
						default:
							Color targetColor = target[w + left, h + top];
							float ratio = alpha / 255f;
							target[w + left, h + top] = Helpers.LinearInterpolation(targetColor, sourceColor, ratio);
							break;
					}
				}
			}
		}

		public static ColorBuffer New(int width, int height, Color? defaultColor = null)
		{
			ColorBuffer buffer = new()
			{
				Pixels = new Color[width * height],
				Width = width,
				Height = height,
			};
			Array.Fill(buffer.Pixels, defaultColor ?? Color.Black);
			return buffer;
		}

		public static ColorBuffer New(Bitmap bitmap)
		{
			ColorBuffer buffer = new()
			{
				Pixels = new Color[bitmap.Width * bitmap.Height],
				Width = bitmap.Width,
				Height = bitmap.Height,
			};
			for (int h = 0, i = 0; h < buffer.Height; h++)
			{
				for (int w = 0; w < buffer.Width; w++, i++)
				{
					buffer.Pixels[i] = bitmap.GetPixel(w, h);
				}
			}
			return buffer;
		}
	}
}
