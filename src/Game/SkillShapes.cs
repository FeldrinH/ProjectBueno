using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using System;
using System.Collections.Generic;

namespace ProjectBueno.Game.Spells
{
	//Base Skill for Shapes
	public abstract class SkillShape : Skill
	{
		protected SkillShape(JObject skill) : base(skill)
		{
		}
		public AnimatedTexture projTexture;
		public abstract Projectile generateProjectiles(Vector2 pos,Vector2 dir, Spell spell);
	}

	//Concrete Shapes
	public class ShapeBall : SkillShape
	{
		public ShapeBall(JObject skill) : base(skill)
		{
			speed = (float)skill["projSpeed"];
		}

		public float speed { get; protected set; }

		public override Projectile generateProjectiles(Vector2 pos, Vector2 dir, Spell spell)
		{
			return new ProjectileSingle(pos,dir*speed,spell);
		}
	}
	public class ShapeBurst : SkillShape
	{
		public ShapeBurst(JObject skill) : base(skill)
		{
			partCount = 5000;//For testing
		}

		protected static int partCount;
		private Random random = new Random(); //For testing

		public override Projectile generateProjectiles(Vector2 pos, Vector2 dir, Spell spell)
		{
			ProjectileGroup projReturn = new ProjectileGroup(spell);
			Vector2 vecSpeed;
			for (int i = 0; i < partCount; i++)
			{
				vecSpeed = new Vector2((float)(random.NextDouble() < 0.5 ? random.NextDouble() : random.NextDouble()*-1.0), (float)(random.NextDouble() < 0.5 ? random.NextDouble() : random.NextDouble() * -1.0));
				vecSpeed.Normalize();
				vecSpeed *= (float)(random.NextDouble() * 2.0 + 1.0);
                projReturn.addProjectile(pos,vecSpeed);
			}
			return projReturn;
		}
	}
	public class ShapeStream : SkillShape
	{
		public ShapeStream(JObject skill) : base(skill)
		{
			partCount = (int)skill["partCount"];
		}

		protected static int partCount;

		public override Projectile generateProjectiles(Vector2 pos, Vector2 dir, Spell spell)
		{
			return null; //Placeholder
			for (int i = 0; i < partCount; i++)
			{
				//Add projectiles
			}
		}
	}
}