using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
using ProjectBueno.Game.Spells;

namespace ProjectBueno.Game.Entities
{
    public class ProjectileSingle : Projectile
    {
		public ProjectileSingle(Vector2 pos,Vector2 speed, Spell spell) : base(spell)
		{
			this.pos = pos;
			this.speed = speed;
			this.health = 450;
			projTexture = new AnimatedTexture(Main.content.Load<Texture2D>("flyingProj"), 3, 0.5f, 4, 4);
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
