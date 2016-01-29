using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
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
		public abstract void generateProjectiles(Vector2 pos, List<Projectile> projectiles);
	}

	//Concrete Shapes
	public class ShapeBall : SkillShape
	{
		public ShapeBall(JObject skill) : base(skill)
		{
		}

		public override void generateProjectiles(Vector2 pos, List<Projectile> projectiles)
		{
			//Add projectile
		}
	}
	public class ShapeBurst : SkillShape
	{
		public ShapeBurst(JObject skill) : base(skill)
		{
			partCount = (int)skill["partCount"];
		}

		protected static int partCount;

		public override void generateProjectiles(Vector2 pos, List<Projectile> projectiles)
		{
			for (int i = 0; i < partCount; i++)
			{
				//Add projectiles
			}
		}
	}
	public class ShapeStream : SkillShape
	{
		public ShapeStream(JObject skill) : base(skill)
		{
			partCount = (int)skill["partCount"];
		}

		protected static int partCount;

		public override void generateProjectiles(Vector2 pos, List<Projectile> projectiles)
		{
			for (int i = 0; i < partCount; i++)
			{
				//Add projectiles
			}
		}
	}
}