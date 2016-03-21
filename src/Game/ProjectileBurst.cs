using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using ProjectBueno.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Game.Entities
{
	class ProjectileBurst : ProjectileGroup
	{
		public ProjectileBurst(Spell spell, GameHandler game, int lifetime, Vector2 origin, float radSquared) : base(spell, game, lifetime)
		{
			this.origin = origin;
			this.radSquared = radSquared;
		}

		protected float radSquared;
		protected Vector2 origin;

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
