using Microsoft.Xna.Framework;
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
		public ProjectileStream(Spell spell, GameHandler game, int lifetime) : base(spell, game, null)
		{
			projectiles = new List<Vector2>();
			this.lifetime = lifetime;
		}

		protected List<Vector2> projectiles;

		protected static int collisiontime;
		protected static int effecttime;

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
			for (int i = projectiles.Count - 1; i >= 0; i--)
			{
				//projPos[i] += projSpeed[i];
				foreach (var entity in game.entities)
				{
					if (entity.checkCollision(projectiles[i], size))
					{
						if (entity.canDamage)
						{
							Vector2 knockback = game.player.pos - entity.pos;
							knockback.Normalize();
							knockback *= 5.0f;
							entity.dealDamage(spell.getDamage(entity),knockback);
						}

						projectiles.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
}
