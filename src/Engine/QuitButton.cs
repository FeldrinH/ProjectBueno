using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	class QuitButton : IButton
	{
		public QuitButton(JObject data)
		{
			text = (string)data["text"];
			bounds = new Rectangle((int)data["x"], (int)data["y"], (int)data["w"], (int)data["h"]);
			pos = new Vector2((float)data["x"], (float)data["y"] - 1.0f);
		}

		public Rectangle bounds;
		public Vector2 pos;
		public string text;

		public void Draw(float mouseX, float mouseY)
		{
			Main.spriteBatch.DrawString(Main.retroFont, text, pos, bounds.Contains(mouseX, mouseY) ? TextMoveButton.highlightColor : TextMoveButton.textColor);
		}

		public void OnClick(float mouseX, float mouseY)
		{
			if (bounds.Contains(mouseX, mouseY))
			{
				Main.Handler.Deinitialize();
				Main.self.Exit();
			}
		}
	}
}
