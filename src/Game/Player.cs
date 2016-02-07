using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Engine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ProjectBueno.Game.Spells;
using System.Reflection;

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
			size = new Vector2((float)stats["Width"],(float)stats["Height"]);

			state = (int)States.STANDING;
			dir = Dir.DOWN;

			knowledgePoints = int.MaxValue; //FOR TESTING

			loadSkills(JArray.Parse(File.ReadAllText("Content/Skills.json")),JObject.Parse(File.ReadAllText("Content/SkillTree.json")));
		}

		protected bool moveHorizontal;
		public List<Skill> skills { get; protected set; }
		public List<SpellContainer> spells { get; protected set; }

		public int selectedSpell;

		public int knowledgePoints;

		public override void Update()
		{
			if (Main.newKeyState.IsKeyDown(Keys.R) && Main.oldKeyState.IsKeyUp(Keys.R))
			{
				dealDamage(0.0f, Dir.LEFT.Vector()*5.0f);
			}

			Vector2 totalMove = new Vector2();
			if (Main.newKeyState.IsKeyDown(Keys.W))
			{
				totalMove.Y -= speed;
				if (Main.oldKeyState.IsKeyUp(Keys.W))
				{
					moveHorizontal = false;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.S))
			{
				totalMove.Y += speed;
				if(Main.oldKeyState.IsKeyUp(Keys.S))
				{
					moveHorizontal = false;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.A))
			{
				totalMove.X -= speed;
				if (Main.oldKeyState.IsKeyUp(Keys.A))
				{
					moveHorizontal = true;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.D))
			{
				totalMove.X += speed;
				if (Main.oldKeyState.IsKeyUp(Keys.D))
				{
					moveHorizontal = true;
				}
			}

			if (totalMove != Vector2.Zero )
			{
				state = (int)States.WALKING;
			}
			else
			{
				state = (int)States.STANDING;
			}

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

			if (totalMove.X > 0.0f)
			{
				dir = Dir.RIGHT;
			}
			else if (totalMove.X < 0.0f)
			{
				dir = Dir.LEFT;
			}
			else if (totalMove.Y > 0.0f)
			{
				dir = Dir.DOWN;
			}
			else if (totalMove.Y < 0.0f)
			{
				dir = Dir.UP;
			}
			pos += totalMove;

			
			if (Main.newKeyState.IsKeyDown(Keys.D1) && !Main.oldKeyState.IsKeyDown(Keys.D1))
			{
				game.projectiles.Add(spells[selectedSpell].createProjectile(pos,dir.Vector()));
			}
			base.Update();
		}
		public override void Draw()
		{
            curTexture.incrementAnimation();
			Main.spriteBatch.Draw(curTexture.texture, pos, curTexture.getCurFrame(), Color.White);
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
				skillMap.Add((string)skill["id"],(Skill)Type.GetType("ProjectBueno.Game.Spells."+(string)skill["class"]).GetConstructor(new Type[1] { typeof(JObject) }).Invoke(new object[1] { skill }));
			}
			
			bool lastUp = false;
			JProperty skillPointer = (JProperty)skillTree.First;
			while(true)
			{
				if (skillPointer.Value.HasValues && !lastUp)
				{
					skillPointer = (JProperty)skillPointer.Value.First;
					continue;
				}

				skillMap[skillPointer.Name].populateDeps((JObject)skillPointer.Value,skillMap);
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

			spells = new List<SpellContainer>() { new SpellContainer(), new SpellContainer(), new SpellContainer(), new SpellContainer(), new SpellContainer() };
			selectedSpell = 0;
		}
		public override void loadTextures(JObject animData)
		{
			foreach (States st in Enum.GetValues(typeof(States)))
			{
				JObject anim = (JObject)animData[st.ToString()];
				Texture2D loadedTex = Main.content.Load<Texture2D>((string)anim["Texture"]);
				int w = (int)anim["Width"];
				int h = (int)anim["Height"];
				foreach (Dir dr in Enum.GetValues(typeof(Dir)))
				{
					textures.Add(new AnimatedTexture(loadedTex,(int)anim[dr.ToString()]["Frames"],(float)anim[dr.ToString()]["Speed"],w,h,0,(int)dr*h));
				}
			}
        }
	}
}
