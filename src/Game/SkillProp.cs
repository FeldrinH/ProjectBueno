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

		public const string ARCINGID = "PropArcing"; //HARDCODED EXCEPTION.(TODO)
	}
}