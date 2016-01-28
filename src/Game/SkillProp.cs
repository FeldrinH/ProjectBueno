using Newtonsoft.Json.Linq;

namespace ProjectBueno.Game.Spells
{
	//Skill for properties and modifiers
	public class SkillProp : Skill
	{
		public SkillProp(JObject skill) : base(skill)
		{
		}
	}
}