namespace ProjectBueno.Engine
{
	/// <summary>
	/// The interface for the handler classes. <para/>
	/// The game can have 1 active <see cref="IHandler"/>, which is stored in <see cref="Main.Handler"/>
	/// </summary>
	public interface IHandler
	{
		/// <summary>
		/// Called on update. <para/> 
		/// Should call all the update logic of its contents.
		/// </summary>
		void Update();

		/// <summary>
		/// Called on draw. <para/> 
		/// Should call all the draw logic of its contents.
		/// </summary>
		void Draw();

		/// <summary>
		/// Called on window resize. Might be called with no resize having ocurred (for instance on change of current handler or when window size is maxed out). <para/> 
		/// Should change all values dependent on screen size to their correct new values. <para/>
		/// Use <see cref="Main.Viewport"/> to get current screen size.
		/// </summary>
		void WindowResize();

		/// <summary>
		/// Called when the handler becomes the active handler (unless intentionally bypassed). <para/>
		///	See also: <seealso cref="Main.Handler"/>
		/// </summary>
		void Initialize();

		/// <summary>
		/// Called when the handler is no longer the active handler (unless intentionally bypassed). <para/>
		///	See also: <seealso cref="Main.Handler"/>
		/// </summary>
		void Deinitialize();
	}
}