using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace NewGame
{
	class Program
	{
		static ColorBuffer screenBuffer;
		static char[] textBuffer;
		static bool gameRunning = true;
		static (int Left, int Top) characterPosition = (0, 0);
		static DateTime previoiusRender = DateTime.Now;

		static void Main()
		{
			Console.CursorVisible = false;
			while (gameRunning)
			{
				HandleInput();
				Render();
				SleepAfterRender();
			}
		}

		static void HandleInput()
		{
			bool canMove = true;
			while (Console.KeyAvailable)
			{
				ConsoleKey key = Console.ReadKey(true).Key;
				switch (key)
				{
					case
						ConsoleKey.UpArrow    or ConsoleKey.W or
						ConsoleKey.DownArrow  or ConsoleKey.S or
						ConsoleKey.LeftArrow  or ConsoleKey.A or
						ConsoleKey.RightArrow or ConsoleKey.D:
						if (canMove)
						{
							characterPosition = key switch
							{
								ConsoleKey.UpArrow    or ConsoleKey.W => (characterPosition.Left, characterPosition.Top - 1),
								ConsoleKey.DownArrow  or ConsoleKey.S => (characterPosition.Left, characterPosition.Top + 1),
								ConsoleKey.LeftArrow  or ConsoleKey.A => (characterPosition.Left - 1, characterPosition.Top),
								ConsoleKey.RightArrow or ConsoleKey.D => (characterPosition.Left + 1, characterPosition.Top),
								_ => throw new Exception("bug"),
							};
							canMove = false;
						}
						break;
					case ConsoleKey.Escape:
						gameRunning = false;
						return;
				}
			}
		}

		static void Render()
		{
			var (width, height) = GetWidthAndHeight();
			if (screenBuffer is null || screenBuffer.Width != width || screenBuffer.Height != height)
			{
				InitializeScreenBuffer(width, height);
			}

			screenBuffer.Fill(Color.Black);

			Bitmap character = new(Path.Combine("Images", "character.png"));
			ColorBuffer characterBuffer = ColorBuffer.New(character);
			ColorBuffer.Render(characterBuffer, screenBuffer, characterPosition.Left, characterPosition.Top);

			Bitmap transparent = new(Path.Combine("Images", "transparent.png"));
			ColorBuffer transparentBuffer = ColorBuffer.New(transparent);
			ColorBuffer.Render(transparentBuffer, screenBuffer, 0, 0);

			int length = screenBuffer.WriteConsoleBuffer(textBuffer, false);
			Console.SetCursorPosition(0, 0);
			Console.Write(textBuffer, 0, length);
		}

		static void InitializeScreenBuffer(int width, int height)
		{
			screenBuffer = ColorBuffer.New(Console.WindowWidth, Console.WindowHeight);
			textBuffer = new char[screenBuffer.MaxConsoleBufferLength()];
		}

		static (int Width, int Height) GetWidthAndHeight()
		{
			RestartRender:
			int width = Console.WindowWidth;
			int height = Console.WindowHeight;
			if (OperatingSystem.IsWindows())
			{
				try
				{
					if (Console.BufferHeight != height) Console.BufferHeight = height;
					if (Console.BufferWidth != width)   Console.BufferWidth = width;
				}
				catch (ArgumentOutOfRangeException)
				{
					Console.Clear();
					goto RestartRender;
				}
			}
			return (width, height);
		}

		static void SleepAfterRender()
		{
			// frame rate control
			// battle view is currently targeting 60 frames per second
			DateTime now = DateTime.Now;
			TimeSpan sleep = TimeSpan.FromMilliseconds(16.66) - (now - previoiusRender);
			if (sleep > TimeSpan.Zero)
			{
				Thread.Sleep(sleep);
			}
			previoiusRender = DateTime.Now;
		}
	}
}