using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Utility
{
	static class VectorHelper
	{
		public static Vector2 MultX(this Vector2 vec, float mult)
		{
			vec.X *= mult;
			return vec;
		}

		public static Vector2 MultY(this Vector2 vec, float mult)
		{
			vec.Y *= mult;
			return vec;
		}
	}
}
