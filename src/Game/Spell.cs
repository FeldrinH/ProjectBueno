using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using System;

namespace ProjectBueno.Game.Spells
{
	public class Spell //Data class
	{
		public Spell(SkillShape shape, SkillMod modMid, SkillMod modTop, SkillMod modBottom)
		{
			this.shape = shape;
			this.modElement = modMid;
			this.modTop = modTop;
			this.modBottom = modBottom;

			if (shape == null || modMid == null || modTop == null || modBottom == null)
			{
				cooldown = -1;
			}
			else
			{
				if (ContainsID("PropArcing"))
				{
					arcCount = shape.arcCount;
				}
				else
				{
					arcCount = 0;
				}

				cooldown = shape.modCooldown(modMid.modCooldown(modTop.modCooldown(modBottom.modCooldown(0))));

				Console.WriteLine(this);
			}
		}

		public override string ToString()
		{
			return cooldown < 0 ? "Uncastable spell" : string.Format("Cooldown: {0}s\nDamage: {1}\nHeal: {2}\nControl: {3}",cooldown/60.0f,getDamage(null),getHeal(null),getControl(null));
		}

		public bool Contains(Skill test)
		{
			return (test != null) && (shape == test || modElement == test || modTop == test || modBottom == test);
		}

		public bool ContainsID(string id)
		{
			return shape?.id == id || modElement?.id == id || modTop?.id == id || modBottom?.id == id;
		}

		public float getDamage(Entity target)
		{
			return (modElement.damageAdd + modTop.damageAdd + modBottom.damageAdd) * shape.potencyMult;
		}

		public float getHeal(Player caster)
		{
			return (modElement.healAdd + modTop.healAdd + modBottom.healAdd) * shape.potencyMult;
		}

		public int getControl(Entity target)
		{
			return (modElement.controlAdd + modTop.controlAdd + modBottom.controlAdd);
		}

		public void doEffect() //Some arguments or stuff???
		{
			#warning Might be needed?
		}

		public readonly SkillShape shape;
		public readonly SkillMod modElement;
		public readonly SkillMod modTop;
		public readonly SkillMod modBottom;
		public readonly int cooldown;
		public readonly int arcCount;
	}

	public class SpellContainer //Wrapper for player spells
	{
		public SpellContainer(Player player)
		{
			this.player = player;
			spell = new Spell(null,null,null,null);
			arrowTexture = Main.content.Load<Texture2D>("Arrows");
		}

		public Player player;

		public int cooldown { get { return spell.cooldown; } }
		public bool canCast { get { return spell.cooldown > -1 && spell.shape != null; } }

		protected Spell spell;

		protected static Texture2D arrowTexture;

		protected static readonly Rectangle upArrowBounds;
		protected static readonly Rectangle downArrowBounds;
		protected static Rectangle arrowSource;
		protected static readonly Vector2 numPos;

		public static Rectangle shapeBounds { get; private set; }
		public static Rectangle modMidBounds { get; private set; }
		public static Rectangle modTopBounds { get; private set; }
		public static Rectangle modBottomBounds { get; private set; }

		static SpellContainer()
		{
			shapeBounds = new Rectangle((int)Main.Config["shape"]["x"], (int)Main.Config["shape"]["y"], Skill.buttonSize, Skill.buttonSize);
			modMidBounds = new Rectangle((int)Main.Config["modMidBounds"]["x"], (int)Main.Config["modMidBounds"]["y"], Skill.buttonSize, Skill.buttonSize);
			modTopBounds = new Rectangle((int)Main.Config["modTopBounds"]["x"], (int)Main.Config["modTopBounds"]["y"], Skill.buttonSize, Skill.buttonSize);
			modBottomBounds = new Rectangle((int)Main.Config["modBottomBounds"]["x"], (int)Main.Config["modBottomBounds"]["y"], Skill.buttonSize, Skill.buttonSize);

			arrowSource = new Rectangle(0, 0, (int)Main.Config["arrowSize"]["w"], (int)Main.Config["arrowSize"]["h"]);
			upArrowBounds = new Rectangle((int)Main.Config["upArrowBounds"]["x"], (int)Main.Config["upArrowBounds"]["y"], arrowSource.Width, arrowSource.Height);
			downArrowBounds = new Rectangle((int)Main.Config["downArrowBounds"]["x"], (int)Main.Config["downArrowBounds"]["y"], arrowSource.Width, arrowSource.Height);
			numPos = new Vector2((int)Main.Config["numPos"]["x"], (int)Main.Config["numPos"]["y"]);
		}

		public override string ToString()
		{
			return spell.ToString();
		}

		public Projectile createProjectile(Vector2 pos, GameHandler game, Entity target)
		{
			game.player.lastCast = spell;
			return spell.shape.generateProjectiles(pos, spell, game, target, game.player);
		}

		public bool onPlaceClick(float mouseX, float mouseY, ref Skill curHeld)
		{
			if (upArrowBounds.Contains(mouseX, mouseY))
			{
				if (player.selectedSpell > 0)
				{
					--player.selectedSpell;
				}
			}
			else if (downArrowBounds.Contains(mouseX, mouseY))
			{
				if (player.selectedSpell < 4)
				{
					++player.selectedSpell;
				}
			}
			else if (!spell.Contains(curHeld))
			{
				if ((curHeld is SkillShape || curHeld == null) && shapeBounds.Contains(mouseX, mouseY))
				{
					spell = new Spell((SkillShape)curHeld, spell.modElement, spell.modTop, spell.modBottom);
				}
				else if (curHeld is SkillMod || curHeld == null)
				{
					if (modMidBounds.Contains(mouseX, mouseY))
					{
						spell = new Spell(spell.shape, (SkillMod)curHeld, spell.modTop, spell.modBottom);
					}
					else if (modTopBounds.Contains(mouseX, mouseY))
					{
						spell = new Spell(spell.shape, spell.modElement, (SkillMod)curHeld, spell.modBottom);
					}
					else if (modBottomBounds.Contains(mouseX, mouseY))
					{
						spell = new Spell(spell.shape, spell.modElement, spell.modTop, (SkillMod)curHeld);
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
				curHeld = null;
				return true;
			}
			return false;
		}

		//Deprecated
		public bool onClearClick(float mouseX, float mouseY)
		{
			if (shapeBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(null, spell.modElement, spell.modTop, spell.modBottom);
			}
			else if (modMidBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, null, spell.modTop, spell.modBottom);
			}
			else if (modTopBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, spell.modElement, null, spell.modBottom);
			}
			else if (modBottomBounds.Contains(mouseX, mouseY))
			{
				spell = new Spell(spell.shape, spell.modElement, spell.modTop, null);
			}
			else
			{
				return false; //Return false if nothing was changed
			}
			return true; //Return true if something was changed
		}

		public Skill DrawButtons(float mouseX, float mouseY) //Returns a skill if the mouse was hovering over it
		{
			Skill returnSkill = null;
			if (spell.shape != null)
			{
				if (spell.shape.DrawHightlight(shapeBounds, mouseX, mouseY))
				{
					returnSkill = spell.shape;
				}
			}
			else
			{
				EmptySkill.DrawHightlight(shapeBounds, mouseX, mouseY);
			}
			if (spell.modElement != null)
			{
				if(spell.modElement.DrawHightlight(modMidBounds, mouseX, mouseY))
				{
					returnSkill = spell.modElement;
				}
			}
			else
			{
				EmptySkill.DrawHightlight(modMidBounds, mouseX, mouseY);
			}
			if (spell.modTop != null)
			{
				if(spell.modTop.DrawHightlight(modTopBounds, mouseX, mouseY))
				{
					returnSkill = spell.modTop;
				}
			}
			else
			{
				EmptySkill.DrawHightlight(modTopBounds, mouseX, mouseY);
			}
			if (spell.modBottom != null)
			{
				if(spell.modBottom.DrawHightlight(modBottomBounds, mouseX, mouseY))
				{
					returnSkill = spell.modBottom;
				}
			}
			else
			{
				EmptySkill.DrawHightlight(modBottomBounds, mouseX, mouseY);
			}

			if (player.selectedSpell > 0)
			{
				arrowSource.Y = 0;
				DrawArrow(upArrowBounds, mouseX, mouseY);
			}
			if (player.selectedSpell < 4)
			{
				arrowSource.Y = arrowSource.Height;
				DrawArrow(downArrowBounds, mouseX, mouseY);
			}
			Main.spriteBatch.DrawString(Main.retroFont, (player.selectedSpell + 1).ToString(), numPos, Color.Black);

			return returnSkill;
		}

		public void DrawArrow(Rectangle rect,float mouseX, float mouseY)
		{
			if (rect.Contains(mouseX, mouseY))
			{
				arrowSource.X = arrowSource.Width;
				Main.spriteBatch.Draw(arrowTexture, rect, arrowSource, Color.White);
				arrowSource.X = 0;
			}
			else
			{
				Main.spriteBatch.Draw(arrowTexture, rect, arrowSource, Color.White);
			}
		}
	}
}
