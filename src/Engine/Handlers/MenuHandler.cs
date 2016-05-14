using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	public abstract class MenuHandler : IHandler
	{
		protected Texture2D background;
		public float downscale { get; protected set; }
		protected Matrix screenScale;

		public abstract void Draw();

		public abstract void Update();

		public virtual void WindowResize()
		{
			screenScale = Matrix.CreateScale((float)Main.Viewport.Width / Main.xRatio);
			downscale = (float)Main.xRatio / Main.Viewport.Width;
		}

		public virtual void Initialize()
		{
		}

		public virtual void Deinitialize()
		{
		}
	}
}
