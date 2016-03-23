using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using ProjectBueno.Game.Spells;
using System;
using System.IO;

namespace ProjectBueno.Engine
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public sealed class Main : Microsoft.Xna.Framework.Game
	{
		//All really bad workaround. Do not use static!
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

		public static readonly JObject Config;

		public static Texture2D boxel { get; private set; }
		public static SpriteFont retroFont { get; private set; }

		public static event EventHandler<EventArgs> exiting;

		public Main()
		{
			graphicsManager = new GraphicsDeviceManager(this) { SynchronizeWithVerticalRetrace = false }; //Bad workaround
			content = Content; //Bad workaround
			window = Window; //Bad workaround
			content.RootDirectory = "Content";
			window.AllowUserResizing = true;
			window.ClientSizeChanged += new EventHandler<EventArgs>(WindowSizeChanged);
			IsMouseVisible = true;
			IsFixedTimeStep = true;
			//TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
			newKeyState = Keyboard.GetState();
			newMouseState = Mouse.GetState();
			graphicsManager.PreferredBackBufferWidth = xRatio * 5;
			graphicsManager.PreferredBackBufferHeight = yRatio * 5;
			oldClientBounds = Window.ClientBounds;
			//IsFixedTimeStep = false;
		}

		static Main()
		{
			Config = JObject.Parse(File.ReadAllText("Content/Config.json"));
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

		//Call static workaround exiting event
		protected override void OnExiting(object sender, EventArgs args)
		{
			//base.OnExiting(sender, args);
			if (exiting != null)
			{
				exiting(sender, args);
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
			spriteBatch = new SpriteBatch(GraphicsDevice); //Bad workaround

			boxel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			boxel.SetData(new[] { Color.White });

			retroFont = Content.Load<SpriteFont>("Font");

			EmptySkill.initEmpty();
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
