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
		public ProjectileGroup(Spell spell) : base(spell)
		{
			projPos = new List<Vector2>();
			projSpeed = new List<Vector2>();
			projTexture = new AnimatedTexture(Main.content.Load<Texture2D>("flyingProj"), 3, 0.5f, 4, 4);
		}

		protected List<Vector2> projPos;
		protected List<Vector2> projSpeed;
		public AnimatedTexture projTexture;
		private Random random = new Random(); //Random for testing with random removal

		public void addProjectile(Vector2 pos, Vector2 speed)
		{
			projPos.Add(pos);
			projSpeed.Add(speed);
		}

		public override bool toRemove
		{
			get
			{
				return !projPos.Any();
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
			for (int i = projPos.Count-1; i >= 0; i--)
			{
				projPos[i] += projSpeed[i];
				if (random.Next(100) == 0)
				{
					projPos.RemoveAt(i);//Remove random projectile for performance testing
					projSpeed.RemoveAt(i);
				}
			}
		}
	}
}
