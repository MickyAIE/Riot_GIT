using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace Riot
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region TileSettings

        public static int tile = 16;
        // abitrary choice for 1m (1 tile = 1 meter)
        public static float meter = tile;
        // very exaggerated gravity (6x)
        //public static float gravity = meter * 9.8f * 6.0f;
        // max vertical speed (10 tiles/sec horizontal, 15 tiles/sec vertical)
        public static Vector2 maxVelocity = new Vector2(meter * 5f, meter * 4f);
        // horizontal acceleration -  take 1/2 second to reach max velocity
        public static float accelerationX = maxVelocity.X * 2;
        // horizontal friction - take 1/6 second to stop from max velocity
        public static float frictionX = maxVelocity.X * 6f;
        // horizontal acceleration -  take 1/2 second to reach max velocity
        public static float accelerationY = maxVelocity.Y * 2;
        // horizontal friction - take 1/6 second to stop from max velocity
        public static float frictionY = maxVelocity.Y * 6f;

        #endregion

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region MenuSettings



        #endregion

        #region Characters/Items

        Player player = null;
        //Zombie zombie = null;

        List<Zombie> zombies = new List<Zombie>();

        #endregion

        #region HUD

        SpriteFont arialFont;
        int score = 0;
        int lives = 3;

        Texture2D heartImage = null;

        #endregion

        #region GameWorld

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;

        #endregion

        enum GameState
        {
            Splash_State,
            Menu_State,
            Playing_State,
            GameOver_State,
            GameWin_State
        }
        GameState GetGameState = GameState.Playing_State;

        #region Methods

        public int ScreenWidth
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Width;
            }
        }

        public int ScreenHeight
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Height;
            }
        }

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            player = new Player(this);
            player.Position = new Vector2(96, 480);

            //zombie = new Zombie(this);
            //zombie.Position = new Vector2(200, 480);

            graphics.PreferMultiSampling = true;
            //graphics.BlendState = BlendState.AlphaBlend;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            player.Load(Content);
            //zombie.Load(Content);
            //zombie.GetPlayer = player;

            arialFont = Content.Load<SpriteFont>("Arial");
            heartImage = Content.Load<Texture2D>("Heart");

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, ScreenWidth, ScreenHeight);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, ScreenHeight);

            map = Content.Load<TiledMap>("Level1");
            mapRenderer = new TiledMapRenderer(GraphicsDevice);

            //ZombieSpawns

            foreach (TiledMapObjectLayer layer in map.ObjectLayers)
            {
                if (layer.Name == "ZombieSpawns")
                {
                    foreach (TiledMapObject obj in layer.Objects)
                    {
                        Zombie zombie = new Zombie(this);
                        zombie.Load(Content);
                        zombie.GetPlayer = player;
                        zombie.Position = new Vector2(obj.Position.X,obj.Position.Y);
                        zombies.Add(zombie);
                    }
                }
            }

            foreach (TiledMapTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collision")
                {
                    collisionLayer = layer;
                }
            }

            MasterCheck();
        }

        void MasterCheck()
        {
            if (graphics == null)
            {
                throw new System.ArgumentException("Graphics is null, please check code and declare!", "graphics");
            }
            if (spriteBatch == null)
            {
                throw new System.ArgumentException("SpriteBatch is null, please check code and declare!", "spriteBatch");
            }
            if (player == null)
            {
                throw new System.ArgumentException("Player is null, please check code and declare!", "player");
            }
            if (arialFont == null)
            {
                throw new System.ArgumentException("ArialFont is null, please check code and declare!", "arialFont");
            }
            if (heartImage == null)
            {
                throw new System.ArgumentException("HeartImage is null, please check code and declare!", "heartImage");
            }
            if (map == null)
            {
                throw new System.ArgumentException("Map is null, please check code and declare!", "map");
            }
            if (mapRenderer == null)
            {
                throw new System.ArgumentException("MapRenderer is null, please check code and declare!", "mapRenderer");
            }
            if (collisionLayer == null)
            {
                throw new System.ArgumentException("CollisionLayer is null, please check code and declare!", "collisionLayer");
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        bool RunOnce = false;
        float Timer = 3f;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Add your update logic here

            switch (GetGameState)
            {
                case GameState.Splash_State:

                    if (RunOnce != true)
                    {
                        Timer = 3f;
                        IsMouseVisible = false;
                        RunOnce = true;
                    }

                    Timer -= deltaTime;
                    if (Timer <= 0)
                    {
                        ChangeState(GameState.Menu_State);
                    }

                    break;
                case GameState.Menu_State:

                    if (RunOnce != true)
                    {
                        Timer = 3f;
                        IsMouseVisible = true;
                        RunOnce = true;
                    }

                    break;
                case GameState.Playing_State:

                    if (RunOnce != true)
                    {
                        IsMouseVisible = false;
                        RunOnce = true;
                    }

                    player.Update(deltaTime);

                    foreach (Zombie zombie in zombies)
                    {
                        zombie.Update(deltaTime);
                    }

                    camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                    camera.Zoom = 2f;

                    //CheckCollisions();

                    break;
                case GameState.GameOver_State:

                    if (RunOnce != true)
                    {
                        //MediaPlayer.Play(gameMusic);
                        IsMouseVisible = true;
                        RunOnce = true;
                    }

                    break;
                case GameState.GameWin_State:

                    if (RunOnce != true)
                    {
                        //MediaPlayer.Play(gameMusic);
                        IsMouseVisible = true;
                        RunOnce = true;
                    }

                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        void ChangeState(GameState ChangeToState)
        {
            GetGameState = ChangeToState;
            RunOnce = false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(28,17,23));
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            // TODO: Add your drawing code here
            switch (GetGameState)
            {
                case GameState.Splash_State:

                    spriteBatch.Begin();



                    spriteBatch.End();

                    break;
                case GameState.Menu_State:

                    spriteBatch.Begin();
                    


                    spriteBatch.End();

                    break;
                case GameState.Playing_State:

                    // Previous Game Code is now here.

                    // TODO: Add your drawing code here

                    Matrix viewMatrix = camera.GetViewMatrix();
                    Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 0f, -2f);

                    // World Space
                    spriteBatch.Begin(transformMatrix: viewMatrix, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.BackToFront);
                    

                    mapRenderer.Draw(map.GetLayer("bg"), ref viewMatrix, ref projectionMatrix, null, 0);
                    mapRenderer.Draw(map.GetLayer("bg2"), ref viewMatrix, ref projectionMatrix, null, 0);
                    mapRenderer.Draw(map.GetLayer("floor"), ref viewMatrix, ref projectionMatrix, null, 0);
                    mapRenderer.Draw(map.GetLayer("fg"), ref viewMatrix, ref projectionMatrix, null, 0);

                    player.Draw(spriteBatch);
                    spriteBatch.DrawRectangle(player.Bounds, Color.Red, 1);

                    foreach (Zombie zombie in zombies)
                    {
                        zombie.Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    //Screen Space
                    spriteBatch.Begin();



                    spriteBatch.End();

                    break;
                case GameState.GameOver_State:

                    spriteBatch.Begin();



                    spriteBatch.End();

                    break;
                case GameState.GameWin_State:

                    spriteBatch.Begin();



                    spriteBatch.End();

                    break;
                default:
                    break;
            }

            spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            base.Draw(gameTime);
        }

        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }

        public int TileToPixel(int tileCoord)
        {
            return tile * tileCoord;
        }

        public int CellAtPixelCoord(Vector2 pixelCoords)
        {
            if (pixelCoords.X < 0 || pixelCoords.X > map.WidthInPixels || pixelCoords.Y < 0)
            {
                return 1;
            }
            if (pixelCoords.Y > map.HeightInPixels)
            {
                return 0;
            }
            return CellAtTileCoord(PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));
        }

        public int CellAtTileCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
            {
                return 1;
            }
            if (ty >= map.Height)
            {
                return 0;
            }

            TiledMapTile? tile;
            collisionLayer.TryGetTile(tx, ty, out tile);
            return tile.Value.GlobalIdentifier;

        }

        private void CheckCollisions()
        {
            
        }

        public bool IsColliding(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect1.Width < rect2.X ||
                rect1.X > rect2.X + rect2.Width ||
                rect1.Y + rect1.Height < rect2.Y ||
                rect1.Y > rect2.Y + rect2.Height)
            {
                // these two rectangles are not colliding
                return false;
            }
            else
            {
                // else, the two AABB rectangles overlap, therefore collision
                return true;
            }
        }

    }
}
