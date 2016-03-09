using Newtonsoft.Json.Linq;
using ProjectBueno.Game.Entities;

namespace ProjectBueno.Game.Spells
{
	//Base Skill for Properties and Modifiers
	public /*abstract*/ class SkillProp : Skill
	{
		/*protected*/ public SkillProp(JObject skill) : base(skill)
		{
		}
		public /*abstract*/ bool isProperty { get; }
		public /*abstract*/ void setProperties() {}
		public /*abstract*/ void onContact(Entity target) {}
	}

	//Concrete Properties and Modifiers
}