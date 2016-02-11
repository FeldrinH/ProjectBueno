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
			partCount = (int)skill["projCount"];
			radSquared = (float)skill["radius"] * (float)skill["radius"];
		}

		protected int partCount;
		protected float radSquared;
		private static Random random = new Random(); //For testing

		public override Projectile generateProjectiles(Vector2 pos, Vector2 dir, Spell spell)
		{
			ProjectileGroup projReturn = new ProjectileBurst(spell,900,pos,radSquared);
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
			partCount = (int)skill["projCount"];
			duration = (int)skill["duration"]; //For testing
			length = (float)skill["length"]; //For testing
		}

		protected int partCount;
		protected int duration;
		protected float length;
		private static Random random = new Random();

		public override Projectile generateProjectiles(Vector2 pos, Vector2 dir, Spell spell)
		{
			ProjectileGroup projReturn = new ProjectileGroup(spell,duration);
			for (int i = 0; i < partCount; i++)
			{
				projReturn.addProjectile(pos + dir * 5.0f + new Vector2((float)(random.NextDouble() * (2.0 + length * dir.X) - 1.0), (float)(random.NextDouble() * (2.0 + length * dir.Y) - 1.0)), Vector2.Zero );
			}
			return projReturn;
		}
	}
}