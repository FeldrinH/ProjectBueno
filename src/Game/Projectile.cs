using Microsoft.Xna.Framework;
using ProjectBueno.Game.Spells;

namespace ProjectBueno.Game.Entities
{
	public abstract class Projectile
	{
		public Projectile(Spell spell)
		{
			this.spell = spell;
		}

		public const int TIMEOUTLIFETIME = 450;

		public abstract bool toRemove { get; }

		public readonly Spell spell;

		public void dealDamage(Entity target, Vector2 dmgPos)
		{
		}
		public abstract void Update();
		public abstract void Draw();
	}
}