using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Utility
{
	static class AngleVector
	{
		public static Vector2 Vector(float angle)
		{
			return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
		}
		public static Vector2 Vector(double angle)
		{
			return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
		}

		public static float Angle(Vector2 vector)
		{
			return (float)Math.Atan2(vector.X, -vector.Y);
		}
		public static double AngleD(Vector2 vector)
		{
			return Math.Atan2(vector.X, -vector.Y);
		}
	}
}
