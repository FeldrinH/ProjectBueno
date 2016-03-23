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
		//Constructor for derived types
		protected Skill(JObject skill)
		{
			buttonBounds = new Rectangle((int)skill["x"], (int)skill["y"], buttonSize, buttonSize);
			textureSource = new Rectangle(0, (int)skill["textureNum"] * buttonSize, buttonSize, buttonSize);
			texture = Main.content.Load<Texture2D>("Skills");

			id = (string)skill["id"];

			name = (string)skill["name"];
			description = (string)skill["desc"];
			cost = (int)skill["cost"];

			cooldown = (int?)skill["cooldown"] ?? 0;

			if ((bool?)skill["bought"] == true)
			{
				locked = false;
				bought = true;
			}
			else
			{
				locked = true;
				bought = false;
			}

			dependants = new List<Skill>();
		}

		public const int buttonSize = 10;
		public Rectangle buttonBounds { get; protected set; }
		protected Rectangle textureSource;
		protected Texture2D texture;

		protected List<Skill> dependants;
		public bool locked;
		private bool _bought;
		public bool bought
		{
			get { return _bought; }
			set {
				if (value)
				{
					textureSource.X = buttonSize;
				}
				else
				{
					textureSource.X = 0;
				}
				_bought = value;
			}
		}
		public string name { get; protected set; }
		public string description { get; protected set; }
		public int cost { get; protected set; }
		protected int cooldown;
		public readonly string id;

		protected static readonly Color forsaleColor = Color.Gray;
		protected static readonly Color lockedColor = new Color(25, 25, 25);
		protected static readonly float forsaleHighlight = 0.2f;
		protected static readonly float boughtHighlight = 0.4f;

		public static Skill Empty { get; private set; }

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

		public virtual int modCooldown(int cooldownIn)
		{
			return (cooldownIn + cooldown);
		}

		public void onClick(float mouseX, float mouseY, ref int knowledgePoints, ref Skill curHeld)
		{
			if (buttonBounds.Contains(mouseX, mouseY))
			{
				if (bought)
				{
					curHeld = this;
				}
				else if (knowledgePoints >= cost && !locked)
				{
					knowledgePoints -= cost;
					bought = true;
					unlockDeps();
				}
			}
		}

		//Button Specialized Draw
		//Returns true if mouse is hovering over button
		public bool DrawButton(float mouseX,float mouseY)
		{
			if (locked)
			{
				Main.spriteBatch.Draw(texture, buttonBounds, textureSource, lockedColor);
			}
			else if (buttonBounds.Contains(mouseX, mouseY))
			{
				Main.spriteBatch.Draw(texture, buttonBounds, textureSource, (bought ? Color.White : forsaleColor));
				Main.spriteBatch.Draw(Main.boxel, buttonBounds, Color.White * (bought? boughtHighlight : forsaleHighlight));
				return true;
			}
			else
			{
				Main.spriteBatch.Draw(texture, buttonBounds, textureSource, (bought ? Color.White : forsaleColor));
			}
			return false;
		}

		//General Draw
		public void Draw(Vector2 pos)
		{
			Main.spriteBatch.Draw(texture, pos, textureSource, Color.White);
		}
		public void DrawHightlight(Rectangle rect, float mouseX, float mouseY)
		{
			Main.spriteBatch.Draw(texture, rect, textureSource, Color.White);
			if (rect.Contains(mouseX, mouseY))
			{
				Main.spriteBatch.Draw(Main.boxel, rect, Color.White * (bought ? boughtHighlight : forsaleHighlight));
			}
		}
	}

	public static class EmptySkill
	{
		private static Rectangle textureSource;
		private static Texture2D texture;

		public static void initEmpty()
		{
			textureSource = new Rectangle(0, 0, Skill.buttonSize, Skill.buttonSize);
			texture = Main.content.Load<Texture2D>("Skills");
		}

		public static void DrawHightlight(Rectangle rect, float mouseX, float mouseY)
		{
			if (rect.Contains(mouseX, mouseY))
			{
				textureSource.X = Skill.buttonSize;
				Main.spriteBatch.Draw(texture, rect, textureSource, Color.White);
				textureSource.X = 0;
			}
			else
			{
				Main.spriteBatch.Draw(texture, rect, textureSource, Color.White);
			}
		}
	}
}
