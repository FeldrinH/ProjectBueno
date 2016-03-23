using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using System;

namespace ProjectBueno.Game.Spells
{
	public class Spell //Data class
	{
		public Spell(SkillShape shape, SkillProp modMid, SkillProp modTop, SkillProp modBottom)
		{
			this.shape = shape;
			this.modMid = modMid;
			this.modTop = modTop;
			this.modBottom = modBottom;
			if (shape == null || modMid == null || modTop == null || modBottom == null)
			{
				cooldown = -1;
			}
			else
			{
				cooldown = shape.modCooldown(modMid.modCooldown(modTop.modCooldown(modBottom.modCooldown(0))));
				Console.WriteLine(cooldown);
			}
		}

		public bool Contains(Skill test)
		{
			return shape == test || modMid == test || modTop == test || modBottom == test;
		}

		public float getDamage(Entity target)
		{
			return (modMid.damageAdd + modTop.damageAdd + modBottom.damageAdd) * shape.potencyMult;
		}

		public float getHeal(Player caster)
		{
			return (modMid.healAdd + modTop.healAdd + modBottom.healAdd) * shape.potencyMult;
		}

		public int getControl(Entity target)
		{
			return (modMid.controlAdd + modTop.controlAdd + modBottom.controlAdd);
		}

		public void doEffect() //Some arguments or stuff???
		{
			#warning Might be needed?
		}

		public readonly SkillShape shape;
		public readonly SkillProp modMid;
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
			shapeBounds = new Rectangle(98, 18, Skill.buttonSize, Skill.buttonSize);
			propBounds = new Rectangle(113, 18, Skill.buttonSize, Skill.buttonSize);
			modTopBounds = new Rectangle(127, 12, Skill.buttonSize, Skill.buttonSize);
			modBottomBounds = new Rectangle(127, 24, Skill.buttonSize, Skill.buttonSize);
		}

		public Projectile createProjectile(Vector2 pos, Vector2 dir, GameHandler game)
		{
			return spell.shape.generateProjectiles(pos, dir, spell, game);
		}

		public void onPlaceClick(float mouseX, float mouseY, ref Skill curHeld)
		{
			if (!spell.Contains(curHeld))
			{
				if (curHeld is SkillShape && shapeBounds.Contains(mouseX, mouseY))
				{
					spell = new Spell((SkillShape)curHeld, spell.modMid, spell.modTop, spell.modBottom);
				}
				else if (curHeld is SkillProp)
				{
					if (propBounds.Contains(mouseX, mouseY))
					{
						spell = new Spell(spell.shape, (SkillProp)curHeld, spell.modTop, spell.modBottom);
					}
					else if (modTopBounds.Contains(mouseX, mouseY))
					{
						spell = new Spell(spell.shape, spell.modMid, (SkillProp)curHeld, spell.modBottom);
					}
					else if (modBottomBounds.Contains(mouseX, mouseY))
					{
						spell = new Spell(spell.shape, spell.modMid, spell.modTop, (SkillProp)curHeld);
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}
				curHeld = null;
			}
		}

		public bool onClearClick(float mouseX, float mouseY)
		{
			if (shapeBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(null, spell.modMid, spell.modTop, spell.modBottom);
			}
			else if (propBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, null, spell.modTop, spell.modBottom);
			}
			else if (modTopBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, spell.modMid, null, spell.modBottom);
			}
			else if (modBottomBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, spell.modMid, spell.modTop, null);
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
			if (spell.modMid != null)
			{
				spell.modMid.DrawHightlight(propBounds, mouseX, mouseY);
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
