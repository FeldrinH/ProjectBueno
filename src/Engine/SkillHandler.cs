﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Game.Entities;
using ProjectBueno.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Engine
{
	public class SkillHandler : IHandler
	{
		public GameHandler game;

		protected Texture2D background;
		protected Player player;
		public float downscale { get; protected set; }
		protected Matrix sreenScale;

		public SkillHandler(GameHandler game, Player player)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("skillTree");
			this.player = player;
		}

		public void Draw()
		{
			Main.graphicsManager.GraphicsDevice.Clear(Color.Gray);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, sreenScale);
			Main.spriteBatch.Draw(background, Vector2.Zero, Color.White);
			foreach (Skill skillButton in player.skills)
			{
				skillButton.DrawButton(downscale);
            }
			Main.spriteBatch.End();
		}

		public void Update()
		{
			if (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released)
			{
				foreach (Skill skill in player.skills)
                {
					skill.onClick(downscale, player);
				}
            }
			if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
			{
				Main.handler = game;
			}
		}
        
		public void windowResize()
		{
			sreenScale = Matrix.CreateScale((float)Main.window.ClientBounds.Width / Main.xRatio);
			downscale = (float)Main.xRatio / Main.window.ClientBounds.Width ;
		}
	}
}
