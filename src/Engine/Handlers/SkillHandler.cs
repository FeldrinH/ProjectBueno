﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Game.Entities;
using ProjectBueno.Game.Spells;
using System;

namespace ProjectBueno.Engine
{
	public class SkillHandler : MenuHandler
	{
		public GameHandler game;

		protected Player player;

		public static readonly Vector2 namePos;
		public static readonly Vector2 descPos;
		public static readonly Vector2 costPos;
		public static readonly Vector2 kpPos;

		protected Skill curHeld;

		public SkillHandler(GameHandler game, Player player)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("skillTree");
			this.player = player;
		}

		static SkillHandler()
		{
			namePos = new Vector2((int)Main.Config["namePos"]["x"], (int)Main.Config["namePos"]["y"]);
			descPos = new Vector2((int)Main.Config["descPos"]["x"], (int)Main.Config["descPos"]["y"]);
			costPos = new Vector2((int)Main.Config["costPos"]["x"], (int)Main.Config["costPos"]["y"]);
			kpPos = new Vector2((int)Main.Config["kpPos"]["x"], (int)Main.Config["kpPos"]["y"]);
		}

		public override void Draw()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;
			Skill drawHeldText = curHeld;
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, Main.AliasedRasterizer, null, screenScale);
			Main.spriteBatch.Draw(background, Vector2.Zero, Color.White);
			foreach (Skill skillButton in player.skills)
			{
				if (skillButton.DrawButton(mouseX, mouseY))
				{
					drawHeldText = skillButton;
				}
			}
			drawHeldText = player.spells[player.selectedSpell].DrawButtons(mouseX, mouseY) ?? drawHeldText;
			if (drawHeldText != null)
			{
				Main.spriteBatch.DrawString(Main.retroFont, drawHeldText.name, namePos, Color.Purple);
				Main.spriteBatch.DrawString(Main.retroFont, drawHeldText.description, descPos, Color.Black);
				Main.spriteBatch.DrawString(Main.retroFont, drawHeldText.cost + "KP", costPos, Color.Green);
			}
			if (curHeld != null)
			{
				curHeld.Draw(new Vector2(mouseX-Skill.buttonSize*0.5f, mouseY-Skill.buttonSize*0.5f));
			}

			Main.spriteBatch.DrawString(Main.retroFont, player.knowledgePoints + "KP", kpPos, Color.Green);

			Main.spriteBatch.End();
		}

		public override void Update()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;
			bool clearHeld = true;

			if (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released)
			{
				foreach (Skill skill in player.skills)
				{
					if (skill.onClick(mouseX, mouseY, ref player.knowledgePoints, ref curHeld))
					{
						clearHeld = false;
					}
				}
				
				if (!player.spells[player.selectedSpell].onPlaceClick(mouseX, mouseY, ref curHeld) && clearHeld)
				{
					curHeld = null;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
			{
				Main.Handler = game;
			}
			if (Main.newKeyState.IsKeyDown(Keys.K) && !Main.oldKeyState.IsKeyDown(Keys.K))
			{
				game.player.knowledgePoints = 10000;
			}
		}
	}
}
