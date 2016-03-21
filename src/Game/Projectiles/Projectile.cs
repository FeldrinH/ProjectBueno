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
			this.target = target;
		}

		public abstract void Draw();
		public abstract void Update();

		public abstract bool toRemove { get; }

		protected int lifetime;

		protected Spell spell;
		protected GameHandler game;
		protected Entity target;

		protected const int TIMEOUT = 450;
	}
}