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
	public class Spell //Data class
	{
		public Spell(SkillShape shape, SkillProp prop, SkillProp modTop, SkillProp modBottom)
		{
			this.shape = shape;
			this.prop = prop;
			this.modTop = modTop;
			this.modBottom = modBottom;
			if (shape == null)
			{
				cooldown = -1;
			}
			else
			{
				cooldown = 0;
				cooldown += shape.cooldown;
				cooldown += modBottom == null ? 0 : modBottom.cooldown;
				cooldown += modBottom == null ? 0 : modBottom.cooldown;
				cooldown += modBottom == null ? 0 : modBottom.cooldown;
			}
		}

		public float getDamage(Entity target)
		{
			return 1.0f;
		}

		public float getHeal(Player caster)
		{
			return 1.0f;
		}

		public void onCollide(Player caster, Entity target, Vector2 pos, Vector2 speed)
		{
			if (target.canDamage)
			{
				target.dealDamage(getDamage(target), Vector2.Zero);//shape.getKnockback(caster, target, pos, speed);
			}
		}

		public readonly SkillShape shape;
		public readonly SkillProp prop;
		public readonly SkillProp modTop;
		public readonly SkillProp modBottom;
		public readonly int cooldown;
	}

	public class SpellContainer //Wrapper for player spells
	{
		public SpellContainer()
		{
			spell = new Spell(null,null,null,null);
		}

		public int cooldown { get { return spell.cooldown; } }
		public bool canCast { get { return spell.cooldown > -1 && spell.shape != null; } }

		protected Spell spell;

		public static Rectangle shapeBounds { get; private set; }
		public static Rectangle propBounds { get; private set; }
		public static Rectangle modTopBounds { get; private set; }
		public static Rectangle modBottomBounds { get; private set; }

		static SpellContainer()
		{
			shapeBounds = new Rectangle(99, 18, Skill.buttonSize, Skill.buttonSize);
			propBounds = new Rectangle(114, 18, Skill.buttonSize, Skill.buttonSize);
			modTopBounds = new Rectangle(128, 11, Skill.buttonSize, Skill.buttonSize);
			modBottomBounds = new Rectangle(128, 25, Skill.buttonSize, Skill.buttonSize);
		}

		public Projectile createProjectile(Vector2 pos, Vector2 dir, GameHandler game)
		{
			return spell.shape.generateProjectiles(pos, dir, spell, game);
		}

		public void onPlaceClick(float mouseX, float mouseY, ref Skill curHeld)
		{
			if (shapeBounds.Contains(mouseX, mouseY) && curHeld is SkillShape)
			{
				spell = new Spell((SkillShape)curHeld, spell.prop, spell.modTop, spell.modBottom);
			}
			else if (propBounds.Contains(mouseX, mouseY) && curHeld is SkillProp)
			{
				spell = new Spell(spell.shape, (SkillProp)curHeld, spell.modTop, spell.modBottom);
			}
			else if (modTopBounds.Contains(mouseX, mouseY) && curHeld is SkillProp)
			{
				spell = new Spell(spell.shape, spell.prop, (SkillProp)curHeld, spell.modBottom);
			}
			else if (modBottomBounds.Contains(mouseX, mouseY) && curHeld is SkillProp)
			{
				spell = new Spell(spell.shape, spell.prop, spell.modTop, (SkillProp)curHeld);
			}
			else
			{
				return;
			}
			curHeld = null;
		}

		public bool onClearClick(float mouseX, float mouseY)
		{
			if (shapeBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(null, spell.prop, spell.modTop, spell.modBottom);
			}
			else if (propBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, null, spell.modTop, spell.modBottom);
			}
			else if (modTopBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, spell.prop, null, spell.modBottom);
			}
			else if (modBottomBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, spell.prop, spell.modTop, null);
			}
			else
			{
				return false; //Return false if nothing was changed
			}
			return true; //Return true if something was changed
		}

		public void DrawButtons(float mouseX, float mouseY)
		{
			if (spell.shape != null)
			{
				spell.shape.DrawHightlight(shapeBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(shapeBounds, mouseX, mouseY);
			}
			if (spell.prop != null)
			{
				spell.prop.DrawHightlight(propBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(propBounds, mouseX, mouseY);
			}
			if (spell.modTop != null)
			{
				spell.modTop.DrawHightlight(modTopBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(modTopBounds, mouseX, mouseY);
			}
			if (spell.modBottom != null)
			{
				spell.modBottom.DrawHightlight(modBottomBounds, mouseX, mouseY);
			}
			else
			{
				EmptySkill.DrawHightlight(modBottomBounds, mouseX, mouseY);
			}
		}
	}
}
