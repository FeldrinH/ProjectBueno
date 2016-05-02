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
			potencyMult = (float?)skill["potencyMult"] ?? 1.0f;
			arcCount = (int?)skill["ArcCount"] ?? 2;
			dmgCooldown = 15;
		}
		public AnimatedTexture projTexture;
		public float potencyMult;
		public int dmgCooldown;
		public int arcCount;
		public abstract Projectile generateProjectiles(Vector2 pos, Spell spell, GameHandler game, Entity target, Entity owner);
	}

	//Concrete Shapes
	public class ShapeBall : SkillShape
	{
		public ShapeBall(JObject skill) : base(skill)
		{
			speed = (float)skill["projSpeed"];
		}

		public float speed { get; protected set; }

		public override Projectile generateProjectiles(Vector2 pos, Spell spell, GameHandler game, Entity target, Entity owner)
		{
			Vector2 dir;
			if (target != null)
			{
				dir = target.pos - pos;
			}
			else
			{
				dir = game.posFromScreenPos(Main.newMouseState.Position.ToVector2()) - pos;//dir = game.player.dir.Vector();
			}
			dir.Normalize();
			return new ProjectileBall(spell, game, target, owner, pos, dir * speed);
		}
	}

	public class ShapeBurst : SkillShape
	{
		public ShapeBurst(JObject skill) : base(skill)
		{
			partCount = (int)skill["projCount"];
			radSquared = (float)skill["radius"] * (float)skill["radius"];
			arcRadSquared = radSquared * (float)skill["arcScale"] * (float)skill["arcScale"];
			cooldownMult = (int)skill["cooldownMult"];
			arcCount = 1;
		}

		protected int partCount;
		protected float radSquared;
		protected float arcRadSquared;
		protected int cooldownMult;
		private static Random random = new Random(); //For testing

		public override int modCooldown(int cooldownIn)
		{
			return base.modCooldown(cooldownIn) * cooldownMult;
		}

		public override Projectile generateProjectiles(Vector2 pos, Spell spell, GameHandler game, Entity target, Entity owner)
		{
			ProjectileBurst projReturn = new ProjectileBurst(spell, game, target, owner, pos, spell.arcCount == 0 ? radSquared : arcRadSquared);
			Vector2 vecSpeed;
			for (int i = 0; i < partCount; i++)
			{
				vecSpeed = new Vector2((float)(random.NextDouble() < 0.5 ? random.NextDouble() : random.NextDouble() * -1.0), (float)(random.NextDouble() < 0.5 ? random.NextDouble() : random.NextDouble() * -1.0));
				vecSpeed.Normalize();
				vecSpeed *= (float)(random.NextDouble() * 2.0 + 1.0);
				projReturn.addProjectile(pos, vecSpeed);
			}
			return projReturn;
		}
	}

	public class ShapeStream : SkillShape
	{
		public ShapeStream(JObject skill) : base(skill)
		{
			partCount = (int)skill["projCount"];
			length = (float)skill["length"];
			cooldown = (int?)skill["cooldown"] ?? 0;
		}

		protected int partCount;
		protected float length;
		private static Random random = new Random();

		public override int modCooldown(int cooldownIn)
		{
			return cooldown;
		}

		public override Projectile generateProjectiles(Vector2 origin, Spell spell, GameHandler game, Entity target, Entity owner)
		{
			Vector2 dir;
			if (target != null)
			{
				dir = target.pos - origin;
			}
			else
			{
				dir = game.posFromScreenPos(Main.newMouseState.Position.ToVector2()) - origin;//dir = game.player.dir.Vector();
			}
			dir.Normalize();

			ProjectileStream projReturn = new ProjectileStream(spell, game, target, owner);

			float maxMult = projReturn.ProcessCollision(origin, dir * length);

			float mult;
			for (int i = 0; i < partCount; i++)
			{
				mult = (float)random.NextDouble();
				if (mult < maxMult)
				{
					projReturn.addProjectile(origin + dir * length * mult);
				}
			}

			return projReturn;
		}
	}
}