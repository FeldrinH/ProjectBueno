using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using System;

namespace ProjectBueno.Game.Spells
{
	//Base Skill for Shapes
	public abstract class SkillShape : Skill
	{
		protected SkillShape(JObject skill) : base(skill)
		{
			#warning To load
			potencyMult = 1.0f;
		}
		public AnimatedTexture projTexture;
		public float potencyMult;
		public abstract Projectile generateProjectiles(Vector2 pos, Spell spell, GameHandler game, Entity target);
	}

	//Concrete Shapes
	public class ShapeBall : SkillShape
	{
		public ShapeBall(JObject skill) : base(skill)
		{
			speed = (float)skill["projSpeed"];
		}

		public float speed { get; protected set; }

		public override Projectile generateProjectiles(Vector2 pos, Spell spell, GameHandler game, Entity target)
		{
			Vector2 dir;
			if (target != null)
			{
				dir = target.pos - pos;
			}
			else
			{
				dir = game.player.dir.Vector();//dir = game.posFromScreenPos(Main.newMouseState.Position.ToVector2()) - pos;
			}
			dir.Normalize();
			return new ProjectileBall(spell, game, target, pos, dir * speed);
		}
	}

	public class ShapeBurst : SkillShape
	{
		public ShapeBurst(JObject skill) : base(skill)
		{
			partCount = (int)skill["projCount"];
			radSquared = (float)skill["radius"] * (float)skill["radius"];
			cooldownMult = (int)skill["cooldownMult"];
		}

		protected int partCount;
		protected float radSquared;
		protected int cooldownMult;
		private static Random random = new Random(); //For testing

		public override int modCooldown(int cooldownIn)
		{
			return base.modCooldown(cooldownIn) * cooldownMult;
		}

		public override Projectile generateProjectiles(Vector2 pos, Spell spell, GameHandler game, Entity target)
		{
			ProjectileBurst projReturn = new ProjectileBurst(spell, game, target, pos, radSquared);
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
			duration = (int)skill["duration"];
			length = (float)skill["length"];
			cooldown = (int?)skill["cooldown"] ?? 0;
			effectRate = (int?)skill["effectRate"] ?? 1;
			effectTime = 0;
		}

		protected int partCount;
		protected int duration;
		protected int effectRate;
		protected int effectTime;
		protected float length;
		private static Random random = new Random();

		public override int modCooldown(int cooldownIn)
		{
			return cooldown;
		}

		public override Projectile generateProjectiles(Vector2 pos, Spell spell, GameHandler game, Entity target)
		{
			Vector2 dir;
			if (target != null)
			{
				dir = target.pos - pos;
			}
			else
			{
				dir = game.player.dir.Vector();//dir = game.posFromScreenPos(Main.newMouseState.Position.ToVector2()) - pos;
			}
			dir.Normalize();
			ProjectileStream projReturn = new ProjectileStream(spell, game, target, duration, effectTime == 0);
			for (int i = 0; i < partCount; i++)
			{
				projReturn.addProjectile(pos + dir * (float)random.NextDouble() * length);
			}

			if (++effectTime >= effectRate)
			{
				effectTime = 0;
			}

			return projReturn;
		}
	}
}