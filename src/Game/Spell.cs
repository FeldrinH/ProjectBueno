using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Game.Spells
{
    class Spell
    {
		public SkillShape shape;
		public SkillProp propLeft;
		public SkillProp propTop;
		public SkillProp propBottom;
		public Point shapeBounds { get; protected set; }
		public Point propLeftBounds { get; protected set; }
		public Point propTopBounds { get; protected set; }
		public Point propBottomBounds { get; protected set; }

		public Spell()
		{
			shapeBounds = new Point(0, 0);
			propLeftBounds = new Point(0, 0);
			propTopBounds = new Point(0, 0);
			propBottomBounds = new Point(0, 0);
		}

		public void onClick()
		{
		}

		public void DrawButtons()
		{
		}
    }
}
