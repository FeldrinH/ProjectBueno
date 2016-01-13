using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;

namespace ProjectBueno.Game.Entities
{
    public class Projectile
    {
		public Projectile(Vector2 pos,Vector2 speed)
		{
			this.pos = pos;
			this.speed = speed;
			this.health = 10;
			texture = new AnimatedTexture(Main.content.Load<Texture2D>("flyingProj"), 3, 0.5f, 4, 4);
		}

		protected Vector2 speed;
		public Vector2 pos;
		public int health;
		public AnimatedTexture texture;

		public virtual void Update()
		{
			pos += speed;
		}
        public virtual void Draw()
		{
			texture.incrementAnimation();
			Main.spriteBatch.Draw(texture.texture, pos, texture.getCurFrame(), Color.White);
		}
	}
}
