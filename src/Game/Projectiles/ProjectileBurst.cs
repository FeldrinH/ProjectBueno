using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using ProjectBueno.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Game.Spells
{
	class ProjectileBurst : Projectile
	{
		public ProjectileBurst(Spell spell, GameHandler game, Vector2 origin, float radSquared) : base(spell, game, null) //Add target
		{
			this.origin = origin;
			this.radSquared = radSquared;
			lifetime = TIMEOUT;
			projectiles = new List<Proj>();
		}

		protected List<Proj> projectiles;

		protected Vector2 origin;
		protected float radSquared;

		public override bool toRemove
		{
			get
			{
				return lifetime <= 0;
			}
		}

		public void addProjectile(Vector2 pos, Vector2 speed)
		{
		}

		public override void Draw()
		{
		}

		public override void Update()
		{
		}
	}
}
