using System;

namespace NewGame
{
	public class Animation
	{
		public struct Frame
		{
			public ColorBuffer ColorBuffer { get; set; }
			public TimeSpan Duration { get; set; }
		}

		public Frame[] Frames { get; set; }
	}
}
