﻿using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using ProjectBueno.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Game.Spells
{
	class ProjectileBurst : Projectile
	{
		public ProjectileBurst(Spell spell, GameHandler game, Entity target, Entity owner, Vector2 origin, float radSquared) : base(spell, game, target, owner) //Add target
		{
			this.origin = origin;
			this.radSquared = radSquared;
			lifetime = TIMEOUT;
			projPos = new List<Vector2>();
			projSpeed = new List<Vector2>();
		}

		protected List<Vector2> projPos;
		protected List<Vector2> projSpeed;

		protected Vector2 origin;
		protected float radSquared;

		public override bool toRemove
		{
			get
			{
				return lifetime <= 0 || !projPos.Any();
			}
		}

		public void addProjectile(Vector2 pos, Vector2 speed)
		{
			projPos.Add(pos);
			projSpeed.Add(speed);
		}

		public override void Draw()
		{
			Rectangle frameCache = projTexture.getCurFrame();
			for (int i = 0; i < projPos.Count; i++)
			{
				Main.spriteBatch.Draw(projTexture.texture, projPos[i], frameCache, Color.White * 0.5f);
			}
		}

		public override void Update()
		{
			--lifetime;
			for (int i = projPos.Count - 1; i >= 0; i--)
			{
				if ((origin - projPos[i]).LengthSquared() > radSquared)
				{
					projPos.RemoveAt(i);
					projSpeed.RemoveAt(i);
					continue;
				}
				projPos[i] += projSpeed[i];
				foreach (var entity in game.entities)
				{
					if (!entity.isAlly && entity.checkCollision(projPos[i], size))
					{
						if (entity.canDamage)
						{
							entity.dealDamage(spell.getDamage(entity), Vector2.Normalize(projSpeed[i]) * 5f, spell.shape.dmgCooldown);
							entity.control += spell.getControl(entity);
							entity.updateState();
						}

						projPos.RemoveAt(i);
						projSpeed.RemoveAt(i);
						break;
					}
				}
			}

			projTexture.incrementAnimation();
		}

		public override void DrawDebug()
		{
			for (int i = 0; i < projPos.Count; i++)
			{
				Main.spriteBatch.Draw(Main.boxel, projPos[i], new Rectangle(0, 0, (int)size.X, (int)size.Y), Color.Red * 0.5f);
			}
		}
	}
}
