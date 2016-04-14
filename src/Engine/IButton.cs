using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	public interface IButton
	{
		void Draw(float mouseX, float mouseY);

		void OnClick(float mouseX, float mouseY);
	}
}
