using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using ProjectBueno.Game.Spells;
using System;
using System.IO;
using System.Windows.Forms;

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
		public static bool graphicsDirty;

		private static Form windowForm;

		public static readonly JObject Config;

		public static Texture2D boxel { get; private set; }
		public static SpriteFont retroFont { get; private set; }

		public static event EventHandler<EventArgs> exiting;

		public Main()
		{
			graphicsManager = new GraphicsDeviceManager(this) { SynchronizeWithVerticalRetrace = false, PreferMultiSampling = false }; //Bad workaround
			content = Content; //Bad workaround
			window = Window; //Bad workaround
			content.RootDirectory = "Content";
			window.AllowUserResizing = true;
			window.Title = "Project Bueno " + PB.VERSION;
			IsMouseVisible = true;
			IsFixedTimeStep = false;
			//TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 5.0);
			newKeyState = Keyboard.GetState();
			newMouseState = Mouse.GetState();
			graphicsManager.PreferredBackBufferWidth = xRatio * 5;
			graphicsManager.PreferredBackBufferHeight = yRatio * 5;
			oldClientBounds = window.ClientBounds;
		}

		static Main()
		{
			Config = JObject.Parse(File.ReadAllText("Content/Config.json"));
		}

		private void WindowSizeChanged(object sender, EventArgs e)
		{
			if (window.ClientBounds.Width != 0)
			{
				if(windowForm.WindowState == FormWindowState.Maximized)
				{
					WindowMaximized();
				}
				else if (window.ClientBounds.Width < xRatio || window.ClientBounds.Height < yRatio)
				{
					graphicsManager.PreferredBackBufferWidth = xRatio;
					graphicsManager.PreferredBackBufferHeight = yRatio;
				}
				else if (oldClientBounds.Width != window.ClientBounds.Width && graphicsManager.PreferredBackBufferWidth != window.ClientBounds.Width)
				{
					graphicsManager.PreferredBackBufferWidth = window.ClientBounds.Width;
					graphicsManager.PreferredBackBufferHeight = (int)(window.ClientBounds.Width * heightMult);
				}
				else if (oldClientBounds.Height != window.ClientBounds.Height && graphicsManager.PreferredBackBufferHeight != window.ClientBounds.Height)
				{
					graphicsManager.PreferredBackBufferHeight = window.ClientBounds.Height;
					graphicsManager.PreferredBackBufferWidth = (int)(window.ClientBounds.Height * widthMult);
				}
				oldClientBounds = window.ClientBounds;
				
				if (handler != null)
				{
					handler.windowResize();
				}
				graphicsDirty = true;
				//Console.WriteLine("window resize");
			}
		}

		private void WindowMaximized()
		{
			graphicsManager.PreferredBackBufferHeight = window.ClientBounds.Height;
			graphicsManager.PreferredBackBufferWidth = window.ClientBounds.Width;

			Viewport viewport = graphicsManager.GraphicsDevice.Viewport;

			if ((float)window.ClientBounds.Width / window.ClientBounds.Height < widthMult)
			{
				viewport.Height = (int)(window.ClientBounds.Width * heightMult);
				viewport.Y = (window.ClientBounds.Height - viewport.Height) / 2;
			}
			else 
			{
				viewport.Width = (int)(window.ClientBounds.Height * widthMult);
				viewport.X = (window.ClientBounds.Width - viewport.Width) / 2;
			}

			graphicsManager.GraphicsDevice.Viewport = viewport;
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

			windowForm = (Form)Control.FromHandle(window.Handle);
			window.ClientSizeChanged += new EventHandler<EventArgs>(WindowSizeChanged);
			WindowSizeChanged(null, null);

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
			newMouseState = new MouseState(newMouseState.X - graphicsManager.GraphicsDevice.Viewport.X, newMouseState.Y - graphicsManager.GraphicsDevice.Viewport.Y, newMouseState.ScrollWheelValue, newMouseState.LeftButton, newMouseState.MiddleButton, newMouseState.RightButton, newMouseState.XButton1, newMouseState.XButton2);
			handler.Update();
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			if (graphicsDirty)
			{
				graphicsManager.ApplyChanges();
				graphicsDirty = false;
				//Console.WriteLine(graphicsManager.PreferredBackBufferWidth + " " + graphicsManager.PreferredBackBufferHeight + " " + window.ClientBounds);
			}

			graphicsManager.GraphicsDevice.Clear(Color.Black);
			handler.Draw();
		}
	}
}
