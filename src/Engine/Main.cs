using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ProjectBueno.Engine
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public sealed class Main : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphicsManager { get; private set; }
        public static SpriteBatch spriteBatch { get; private set; }
		public static ContentManager content { get; private set; }
		public static GameWindow window { get; private set; }
        public static IHandler handler { get; set; }

		public static KeyboardState oldKeyState { get; private set; }
		public static KeyboardState newKeyState { get; private set; }
		public static MouseState oldMouseState { get; private set; }
		public static MouseState newMouseState { get; private set; }

		public const float widthMult = (float)143 / 107;
		public const float heightMult = (float)107 / 143;

		private static Rectangle oldClientBounds;

		public Main()
        {
			graphicsManager = new GraphicsDeviceManager(this); //{ SynchronizeWithVerticalRetrace = false };
			content = Content;
			content.RootDirectory = "Content";
			window = Window;
			window.AllowUserResizing = true;
			window.ClientSizeChanged += new EventHandler<EventArgs>(WindowSizeChanged);
			IsMouseVisible = true;
			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 30.0);
			oldKeyState = Keyboard.GetState();

			graphicsManager.PreferredBackBufferWidth = 143 * 5;
			graphicsManager.PreferredBackBufferHeight = 107 * 5;
			oldClientBounds = Window.ClientBounds;
		}

		private void WindowSizeChanged(object sender, EventArgs e)
		{
			if (Window.ClientBounds.Width != 0)
			{
				if (Window.ClientBounds.Width < 143 || Window.ClientBounds.Height < 107)
				{
					graphicsManager.PreferredBackBufferWidth = 143;
					graphicsManager.PreferredBackBufferHeight = 107;
				}
				else if (oldClientBounds.Width != Window.ClientBounds.Width && graphicsManager.PreferredBackBufferWidth != Window.ClientBounds.Width)
				{
					graphicsManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
					graphicsManager.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width * heightMult);
				}
				else if (oldClientBounds.Height != Window.ClientBounds.Height && graphicsManager.PreferredBackBufferHeight != Window.ClientBounds.Height)
				{
					graphicsManager.PreferredBackBufferHeight = Window.ClientBounds.Height;
					graphicsManager.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * widthMult);
				}
				oldClientBounds = Window.ClientBounds;
				
				if (handler != null)
				{
					handler.windowResize();
				}
				//Console.WriteLine("Window resize");
			}
		}

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
			base.Initialize();
			handler = new GameHandler();
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			newKeyState = Keyboard.GetState();
			handler.Update();
			oldKeyState = newKeyState;
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			graphicsManager.ApplyChanges();
			handler.Draw();
		}
	}
}
