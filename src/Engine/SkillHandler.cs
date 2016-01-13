using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
		protected Color backColor;

		public SkillHandler(GameHandler game)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("skillTree");
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
			if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
			{
				Main.handler = game;
			}
		}

		public void windowResize()
		{
			/*if ((double)Main.window.ClientBounds.Width / Main.window.ClientBounds.Height > targetRatio)
			{
				screenScale = Matrix.CreateScale((float)Main.window.ClientBounds.Height / background.Height); //* Matrix.CreateTranslation(new Vector3(((float)Main.window.ClientBounds.Width / Main.window.ClientBounds.Height - targetRatio) * Main.window.ClientBounds.Height * 0.5f, 0.0f, 0.0f));
			}
			else
			{
				screenScale = Matrix.CreateScale((float)Main.window.ClientBounds.Width / background.Width); //* Matrix.CreateTranslation(new Vector3(0.0f, ((float)Main.window.ClientBounds.Height / Main.window.ClientBounds.Width - 1/targetRatio) * Main.window.ClientBounds.Width * 0.5f, 0.0f));
			}*/
		}
	}
}
