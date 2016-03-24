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
		public ProjectileBall(Spell spell, GameHandler game, Entity target, Vector2 pos, Vector2 speed) : base(spell, game, target)
		{
			this.pos = pos;
			this.speed = speed;
			lifetime = TIMEOUT;
		}

		protected Vector2 pos;
		protected Vector2 speed;

		public override bool toRemove
		{
			get
			{
				return lifetime <= 0;
			}
		}

		public override void Draw()
		{
			projTexture.incrementAnimation();
			Main.spriteBatch.Draw(projTexture.texture, pos, projTexture.getCurFrame(), Color.White);
		}

		public override void Update()
		{
			--lifetime;
			foreach (var entity in game.entities)
			{
				if (entity.checkCollision(pos, size))
				{
					Vector2 pushback = speed;
					pushback.Normalize();
					pushback *= 5.0f; //To load
					entity.dealDamage(spell.getDamage(entity), pushback);
					lifetime = 0;
					break;
				}
			}
			pos += speed;
		}
	}
}
