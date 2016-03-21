using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using ProjectBueno.Game.Spells;

namespace ProjectBueno.Game.Entities
{
	class ProjectileBurst : ProjectileGroup
	{
		public ProjectileBurst(Spell spell, GameHandler game, int lifetime, Vector2 origin, float radSquared) : base(spell, game, lifetime)
		{
			this.origin = origin;
			this.radSquared = radSquared;
			damagetime = 7;
			effecttime = 7;
		}

		protected float radSquared;
		protected Vector2 origin;
		protected static int damagetime;
		protected static int effecttime;

		public override void Update()
		{
			for (int i = projPos.Count - 1; i >= 0; i--)
			{
				projPos[i] += projSpeed[i];
				foreach (var entity in game.entities)
				{
					if (entity.checkCollision(projPos[i], size))
					{
						Vector2 pushback = projSpeed[i];
						pushback.Normalize();
						pushback *= 5.0f; //To load
						entity.dealDamage(spell.getDamage(entity), pushback);
						projPos.RemoveAt(i);
						projSpeed.RemoveAt(i);
						break;
					}
				}
				if ((origin - projPos[i]).LengthSquared() > radSquared)
				{
					projPos.RemoveAt(i);
					projSpeed.RemoveAt(i);
				}
			}
		}
	}
}
