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
	class ProjectileBall : Projectile
	{
		public ProjectileBall(Spell spell, GameHandler game, Entity target, Entity owner, Vector2 pos, Vector2 direction, float speed) : base(spell, game, target, owner)
		{
			this.pos = pos;
			this.speed = speed;
			moveSpeed = direction * speed;
			lifetime = TIMEOUT;
			lastCollide = null;
		}

		protected Vector2 pos;
		protected float speed;
		protected Vector2 moveSpeed;
		protected Entity lastCollide;

		public override bool toRemove
		{
			get
			{
				return lifetime <= 0;
			}
		}

		public override void Draw()
		{
			Main.spriteBatch.Draw(projTexture.texture, pos, projTexture.getCurFrame(), Color.White);
		}

		public override void Update()
		{
			--lifetime;
			foreach (var entity in game.entities)
			{
				if (!entity.isAlly && entity != lastCollide && entity.checkCollision(pos, size))
				{
					entity.dealDamage(spell.getDamage(entity), Vector2.Normalize(moveSpeed) * 5f, spell.shape.dmgCooldown);
					entity.control += spell.getControl(entity);
					entity.updateState();

					if (arcCount <= 0)
					{
						lifetime = 0;
					}
					else
					{
						target = entity.GetClosest(game.entities.Where(ent => !ent.isAlly));
						moveSpeed = Vector2.Normalize(target.pos - pos) * speed;
					}
					arcCount -= 1;

					break;
				}
			}
			pos += moveSpeed;

			projTexture.incrementAnimation();
		}

		public override void DrawDebug()
		{
			Main.spriteBatch.Draw(Main.boxel, pos, new Rectangle(0, 0, (int)size.X, (int)size.Y), Color.Red * 0.5f);
		}
	}
}
