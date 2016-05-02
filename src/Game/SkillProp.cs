using Newtonsoft.Json.Linq;
using ProjectBueno.Game.Entities;

namespace ProjectBueno.Game.Spells
{
	//Concrete Properties and Modifiers
	public class SkillProp : Skill
	{
		public SkillProp(JObject skill) : base(skill)
		{
			damageAdd = (float?)skill["damage"] ?? 0.0f;
			healAdd = (float?)skill["heal"] ?? 0.0f;
			controlAdd = (int?)skill["control"] ?? 0;
		}

		public float healAdd;
		public float damageAdd;
		public int controlAdd;
	}

	public class PropPrejudice : SkillProp
	{
		public PropPrejudice(JObject skill) : base(skill)
		{
			if (skill["robots"] != null)
			{
				damageRobots = (float?)skill["robots"]["damage"] ?? 0.0f;
				healRobots = (float?)skill["robots"]["heal"] ?? 0.0f;
				controlRobots = (int?)skill["robots"]["control"] ?? 0;
			}

			if (skill["living"] != null)
			{
				damageLiving = (float?)skill["living"]["damage"] ?? 0.0f;
				healLiving = (float?)skill["living"]["heal"] ?? 0.0f;
				controlLiving = (int?)skill["living"]["control"] ?? 0;
			}
		}

		public float healRobots;
		public float damageRobots;
		public int controlRobots;

		public float healLiving;
		public float damageLiving;
		public int controlLiving;
	}
}