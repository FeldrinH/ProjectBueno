using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Game.Spells
{
    public class Spell
    {
		public SkillShape shape;
		public SkillProp prop;
		public SkillProp modpTop;
		public SkillProp modBottom;
		public Point shapeBounds { get; protected set; }
		public Point propBounds { get; protected set; }
		public Point modTopBounds { get; protected set; }
		public Point modBottomBounds { get; protected set; }

		public Spell()
		{
			shapeBounds = new Point(0, 0);
			propBounds = new Point(0, 0);
			modTopBounds = new Point(0, 0);
			modBottomBounds = new Point(0, 0);
		}

		public void onClick(float downscale)
		{
		}

		public void DrawButtons()
		{
		}
    }
}
