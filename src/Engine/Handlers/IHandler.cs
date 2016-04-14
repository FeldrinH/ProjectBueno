namespace ProjectBueno.Engine
{
	public interface IHandler
	{
		void Update();
		void Draw();
		void windowResize();
		void Initialize();
		void Deinitialize();
	}
}