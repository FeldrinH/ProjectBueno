using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;

namespace ProjectBueno.Game.Spells
{
	public abstract class Projectile
	{
		public Projectile(Spell spell,GameHandler game, Entity target)
		{
			this.spell = spell;
			this.game = game;
			this.target = target; //To implement
			size = new Vector2(4.0f, 4.0f); //To load
			projTexture = new AnimatedTexture(Main.content.Load<Texture2D>("flyingProj"), 3, 0.5f, 4, 4); //To load/implement
		}

		public abstract void Draw();
		public abstract void Update();
		public abstract void DrawDebug();

		public abstract bool toRemove { get; }

		protected AnimatedTexture projTexture;
		protected int lifetime;
		protected Vector2 size;

		protected Spell spell;
		protected GameHandler game;
		protected Entity target;

		protected const int TIMEOUT = 450;
	}
}