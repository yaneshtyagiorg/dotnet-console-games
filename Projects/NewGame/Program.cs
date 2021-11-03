using System;
using System.Drawing;
using System.IO;

namespace NewGame
{
	class Program
	{
		static void Main()
		{
			ColorBuffer screenBuffer = ColorBuffer.New(Console.WindowWidth, Console.WindowHeight);
			char[] textBuffer = new char[screenBuffer.MaxConsoleBufferLength()];

			Bitmap bitmap = new(Path.Combine("Images", "character.png"));
			ColorBuffer buttonBuffer = ColorBuffer.New(bitmap);
			ColorBuffer.Render(buttonBuffer, screenBuffer, 0, 0);

			int length = screenBuffer.WriteConsoleBuffer(textBuffer, false);
			Console.Write(textBuffer, 0, length);
		}
	}
}