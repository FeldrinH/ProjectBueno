using Microsoft.Xna.Framework;
using ProjectBueno.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Game.Spells
{
	class ProjectileBall : Projectile
	{
		public ProjectileBall(Spell spell, GameHandler game, Vector2 pos, Vector2 speed) : base(spell, game, null)
		{
			this.pos = pos;
			this.speed = speed;
			lifetime = TIMEOUT;
		}

		protected Vector2 pos;
		protected Vector2 speed;

		public override bool toRemove
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void Draw()
		{
			throw new NotImplementedException();
		}

		public override void Update()
		{
			throw new NotImplementedException();
		}
	}
}
