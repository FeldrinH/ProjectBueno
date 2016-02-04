using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
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
		public static Rectangle shapeBounds { get; protected set; }
		public static Rectangle propBounds { get; protected set; }
		public static Rectangle modTopBounds { get; protected set; }
		public static Rectangle modBottomBounds { get; protected set; }

		protected SkillHandler skillHandler;

		public Spell()
		{
		}

		static Spell()
		{
			shapeBounds = new Rectangle(99,18, Skill.buttonSize, Skill.buttonSize);
			propBounds = new Rectangle(114, 18, Skill.buttonSize, Skill.buttonSize);
			modTopBounds = new Rectangle(128, 11, Skill.buttonSize, Skill.buttonSize);
			modBottomBounds = new Rectangle(128, 25, Skill.buttonSize, Skill.buttonSize);
		}

		public void createProjectile(Vector2 pos, List<Projectile> projectiles)
		{
			shape.generateProjectiles(pos, projectiles);
		}

		public bool onClick(float mouseX, float mouseY, ref Skill curHeld)
		{
			if (shapeBounds.Contains(mouseX, mouseY) && (curHeld == null || curHeld is SkillShape))
			{
				shape = (SkillShape)curHeld;
			}
			else if (propBounds.Contains(mouseX, mouseY) && (curHeld == null || curHeld is SkillProp))
			{
				prop = (SkillProp)curHeld;
			}
			else if (modTopBounds.Contains(mouseX, mouseY) && (curHeld == null || curHeld is SkillProp))
			{
				modTop = (SkillProp)curHeld;
			}
			else if (modBottomBounds.Contains(mouseX, mouseY) && (curHeld == null || curHeld is SkillProp))
			{
				modBottom = (SkillProp)curHeld;
			}
			else
			{
				return false;
			}
			curHeld = null;
			return true;
		}

		public Spell getCopy()
		{
			return (Spell)MemberwiseClone();
		}

		public void DrawButtons(float mouseX, float mouseY)
		{
			if (shape != null)
			{
				shape.DrawHightlight(shapeBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(shapeBounds, mouseX, mouseY);
			}
			if (prop != null)
			{
				prop.DrawHightlight(propBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(propBounds, mouseX, mouseY);
			}
			if (modTop != null)
			{
				modTop.DrawHightlight(modTopBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(modTopBounds, mouseX, mouseY);
			}
			if (modBottom != null)
			{
				modBottom.DrawHightlight(modBottomBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(modBottomBounds, mouseX, mouseY);
			}
		}
	}
}
