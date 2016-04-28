using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Game.Entities;
using System.Collections.Generic;
using System;
using ProjectBueno.Engine.World;
using System.Diagnostics;
using ProjectBueno.Game.Spells;
using ProjectBueno.Utility;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Audio;

namespace ProjectBueno.Engine
{
	public class GameHandler : IHandler
	{
		public GameHandler()
		{
			Main.exiting += onExitSave;

			player = new Player(new Vector2(Terrain.xSize * Terrain.BLOCK_SIZE * Terrain.TILESIZE * 0.5f, Terrain.ySize * Terrain.BLOCK_SIZE * Terrain.TILESIZE * 0.5f), this);
			screenScale = 2.0f;
			projectiles = new List<Projectile>();
			entities = new List<Entity>();

			terrain = new Terrain();

			terrain.Generate(player.pos, player.size);

			hudHealthTexture = Main.content.Load<Texture2D>("healthBar");
			hudCooldownTexture = Main.content.Load<Texture2D>("cooldownBar");
			hudBackground = Main.content.Load<Texture2D>("hudBackground");

			biomeMusic = new[] { Main.content.Load<SoundEffect>("startMusic").CreateInstance(), Main.content.Load<SoundEffect>("pauseMusic").CreateInstance(), Main.content.Load<SoundEffect>("ColdMusic").CreateInstance(), Main.content.Load<SoundEffect>("startMusic").CreateInstance() };
			foreach (var music in biomeMusic)
			{
				music.IsLooped = true;
			}
			clockTick = Main.content.Load<SoundEffect>("tiktok").CreateInstance();
			clockTick.IsLooped = true;
			_curMusic = clockTick;

			drawDebug = false;

			Screen = new TextScreen(Main.content.Load<Texture2D>("welcomeScreen"));

			//selectedEnemy = new AnimatedTexture(Main.content.Load<Texture2D>("selectedTargetTest"),2,1.0f/30,14,16);
			//selectedEnemySize = new Vector2(selectedEnemy.w, selectedEnemy.h);
			//loadedChunks = new List<List<List<Tiles>>>();

			outlineShader = Main.content.Load<Effect>("OutlineShader");

			maskStencil = new DepthStencilState
			{
				StencilEnable = true,
				StencilFunction = CompareFunction.Always,
				StencilPass = StencilOperation.Replace,
				ReferenceStencil = 1,
				DepthBufferEnable = false,
			};
			drawStencil = new DepthStencilState
			{
				StencilEnable = true,
				StencilFunction = CompareFunction.GreaterEqual,
				StencilPass = StencilOperation.Keep,
				ReferenceStencil = 0,
				DepthBufferEnable = false,
			};

			blendTransparent = new BlendState
			{
				ColorWriteChannels = ColorWriteChannels.None
			};

			alphaTest = new AlphaTestEffect(Main.graphicsManager.GraphicsDevice)
			{
				Projection = Matrix.CreateOrthographicOffCenter(0, Main.graphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight, 0, 0, 1)
			};

			_doUpdate = false;
		}

		static GameHandler()
		{
			JObject data = (JObject)Main.Config["hudHealthBar"];
			hudHealthBar = new Rectangle((int)data["x"], (int)data["y"], (int)data["w"], (int)data["h"]);

			data = (JObject)Main.Config["hudCooldownBar"];
			hudCooldownBar = new Rectangle((int)data["x"], (int)data["y"], (int)data["w"], (int)data["h"]);

			data = (JObject)Main.Config["hudKp"];
			hudKpPos = new Vector2((float)data["x"], (float)data["y"] - 1.0f);
		}

		public TextScreen Screen;

		protected static Texture2D hudBackground;

		protected static readonly Vector2 hudKpPos;

		protected static Texture2D hudHealthTexture;
		protected static readonly Rectangle hudHealthBar;

		protected static Texture2D hudCooldownTexture;
		protected static readonly Rectangle hudCooldownBar;

		private float _screenScale;
		public float screenScale
		{
			get { return _screenScale; }
			set
			{
				screenScaleInv = 1 / value;
				_screenScale = value;
			}
		}
		public float screenScaleInv;
		public Vector2 screenShift;
		public Matrix screenMatrix;

		protected Matrix hudScale;
		protected Matrix textScale;

		protected bool drawDebug;

		private SoundEffectInstance _curMusic;

		public static SoundEffectInstance[] biomeMusic;
		protected static SoundEffectInstance clockTick;
		public SoundEffectInstance curMusic
		{
			get { return _curMusic; }
			set
			{
				if (value != _curMusic)
				{
					_curMusic.Pause();

					if (value.State == SoundState.Stopped || value == clockTick)
					{
						value.Play();
					}
					else
					{
						value.Resume();
					}
					
					_curMusic = value;
				}
			}
		}

		protected bool _doUpdate;
		public bool doUpdate
		{
			get { return _doUpdate; }
			set
			{
				if (_doUpdate != value)
				{
					if (value)
					{
						curMusic = biomeMusic[(int)player.curBiome];
					}
					else
					{
						curMusic = clockTick; 
					}
					_doUpdate = value;
				}
			}
		}

		protected static Effect outlineShader;
		protected static DepthStencilState maskStencil, drawStencil;
		protected static AlphaTestEffect alphaTest;
		protected static BlendState blendTransparent;

		protected AnimatedTexture selectedEnemy;
		protected Vector2 selectedEnemySize;

		protected static readonly Random random = new Random();

		public List<Entity> entities { get; protected set; }
		public List<Projectile> projectiles { get; protected set; }
		//protected Point ppChunk, mmChunk, mpChunk, pmChunk;
		public Player player { get; protected set; }
		public Terrain terrain { get; protected set; }

		public Vector2 posFromScreenPos(Vector2 screenPos)
		{
			return (screenPos - screenShift) * screenScaleInv + player.pos;
		}

		public Entity getEntityAtPos(Vector2 pos)
		{
			foreach (var entity in entities)
			{
				if (entity.checkCollision(pos, Vector2.Zero))
				{
					return entity;
				}
			}
			return null;
		}

		public void addProjectile(Projectile proj)
		{
			//if (proj != null)
			//{
			projectiles.Add(proj);
			//}
		}

		private void onExitSave(object sender, EventArgs args)
		{
			Console.WriteLine("NOW PRETENDING TO SAVE GAME");
		}

		public void closeGameHandler()
		{
			onExitSave(null, null);
			Main.exiting -= onExitSave;
		}

		public void windowResize()
		{
			screenShift = new Vector2((float)Math.Floor((Main.graphicsManager.GraphicsDevice.Viewport.Width - (player.size.X * screenScale)) * 0.5), (float)Math.Floor((Main.graphicsManager.GraphicsDevice.Viewport.Height - (player.size.Y * screenScale)) * 0.5));
			screenMatrix = Matrix.CreateScale(screenScale) * Matrix.CreateTranslation(new Vector3(screenShift, 0.0f));

			textScale = Matrix.CreateScale((float)Main.graphicsManager.GraphicsDevice.Viewport.Width / Main.xRatio);
			hudScale = Matrix.CreateScale((float)Math.Round(0.5 * Main.graphicsManager.GraphicsDevice.Viewport.Width / Main.xRatio, MidpointRounding.AwayFromZero));

			alphaTest = new AlphaTestEffect(Main.graphicsManager.GraphicsDevice)
			{
				Projection = Matrix.CreateOrthographicOffCenter(0, Main.graphicsManager.GraphicsDevice.Viewport.Width, Main.graphicsManager.GraphicsDevice.Viewport.Height, 0, 0, 1)
			};
		}

		public void AddEntity(Entity ent)
		{
			if (ent != null)
			{
				entities.Add(ent);
			}
		}

		public void Draw()
		{
			Matrix matrixCache = Matrix.CreateTranslation(new Vector3(-player.pos, 0.0f)) * screenMatrix;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, matrixCache);

			//int xRight = Math.Min((int)((player.pos.X + Main.window.ClientBounds.Width * screenScaleInv * 0.5f) * Tile.TILEMULT) + 2, Terrain.xSize);
			//int yBottom = Math.Min((int)((player.pos.Y + Main.window.ClientBounds.Height * screenScaleInv * 0.5f) * Tile.TILEMULT) + 2, Terrain.ySize);

			Point playerChunk = Terrain.getChunkFromPos(player.pos);
			int hShift = (player.pos.X - playerChunk.ToVector2().X * Terrain.CHUNK_SIZE * Terrain.TILESIZE) > Terrain.CHUNK_SHIFT ? 1 : -1;
			int vShift = (player.pos.Y - playerChunk.ToVector2().Y * Terrain.CHUNK_SIZE * Terrain.TILESIZE) > Terrain.CHUNK_SHIFT ? 1 : -1;
			terrain.drawChunk(playerChunk);
			terrain.drawChunk(new Point(playerChunk.X + hShift, playerChunk.Y));
			terrain.drawChunk(new Point(playerChunk.X, playerChunk.Y + vShift));
			terrain.drawChunk(new Point(playerChunk.X + hShift, playerChunk.Y + vShift));

			player.Draw();
			for (int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Draw();
			}

			foreach (var ent in entities)
			{
				ent.Draw();
			}

			if (vShift == 1)
			{
				terrain.drawTrees(playerChunk);
				terrain.drawTrees(new Point(playerChunk.X + hShift, playerChunk.Y));
			}
			terrain.drawTrees(new Point(playerChunk.X, playerChunk.Y + vShift));
			terrain.drawTrees(new Point(playerChunk.X + hShift, playerChunk.Y + vShift));
			if (vShift == -1)
			{
				terrain.drawTrees(playerChunk);
				terrain.drawTrees(new Point(playerChunk.X + hShift, playerChunk.Y));
			}

			if (drawDebug)
			{
				player.DrawDebug();
				for (int i = 0; i < projectiles.Count; i++)
				{
					projectiles[i].DrawDebug();
				}

				foreach (var ent in entities)
				{
					ent.DrawDebug();
				}
			}

			Main.spriteBatch.End();


			if (player.target != null)
			{
				alphaTest.World = matrixCache;

				Main.spriteBatch.Begin(SpriteSortMode.Deferred, blendTransparent, SamplerState.PointClamp, maskStencil, null, alphaTest, matrixCache);
				player.target.DrawRaw();
				Main.spriteBatch.End();

				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, drawStencil, null, outlineShader, matrixCache);
				player.target.DrawOutline(player.target.isAlly ? Color.Lime : Color.Red);
				Main.spriteBatch.End();

				//selectedEnemy.incrementAnimation();
				//Main.spriteBatch.Draw(selectedEnemy.texture, player.target.pos+0.5f*(player.target.size-selectedEnemySize),selectedEnemy.getCurFrame(), Color.White);
			}


			//HUD

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, hudScale);

			Main.spriteBatch.Draw(hudBackground, Vector2.Zero, Color.White);

			Main.spriteBatch.DrawString(Main.retroFont, player.knowledgePoints + "KP", hudKpPos, Color.White);

			Main.spriteBatch.Draw(hudHealthTexture, hudHealthBar.ToVector(), hudHealthBar.ScaleSize(player.Health * player.maxHealthMult), Color.White);

			Main.spriteBatch.Draw(hudCooldownTexture, hudCooldownBar.ToVector(), player.lastCast == null ? hudCooldownBar.ToSize() : hudCooldownBar.ScaleSize(player.lastCast.cooldown - player.cooldown, player.lastCast.cooldown), Color.White);

			Main.spriteBatch.End();


			if (Main.newKeyState.IsKeyDown(Keys.M))
			{
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
				terrain.drawChunkMap(player.pos);
				Main.spriteBatch.End();
			}

			if (Screen != null)
			{
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, textScale);
				Screen.Draw();
				Main.spriteBatch.End();
			}
		}

		public void Update()
		{
			if (Screen == null)
			{
				if (Main.newKeyState.IsKeyDown(Keys.Up) && !Main.oldKeyState.IsKeyDown(Keys.Up))
				{
					screenScale *= 2.0f;
					windowResize();
				}
				if (Main.newKeyState.IsKeyDown(Keys.Down) && !Main.oldKeyState.IsKeyDown(Keys.Down))
				{
					screenScale *= 0.5f;
					windowResize();
				}

				if (Main.newKeyState.IsKeyDown(Keys.Enter) && !Main.oldKeyState.IsKeyDown(Keys.Enter))
				{
					terrain.Generate(player.pos, player.size);
				}

				if (Main.newKeyState.IsKeyDown(Keys.K) && !Main.oldKeyState.IsKeyDown(Keys.K))
				{
					entities.Clear();
				}

				if (entities.Count < 10)
				{
					AddEntity(EnemyManager.SpawnEnemy(player.pos + AngleVector.Vector(random.NextDouble() * 360.0) * 500.0f, this));
				}

				projectiles.RemoveAll(item => item.toRemove);
				entities.RemoveAll(item => item.isDead);

				player.Update();

				if (doUpdate)
				{
					for (int i = 0; i < projectiles.Count; i++)
					{
						projectiles[i].Update();
					}
					foreach (var ent in entities)
					{
						ent.Update();
					}
				}

				if (player.target != null && player.target.isDead)
				{
					player.target = null;
				}
				if (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released)
				{
					player.target = getEntityAtPos(posFromScreenPos(Main.newMouseState.Position.ToVector2()));
					//Console.WriteLine("Mouse:" + Main.newMouseState.Position);
				}

				if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
				{
					Main.handler = new SkillHandler(this, player);
				}
				if (Main.newKeyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P))
				{
					Main.handler = new PauseHandler(this);
				}

				if (Main.newKeyState.IsKeyDown(Keys.C) && !Main.oldKeyState.IsKeyDown(Keys.C))
				{
					drawDebug = !drawDebug;
				}

			}
			else
			{
				Screen.Update(this);
			}
		}

		public void Initialize()
		{
			curMusic.Play();
		}

		public void Deinitialize()
		{
			curMusic.Pause();
		}
	}
}