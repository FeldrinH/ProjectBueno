using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
using ProjectBueno.Game.Spells;

namespace ProjectBueno.Game.Entities
{
	public class ProjectileSingle : Projectile
	{
		public ProjectileSingle(Vector2 pos,Vector2 speed, Spell spell, GameHandler game) : base(spell, game)
		{
			this.pos = pos;
			this.speed = speed;
			this.health = TIMEOUTLIFETIME;
			size = new Vector2(4.0f, 4.0f); //To load
			projTexture = new AnimatedTexture(Main.content.Load<Texture2D>("flyingProj"), 3, 0.25f, 4, 4);
		}

		public AnimatedTexture projTexture;
		private Vector2 speed;
		public Vector2 pos;
		public int health;
		public override bool toRemove
		{
			get
			{
				return health <= 0;
			}
		}

		public override void Update()
		{
			foreach (var entity in game.entities)
			{
				if (entity.checkCollision(pos, size))
				{
					Vector2 pushback = speed;
					pushback.Normalize();
					pushback *= 5.0f; //To load
					entity.dealDamage(spell.getDamage(entity), pushback);
					health = 0;
				}
			}
			pos += speed;
			--health;
		}
		public override void Draw()
		{
			projTexture.incrementAnimation();
			Main.spriteBatch.Draw(projTexture.texture, pos, projTexture.getCurFrame(), Color.White);
		}
	}
}
