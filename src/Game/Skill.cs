using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using ProjectBueno.Game.Entities;
using System;
using System.Collections.Generic;

namespace ProjectBueno.Game.Spells
{
	public abstract class Skill
	{
		protected Skill(JObject skill)
		{
			buttonBounds = new Rectangle((int)skill["x"], (int)skill["y"], buttonSize, buttonSize);
			textureSource = new Rectangle(0, (int)skill["textureNum"] * buttonSize, buttonSize, buttonSize);
			texture = Main.content.Load<Texture2D>("Skills");

			name = (string)skill["name"];
			description = (string)skill["desc"];
			cost = (int)skill["cost"];
			locked = true;

			if ((bool?)skill["bought"] == true)
			{
				bought = true;
				locked = false;
			}
			else
			{
				bought = false;
			}

			dependants = new List<Skill>();
		}

		protected const int buttonSize = 10;
		public Rectangle buttonBounds { get; protected set; }
		protected Rectangle textureSource;
		protected Texture2D texture;

		protected List<Skill> dependants;
		public bool locked;
		public bool bought;
		protected string name;
		protected string description;
		protected int cost;

		protected static readonly Color forsaleColor = Color.Gray;
		protected static readonly Color lockedColor = new Color(25, 25, 25);

		public void unlockDeps()
		{
			foreach (var skill in dependants)
			{
				skill.locked = false;
			}
		}
		public void populateDeps(JObject deps, Dictionary<string, Skill> skills)
		{
			foreach (var dep in deps)
			{
				dependants.Add(skills[dep.Key]);
			}
		}

		public void onClick(float downscale, Player player)
		{
			if (buttonBounds.Contains(Main.newMouseState.X * downscale, Main.newMouseState.Y * downscale) && player.knowledgePoints >= cost)
			{
				player.knowledgePoints -= cost;
				bought = true;
				unlockDeps();
			}
		}

		//Button Specialized Draw
		public void DrawButton(float downscale)
		{
			if (locked)
			{
				Main.spriteBatch.Draw(texture, buttonBounds, textureSource, lockedColor);
			}
			else if (buttonBounds.Contains(Main.newMouseState.X * downscale, Main.newMouseState.Y * downscale))
			{
				textureSource.X = buttonSize;
				Main.spriteBatch.Draw(texture, buttonBounds, textureSource, bought ? Color.White : forsaleColor);
				textureSource.X = 0;
			}
			else
			{
				Main.spriteBatch.Draw(texture, buttonBounds, textureSource, bought ? Color.White : forsaleColor);
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

			if (posBounds.Contains(Main.newMouseState.X * downscale, Main.newMouseState.Y * downscale))
			{
				textureSource.X = buttonSize;
				Main.spriteBatch.Draw(texture, posBounds, textureSource, Color.White);
				textureSource.X = 0;
			}
			else
			{
				Main.spriteBatch.Draw(texture, posBounds, textureSource, Color.White);
			}
		}
	}
}
