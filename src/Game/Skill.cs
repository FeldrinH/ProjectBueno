using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using System;
using System.Collections.Generic;

namespace ProjectBueno.Game.Spells
{
	public class Skill
	{
		public Skill(JObject skill)
		{
			buttonBounds = new Rectangle((int)skill["x"], (int)skill["y"], buttonSize, buttonSize);
			textureSource = new Rectangle((int)skill["textureNum"] * buttonSize, 0, buttonSize, buttonSize);
			texture = Main.content.Load<Texture2D>("Skills");

			name = (string)skill["name"];
			description = (string)skill["desc"];
			cost = (int)skill["cost"];
			locked = true;

			if ((bool?)skill["bought"] == true)
			{ bought = true; }
			else
			{ bought = false; }

			dependants = new List<Skill>();
		}

		protected const int buttonSize = 16;
		public Rectangle buttonBounds { get; protected set; }
		protected Rectangle textureSource;
		protected Texture2D texture;

		protected List<Skill> dependants;
		protected bool locked;
		protected bool bought;
		protected string name;
		protected string description;
		protected int cost;

		public void unlockDeps()
		{
			foreach (var skill in dependants)
			{
				skill.locked = false;
			}
		}

		public void populateDeps(JObject deps, Dictionary<string,Skill> skills)
		{
			foreach (var dep in deps)
			{
				dependants.Add(skills[dep.Key]);
			}
		}

		//Button Specialized Draw
		public void DrawButton(float downscale)
		{
			Main.spriteBatch.Draw(texture, buttonBounds, textureSource, Color.White);
			if (buttonBounds.Contains(Main.newMouseState.X*downscale, Main.newMouseState.Y*downscale))
			{
				Main.spriteBatch.Draw(Main.boxel,buttonBounds,Color.White*0.5f);
			}
		}

		//General Draw
		public void Draw(Point pos)
		{
			Rectangle posBounds = buttonBounds;
			posBounds.X = pos.X;
			posBounds.Y = pos.Y;
			Main.spriteBatch.Draw(texture, posBounds, textureSource, Color.White);
		}
		public void DrawHightlight(Point pos, float downscale)
		{
			Rectangle posBounds = buttonBounds;
			posBounds.X = pos.X;
			posBounds.Y = pos.Y;
			Main.spriteBatch.Draw(texture, posBounds, textureSource, Color.White);
			if (posBounds.Contains(Main.newMouseState.X * downscale, Main.newMouseState.Y * downscale))
			{
				Main.spriteBatch.Draw(Main.boxel, posBounds, Color.White * 0.5f);
			}
		}
	}
}
