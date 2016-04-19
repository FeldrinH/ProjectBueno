using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Engine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ProjectBueno.Game.Spells;
using ProjectBueno.Engine.World;

namespace ProjectBueno.Game.Entities
{
	public class Player : Entity
	{
		public enum States : int
		{
			STANDING,
			WALKING,
			SHOOTING
		}

		public Player(Vector2 pos, GameHandler game) : base(pos, game)
		{
			JObject data = JObject.Parse(File.ReadAllText("Content/Player.json"));
			JObject stats = (JObject)data["Stats"];

			//Stopwatch s1 = new Stopwatch();
			//s1.Start();
			loadTextures((JObject)data["Animations"]);
			//s1.Stop();
			//Console.WriteLine(s1.ElapsedMilliseconds);

			health = (float)stats["Health"];
			speed = (float)stats["Speed"];
			size = new Vector2((float)stats["Width"], (float)stats["Height"]);

			state = (int)States.STANDING;
			dir = Dir.DOWN;

			knowledgePoints = 10000; //FOR TESTING

			loadSkills(JArray.Parse(File.ReadAllText("Content/Skills.json")), JObject.Parse(File.ReadAllText("Content/SkillTree.json")));
		}

		protected bool moveHorizontal;
		public List<Skill> skills { get; protected set; }
		public List<SpellContainer> spells { get; protected set; }

		public Spell lastCast;

		public int selectedSpell;
		public int cooldown;

		public int knowledgePoints;

		public bool hasCastBurst;

		public Entity target;

		public override void Update()
		{
			if (cooldown > 0 && game.doUpdate)
			{
				cooldown--;
			}

			base.Update();

			float totalSpeed = Main.newKeyState.IsKeyDown(Keys.LeftShift) ? Terrain.BLOCK_SIZE * Terrain.TILESIZE : speed;
			Vector2 totalMove = new Vector2();
			if (Main.newKeyState.IsKeyDown(Keys.W))
			{
				totalMove.Y -= totalSpeed;
				if (Main.oldKeyState.IsKeyUp(Keys.W))
				{
					moveHorizontal = false;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.S))
			{
				totalMove.Y += totalSpeed;
				if (Main.oldKeyState.IsKeyUp(Keys.S))
				{
					moveHorizontal = false;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.A))
			{
				totalMove.X -= totalSpeed;
				if (Main.oldKeyState.IsKeyUp(Keys.A))
				{
					moveHorizontal = true;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.D))
			{
				totalMove.X += totalSpeed;
				if (Main.oldKeyState.IsKeyUp(Keys.D))
				{
					moveHorizontal = true;
				}
			}

			game.doUpdate = false;
			if (totalMove != Vector2.Zero)
			{
				state = (int)States.WALKING;

				if (totalMove.X != 0.0f && totalMove.Y != 0.0f)
				{
					if (moveHorizontal)
					{
						totalMove.Y = 0.0f;
					}
					else
					{
						totalMove.X = 0.0f;
					}
				}

				moveDir(totalMove);

				if (!game.terrain.isColliding(pos + totalMove, size) || Main.newKeyState.IsKeyDown(Keys.LeftShift))
				{
					pos += totalMove;
					game.doUpdate = true;
					hasCastBurst = false;
				}

			}
			else
			{
				state = (int)States.STANDING;
			}
			

			if (Main.newKeyState.IsKeyDown(Keys.D1)/* && !Main.oldKeyState.IsKeyDown(Keys.D1)*/)
			{
				if (spells[0].canCast && cooldown < 1)
				{
					game.addProjectile(spells[0].createProjectile(pos, game, target));
					cooldown = spells[0].cooldown;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.D2)/* && !Main.oldKeyState.IsKeyDown(Keys.D1)*/)
			{
				if (spells[1].canCast && cooldown < 1)
				{
					game.addProjectile(spells[1].createProjectile(pos, game, target));
					cooldown = spells[1].cooldown;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.D3)/* && !Main.oldKeyState.IsKeyDown(Keys.D1)*/)
			{
				if (spells[2].canCast && cooldown < 1)
				{
					game.addProjectile(spells[2].createProjectile(pos, game, target));
					cooldown = spells[2].cooldown;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.D4)/* && !Main.oldKeyState.IsKeyDown(Keys.D1)*/)
			{
				if (spells[3].canCast && cooldown < 1)
				{
					game.addProjectile(spells[3].createProjectile(pos, game, target));
					cooldown = spells[3].cooldown;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.D5)/* && !Main.oldKeyState.IsKeyDown(Keys.D1)*/)
			{
				if (spells[4].canCast && cooldown < 1)
				{
					game.addProjectile(spells[4].createProjectile(pos, game, target));
					cooldown = spells[4].cooldown;
				}
			}
		}

		public void loadSkills(JArray skillList, JObject skillTree)
		{
			#region AlternativeReflection
			/*for (int i = 0; i < 100000; i++)
			{
				skillMap.Add((string)skill["id"],(Skill)Activator.CreateInstance(null, "ProjectBueno.Game.Spells." + skill["class"],false,0,null,new object[1] { (JObject)skill },null,null).Unwrap());
			}*/
			#endregion

			Dictionary<string, Skill> skillMap = new Dictionary<string, Skill>();

			foreach (JObject skill in skillList)
			{
				skillMap.Add((string)skill["id"], (Skill)Type.GetType("ProjectBueno.Game.Spells." + (string)skill["class"]).GetConstructor(new Type[1] { typeof(JObject) }).Invoke(new object[1] { skill }));
			}

			bool lastUp = false;
			JProperty skillPointer = (JProperty)skillTree.First;
			while (true)
			{
				if (skillPointer.Value.HasValues && !lastUp)
				{
					skillPointer = (JProperty)skillPointer.Value.First;
					continue;
				}

				skillMap[skillPointer.Name].populateDeps((JObject)skillPointer.Value, skillMap);
				//Console.WriteLine(skillPointer.Path);

				lastUp = false;
				if (skillPointer.Next != null)
				{
					skillPointer = (JProperty)skillPointer.Next;
				}
				else if (skillPointer.Parent != skillPointer.Root)
				{
					skillPointer = (JProperty)skillPointer.Parent.Parent;
					lastUp = true;
				}
				else
				{
					break;
				}
			}
			skillPointer = (JProperty)skillTree.First;
			while (skillPointer != null)
			{
				skillMap[skillPointer.Name].locked = false;
				if (skillMap[skillPointer.Name].bought)
				{
					skillMap[skillPointer.Name].unlockDeps();
				}
				skillPointer = (JProperty)skillPointer.Next;
			}

			skills = skillMap.Values.ToList();

			spells = new List<SpellContainer>() { new SpellContainer(this), new SpellContainer(this), new SpellContainer(this), new SpellContainer(this), new SpellContainer(this) };
			selectedSpell = 0;
		}

		public override void loadTextures(JObject animData)
		{
			foreach (States st in Enum.GetValues(typeof(States)))
			{
				loadTexture((JObject)animData[st.ToString()]);
			}
		}

		public override void onTargetCollide(Entity target)
		{
			Console.WriteLine("Whoa! Multiplayer magically implemented itself. Or this is a serious error.");
		}
	}
}
