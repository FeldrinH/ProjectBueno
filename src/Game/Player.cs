using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Engine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using ProjectBueno.Game.Spells;

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

			loadSkills(JArray.Parse(File.ReadAllText("Content/Skills.json")),JObject.Parse(File.ReadAllText("Content/SkillTree.json")));
		}

		public bool moveHorizontal;
		public List<Skill> skills { get; protected set; }

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

			
			if (Main.newKeyState.IsKeyDown(Keys.V) && !Main.oldKeyState.IsKeyDown(Keys.V))
			{
				game.projectiles.Add(new Projectile(pos,dir.Vector()*2.0f));
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
			Dictionary<string, Skill> skillMap = new Dictionary<string, Skill>();
			foreach (var skill in skillList)
			{
				skillMap.Add((string)skill["id"],new Skill((JObject)skill));
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
			skills = skillMap.Values.ToList();
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
