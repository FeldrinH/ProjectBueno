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

		public const int xRatio = 140;
		public const int yRatio = 105;
        public const float widthMult = (float)xRatio / yRatio;
		public const float heightMult = (float)yRatio / xRatio;

		private static Rectangle oldClientBounds;

		public static Texture2D boxel;

		public Main()
        {
			graphicsManager = new GraphicsDeviceManager(this);// { SynchronizeWithVerticalRetrace = false };
			content = Content;
			content.RootDirectory = "Content";
			window = Window;
			window.AllowUserResizing = true;
			window.ClientSizeChanged += new EventHandler<EventArgs>(WindowSizeChanged);
			IsMouseVisible = true;
			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 30.0);
			newKeyState = Keyboard.GetState();
			newMouseState = Mouse.GetState();
			graphicsManager.PreferredBackBufferWidth = xRatio * 5;
			graphicsManager.PreferredBackBufferHeight = yRatio * 5;
			oldClientBounds = Window.ClientBounds;
			//IsFixedTimeStep = false;
		}

		private void WindowSizeChanged(object sender, EventArgs e)
		{
			if (Window.ClientBounds.Width != 0)
			{
				if (Window.ClientBounds.Width < xRatio || Window.ClientBounds.Height < yRatio)
				{
					graphicsManager.PreferredBackBufferWidth = xRatio;
					graphicsManager.PreferredBackBufferHeight = yRatio;
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

			boxel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			boxel.SetData(new[] { Color.White });
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
			oldKeyState = newKeyState;
			oldMouseState = newMouseState;
			newKeyState = Keyboard.GetState();
			newMouseState = Mouse.GetState();
			handler.Update();
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
