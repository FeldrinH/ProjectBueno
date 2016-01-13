using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Engine
{
	public class PauseHandler : IHandler
	{
		GameHandler game;
		protected Texture2D background;
		protected Color backColor;

		public PauseHandler(GameHandler game)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("pauseScreen");
			backColor = new Color(0, 0, 0);
		}

		public void Draw()
		{
			Main.graphicsManager.GraphicsDevice.Clear(backColor);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, null);
			Main.spriteBatch.Draw(background, Main.window.ClientBounds, Color.White);
			Main.spriteBatch.End();
		}

		public void Update()
		{
			if (Main.newKeyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P))
			{
				Main.handler = game;
			}
		}

		public void windowResize()
		{
		}
	}
}
