using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	class MoveButton : IButton
	{
		public MoveButton(IHandler moveTo, JObject data, bool bypass = false)
		{
			texture = Main.content.Load<Texture2D>((string)data["texture"]);
			bounds = new Rectangle((int)data["x"], (int)data["y"], (int)data["w"], (int)data["h"]);
			this.moveTo = moveTo;

			this.bypass = bypass;
		}

		public Rectangle bounds;
		public Texture2D texture;
		public IHandler moveTo;

		protected bool bypass; //Bypass Initialize and Deinitialize

		public static readonly Color textColor = new Color(255,250,204);
		public static readonly Color highlightColor = new Color(66,147,0);

		public void Draw(float mouseX, float mouseY)
		{
			Main.spriteBatch.Draw(texture, bounds, new Rectangle(bounds.Contains(mouseX, mouseY) ? bounds.Width : 0, 0, bounds.Width, bounds.Height), Color.White);
		}

		public void OnClick(float mouseX, float mouseY)
		{
			if (bounds.Contains(mouseX, mouseY))
			{
				if (bypass)
				{
					Main.HandlerBP = moveTo;
				}
				else
				{
					Main.Handler = moveTo;
				}
			}
		}
	}
}
