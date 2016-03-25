using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	class OutlinedTexture : AnimatedTexture
	{
		public OutlinedTexture(Texture2D texture, Texture2D outlineTexture, int frameCount, float speed, int w, int h, int yShift = 0, float xOffset = 0.0f, float yOffset = 0.0f) : base(texture,frameCount,speed,w,h,yShift,xOffset,yOffset)
		{
			outlineW = w + 2;
			outlineH = h + 2;
			outlineYShift = yShift * outlineH;

			this.outlineTexture = getOutlinedTexture(texture,w,h);
		}

		static OutlinedTexture()
		{
			outlineTextures = new Dictionary<string, Texture2D>();
		}

		public readonly Texture2D outlineTexture;

		public readonly int outlineW;
		public readonly int outlineH;
		public readonly int outlineYShift;

		protected static readonly Dictionary<string,Texture2D> outlineTextures;

		public Rectangle getOutlineFrame()
		{
			return new Rectangle(outlineW * (int)curFrame, outlineYShift, outlineW, outlineH);
		}

		public static Texture2D getOutlinedTexture(Texture2D baseTexture, int w, int h)
		{
			Texture2D outTexture;
			if (!outlineTextures.TryGetValue(baseTexture.Name, out outTexture))
			{
				outTexture = generateOutlinedTexture(w, h, baseTexture);
			}
			return outTexture;
		}
		protected static Texture2D generateOutlinedTexture(int w, int h, Texture2D texture)
		{
			int xCount = texture.Width / w;
			int yCount = texture.Height / h;

			Texture2D outTexture = new Texture2D(Main.graphicsManager.GraphicsDevice, texture.Width + xCount * 2, texture.Height + yCount * 2);
			Color[] baseTexture = new Color[texture.Width * texture.Height];
			texture.GetData(baseTexture);

			w += 2;
			h += 2;

			//TODO

			return outTexture;
		}
	}
}
