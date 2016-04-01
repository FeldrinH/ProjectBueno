﻿using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Game.Spells
{
	class ProjectileStream : Projectile
	{
		public ProjectileStream(Spell spell, GameHandler game, Entity target, int lifetime, bool dealDamage) : base(spell, game, target)
		{
			projectiles = new List<Vector2>();
			this.lifetime = lifetime;
			this.dealDamage = dealDamage;
		}

		protected List<Vector2> projectiles;
		protected bool dealDamage;

		public override bool toRemove
		{
			get
			{
				return lifetime <= 0;
			}
		}

		public void addProjectile(Vector2 pos)
		{
			projectiles.Add(pos);
		}

		public override void Draw()
		{
			projTexture.incrementAnimation();
			Rectangle frameCache = projTexture.getCurFrame();
			for (int i = 0; i < projectiles.Count; i++)
			{
				Main.spriteBatch.Draw(projTexture.texture, projectiles[i], frameCache, Color.White * 0.5f);
			}
		}

		public override void Update()
		{
			--lifetime;
			if (dealDamage)
			{
				for (int i = projectiles.Count - 1; i >= 0; i--)
				{
					//projPos[i] += projSpeed[i];
					foreach (var entity in game.entities)
					{
						if (entity.checkCollision(projectiles[i], size))
						{
							if (entity.canDamage)
							{
								Vector2 knockback = entity.pos - game.player.pos;
								knockback.Normalize();
								knockback *= 5.0f;
								entity.dealDamage(spell.getDamage(entity), knockback);
							}

							projectiles.RemoveAt(i);
							break;
						}
					}
				}
			}
		}
	}
}