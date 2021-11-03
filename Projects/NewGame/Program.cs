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

			Bitmap character = new(Path.Combine("Images", "character.png"));
			ColorBuffer characterBuffer = ColorBuffer.New(character);
			ColorBuffer.Render(characterBuffer, screenBuffer, 0, 0);

			Bitmap transparent = new(Path.Combine("Images", "transparent.png"));
			ColorBuffer transparentBuffer = ColorBuffer.New(transparent);
			ColorBuffer.Render(transparentBuffer, screenBuffer, 0, 0);

			int length = screenBuffer.WriteConsoleBuffer(textBuffer, false);
			Console.Write(textBuffer, 0, length);
		}
	}
}