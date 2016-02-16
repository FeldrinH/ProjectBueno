using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
using ProjectBueno.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Game.Entities
{
	class ProjectileGroup : Projectile
	{
		public ProjectileGroup(Spell spell, int lifetime) : base(spell)
		{
			projPos = new List<Vector2>();
			projSpeed = new List<Vector2>();
			this.lifetime = lifetime;
			projTexture = new AnimatedTexture(Main.content.Load<Texture2D>("flyingProj"), 3, 0.5f, 4, 4);
		}

		protected List<Vector2> projPos;
		protected List<Vector2> projSpeed;
		protected AnimatedTexture projTexture;
		protected int lifetime;
		private static Random random = new Random(); //Random for testing with random removal

		public void addProjectile(Vector2 pos, Vector2 speed)
		{
			projPos.Add(pos);
			projSpeed.Add(speed);
		}

		public override bool toRemove
		{
			get
			{
				return !projPos.Any() || lifetime <= 0;
			}
		}

		public override void Draw()
		{
			projTexture.incrementAnimation();
			Rectangle frameCache = projTexture.getCurFrame();
            for (int i = 0; i < projPos.Count; i++)
			{
				Main.spriteBatch.Draw(projTexture.texture, projPos[i], frameCache, Color.White*0.5f);
			}
		}

		public override void Update()
		{
			--lifetime;
			for (int i = projPos.Count-1; i >= 0; i--)
			{
				projPos[i] += projSpeed[i];
			}
		}
	}

	class ProjectileBurst : ProjectileGroup
	{
		public ProjectileBurst(Spell spell, int lifetime, Vector2 origin, float radSquared) : base(spell,lifetime)
		{
			this.origin = origin;
			this.radSquared = radSquared;
		}

		protected float radSquared;
		protected Vector2 origin;

		public override void Update()
		{
			base.Update();
			for (int i = projPos.Count-1; i >= 0; i--)
			{
				if ((origin - projPos[i]).LengthSquared() > radSquared)
				{
					projPos.RemoveAt(i);
					projSpeed.RemoveAt(i);
				}
			}
		}
	}
}
