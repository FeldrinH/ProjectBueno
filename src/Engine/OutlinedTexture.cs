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
		public OutlinedTexture(Texture2D texture, int frameCount, float speed, int w, int h, int yShift = 0, float xOffset = 0.0f, float yOffset = 0.0f) : base(texture,frameCount,speed,w,h,yShift,xOffset,yOffset)
		{
			outlineW = w + 2;
			outlineH = h + 2;
			outlineYShift = yShift * outlineH;
		}

		public readonly Texture2D outlineTexture;

		public readonly int outlineW;
		public readonly int outlineH;
		public readonly int outlineYShift;

		public Rectangle getOutlineFrame()
		{
			return new Rectangle(outlineW * (int)curFrame, outlineYShift, outlineW, outlineH);
		}
	}
}
