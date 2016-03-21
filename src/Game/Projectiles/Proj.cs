using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Game.Spells
{
	struct Proj
	{
		public Proj(Vector2 pos, Vector2 speed)
		{
			this.pos = pos;
			this.speed = speed;
		}

		public Vector2 pos;
		public Vector2 speed;
	}
}
