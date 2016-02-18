using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using ProjectBueno.Game.Spells;

namespace ProjectBueno.Game.Entities
{
	public abstract class Projectile
	{
		public Projectile(Spell spell, GameHandler game)
		{
			this.spell = spell;
			this.game = game;
		}

		public const int TIMEOUTLIFETIME = 450;

		public abstract bool toRemove { get; }

		protected Vector2 size;

		protected float damage;

		public readonly Spell spell;
		protected readonly GameHandler game;

		public void dealDamage(Entity target, Vector2 dmgPos)
		{
		}
		public abstract void Update();
		public abstract void Draw();
	}
}