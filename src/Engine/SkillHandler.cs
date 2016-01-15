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
		protected List<SkillButton> skillButtons;
		public MouseState drawMouseState { get; protected set; }
		public float downscale { get; protected set; }
		protected Matrix sreenScale;

		public SkillHandler(GameHandler game)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("skillTree");
			backColor = new Color(0, 0, 0);
			skillButtons = new List<SkillButton>();
			skillButtons.Add(new SkillButton(new Rectangle(2, 11, 10, 10), this));
			skillButtons.Add(new SkillButton(new Rectangle(15, 10, 10, 10), this));
			skillButtons.Add(new SkillButton(new Rectangle(15, 24, 10, 10), this));
			skillButtons.Add(new SkillButton(new Rectangle(1, 24, 10, 10), this));
			skillButtons.Add(new SkillButton(new Rectangle(1, 42, 10, 10), this));
			skillButtons.Add(new SkillButton(new Rectangle(33, 10, 10, 10), this));
		}

		public void Draw()
		{
			Main.graphicsManager.GraphicsDevice.Clear(Color.White);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, sreenScale);
			Main.spriteBatch.Draw(background, Vector2.Zero, Color.White);
			drawMouseState = Mouse.GetState();
			foreach (SkillButton skillButton in skillButtons)
			{
				skillButton.Draw();
            }
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
			sreenScale = Matrix.CreateScale((float)Main.window.ClientBounds.Width / Main.xRatio);
			downscale = (float)Main.xRatio / Main.window.ClientBounds.Width ;
		}
	}
}
