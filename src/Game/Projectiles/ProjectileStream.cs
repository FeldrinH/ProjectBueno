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
		public ProjectileStream(Spell spell, GameHandler game, Entity target, Entity owner) : base(spell, game, target, owner)
		{
			projectiles = new List<Vector2>();
			this.hasUpdated = false;
		}

		protected const int COLLISION_COUNT = 128;
		protected const float COLLISION_INTERVAL = 1f / COLLISION_COUNT;

		protected List<Vector2> projectiles;
		protected bool hasUpdated;
		protected Entity colTarget;
		protected Vector2 colPos;

		public override bool toRemove
		{
			get
			{
				return hasUpdated;
			}
		}

		public void addProjectile(Vector2 pos)
		{
			projectiles.Add(pos);
		}

		public float ProcessCollision(Vector2 origin, Vector2 dir)
		{
			float mult = 0.0f;

			for (int i = 0; i <= COLLISION_COUNT; i++)
			{
				foreach (var entity in game.entities)
				{
					if (!entity.isAlly && entity != owner && entity.checkCollision(origin + dir * mult, size))
					{
						colTarget = entity;
						colPos = origin + dir * (mult + COLLISION_INTERVAL);

						return mult;
					}
				}

				mult += COLLISION_INTERVAL;
			}

			return 2.0f;
		}

		public override void Draw()
		{
			Rectangle frameCache = projTexture.getCurFrame();
			for (int i = 0; i < projectiles.Count; i++)
			{
				Main.spriteBatch.Draw(projTexture.texture, projectiles[i], frameCache, Color.White * 0.5f);
			}
		}

		public override void Update()
		{
			hasUpdated = true;

			if (colTarget != null)
			{
				if (colTarget.canDamage)
				{
					Vector2 knockback = colTarget.pos - owner.pos;
					knockback.Normalize();
					knockback *= 5.0f;
					colTarget.dealDamage(spell.getDamage(colTarget), knockback, spell.shape.dmgCooldown);
					colTarget.control += spell.getControl(colTarget);
					colTarget.updateState();
				}

				if (arcCount > 0)
				{
					Entity arcTarget = colTarget.GetClosest(game.entities.Where(ent => !ent.isAlly && ent != colTarget));
					if (arcTarget != null)
					{
						Projectile proj = spell.shape.generateProjectiles(colPos, spell, game, arcTarget, colTarget);
						proj.arcCount = arcCount - 1;
						game.addProjectile(proj);
					}
				}
			}

			projTexture.incrementAnimation();
		}

		public override void DrawDebug()
		{
			for (int i = 0; i < projectiles.Count; i++)
			{
				Main.spriteBatch.Draw(Main.boxel, projectiles[i], new Rectangle(0, 0, (int)size.X, (int)size.Y), Color.Red * 0.5f);
			}
		}
	}
}
