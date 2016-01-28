using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using ProjectBueno.Game.Entities;
using System.Collections.Generic;

namespace ProjectBueno.Game.Spells
{
	//Skill for shapes
	public class SkillShape : Skill
	{
		public SkillShape(JObject skill) : base(skill)
		{
			projCount = (int)skill["projCount"];
		}

		protected static int projCount;

		public virtual void generateProjectiles(Vector2 pos, List<Projectile> projectiles)
		{
			for (int i = 0; i < projCount; i++)
			{
				//Add projectiles
			}
		}
	}
}