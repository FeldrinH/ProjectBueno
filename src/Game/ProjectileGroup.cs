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
		public ProjectileGroup(Spell spell, GameHandler game, int lifetime) : base(spell,game)
		{
			projPos = new List<Vector2>();
			projSpeed = new List<Vector2>();
			this.lifetime = lifetime;
			size = new Vector2(4.0f, 4.0f); //To load
			damage = 0.0f; //To load
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
			bool removeFlag;
            for (int i = projPos.Count-1; i >= 0; i--)
			{
				removeFlag = false;
				foreach (var entity in game.entities)
				{
					if (entity.checkCollision(projPos[i], size))
					{
						Vector2 pushback = projPos[i] - game.player.pos;
						pushback.Normalize();
						pushback *= 5.0f; //To load
						entity.dealDamage(damage, pushback);
						projPos.RemoveAt(i);
						projSpeed.RemoveAt(i);
						removeFlag = true;
					}
				}
				if (!removeFlag)
				{
					projPos[i] += projSpeed[i];
				}
			}
		}
	}

	class ProjectileBurst : ProjectileGroup
	{
		public ProjectileBurst(Spell spell, GameHandler game, int lifetime, Vector2 origin, float radSquared) : base(spell,game,lifetime)
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
