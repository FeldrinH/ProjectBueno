using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
			if (Main.newKeyState.GetPressedKeys().Except(Main.oldKeyState.GetPressedKeys()).Any())
			{
				game.Screen = null;
			}
		}

		protected Texture2D texture;
	}
}
