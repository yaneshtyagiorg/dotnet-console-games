using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace NewGame
{
	class Program
	{
		static ColorBuffer screenBufferA;
		static ColorBuffer screenBufferB;
		static char[] textBuffer;
		static bool gameRunning = true;
		static (int Left, int Top) characterPosition = (0, 0);
		static DateTime previoiusRender = DateTime.Now;

		static Bitmap character = new(Path.Combine("Images", "character.png"));
		static ColorBuffer characterBuffer = ColorBuffer.New(character);
		static Bitmap transparent = new(Path.Combine("Images", "transparent.png"));
		static ColorBuffer transparentBuffer = ColorBuffer.New(transparent);

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
			if (screenBufferB is null || screenBufferB.Width != width || screenBufferB.Height != height)
			{
				InitializeScreenBuffer(width, height);
			}

			screenBufferB.Fill(Color.Black);
			ColorBuffer.Render(characterBuffer, screenBufferB, characterPosition.Left, characterPosition.Top);
			ColorBuffer.Render(transparentBuffer, screenBufferB, 0, 0);
			int length = ColorBuffer.WriteChangesConsoleBuffer(screenBufferA, screenBufferB, textBuffer);
			(screenBufferA, screenBufferB) = (screenBufferB, screenBufferA);
			//int length = screenBufferB.WriteConsoleBuffer(textBuffer);
			Console.SetCursorPosition(0, 0);
			Console.Write(textBuffer, 0, length);
		}

		static void InitializeScreenBuffer(int width, int height)
		{
			screenBufferA = ColorBuffer.New(width, height);
			screenBufferB = ColorBuffer.New(width, height);
			textBuffer = new char[screenBufferB.MaxConsoleBufferLength()];
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