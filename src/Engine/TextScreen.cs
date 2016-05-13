using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	public class TextScreen
	{
		public TextScreen(Texture2D texture)
		{
			this.texture = texture;
		}

		public void Draw()
		{
			Main.spriteBatch.Draw(texture, Vector2.Zero, Color.White);
		}

		public void Update(GameHandler game)
		{
			if ((Main.newMouseState.RightButton == ButtonState.Pressed && Main.oldMouseState.RightButton == ButtonState.Released) || (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released) || Main.newKeyState.GetPressedKeys().Except(Main.oldKeyState.GetPressedKeys()).Any())
			{
				game.Screen = null;
				//game.Initialize();
			}
		}

		protected Texture2D texture;
	}
}
