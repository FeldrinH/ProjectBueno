using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Utility
{
	static class RectangleHelper
	{
		public static Vector2 ToVector(this Rectangle rect)
		{
			return new Vector2(rect.X, rect.Y);
		}

		public static Rectangle Scale(this Rectangle rect, float scale)
		{
			return new Rectangle(rect.X, rect.Y, (int)(rect.Width * scale), rect.Height);
		}

		public static Rectangle ScaleSize(this Rectangle rect, float scale)
		{
			return new Rectangle(0, 0, (int)(rect.Width * scale), rect.Height);
		}
	}
}
