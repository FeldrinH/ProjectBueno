using Microsoft.Xna.Framework;
using ProjectBueno.Game.Entities;
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
		public SkillProp modTop;
		public SkillProp modBottom;
		public int cooldown;
		public float damage;
		protected Texture2D texture;
		public static Point shapeBounds { get; protected set; }
		public static Point propBounds { get; protected set; }
		public static Point modTopBounds { get; protected set; }
		public static Point modBottomBounds { get; protected set; }

		public Spell()
		{
			shapeBounds = new Point(0, 0);
			propBounds = new Point(0, 0);
			modTopBounds = new Point(0, 0);
			modBottomBounds = new Point(0, 0);
		}

		public void createProjectile(Vector2 pos, List<Projectile> projectiles)
		{
			shape.generateProjectiles(pos, projectiles);
		}

		public void onClick(float downscale)
		{
		}

		public void DrawButtons()
		{
		}
	}
	public class SkillData
	{
		public SkillProp prop;
		public SkillProp modTop;
		public SkillProp modBottom;
		public Animated
		public float damage;
	}
}
