using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Riot
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region TileSettings

        public static int tile = 256;
        // abitrary choice for 1m (1 tile = 1 meter)
        public static float meter = tile;
        // very exaggerated gravity (6x)
        //public static float gravity = meter * 9.8f * 6.0f;
        // max vertical speed (10 tiles/sec horizontal, 15 tiles/sec vertical)
        public static Vector2 maxVelocity = new Vector2(meter * 2f, meter * 2f);
        // horizontal acceleration -  take 1/2 second to reach max velocity
        public static float accelerationX = maxVelocity.X * 6;
        // horizontal friction - take 1/6 second to stop from max velocity
        public static float frictionX = maxVelocity.X * 3f;
        // Vertical acceleration -  take 1/2 second to reach max velocity
        public static float accelerationY = maxVelocity.Y * 6;
        // Vertical friction - take 1/6 second to stop from max velocity
        public static float frictionY = maxVelocity.Y * 3f;

        #endregion

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region MenuSettings



        #endregion

        #region Characters/Items

        Player player = null;
        List<Enemy> enemies = new List<Enemy>();
        public List<Bullet> Bullets = new List<Bullet>();

        #endregion

        #region HUD

        SpriteFont arial;

        int Shield = 100;
        int Health = 100;
        int Score = 0;

        #endregion

        #region GameWorld

        public Camera2D camera = null;

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

        public float DeltaTime;

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
            player.Position = new Vector2(ScreenWidth / 2 - player.playerTexture.Width, ScreenHeight + 100);

            arial = Content.Load<SpriteFont>("arial");

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, ScreenWidth, ScreenHeight);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, 0);
            camera.Zoom = 0.5f;

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
            DeltaTime = deltaTime;


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

                    SpawnWaves();

                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Update(deltaTime);
                    }

                    foreach (Bullet bullet in Bullets)
                    {
                        bullet.Update(deltaTime);
                    }

                    //camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);

                    CheckCollisions();

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

                    player.Draw(spriteBatch);

                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }

                    foreach (Bullet bullet in Bullets)
                    {
                        bullet.Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    //Screen Space
                    spriteBatch.Begin();

                    spriteBatch.DrawString(arial,Shield.ToString(),new Vector2(0,0),Color.Aqua);
                    spriteBatch.DrawString(arial, Health.ToString(), new Vector2(0, 15), Color.Red);

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

        public int hazardCount = 3;
        public float spawnWait = 0.5f;
        public float startWait = 1.0f;
        public float waveWait = 20.0f;

        int currentHazards = 0;

        void SpawnWaves()
        {
            if (startWait <= 0f)
            {
                if (spawnWait <= 0f && currentHazards < hazardCount)
                {
                    currentHazards++;

                    Random random = new Random();
                    Vector2 spawnPosition = new Vector2(random.Next((int)camera.BoundingRectangle.Left, (int)camera.BoundingRectangle.Right), camera.BoundingRectangle.Top - 100);
                    float spawnRotation = MathHelper.ToRadians(180);
                    Enemy enemy = new Enemy(this);
                    enemy.Load(Content);
                    enemy.GetPlayer = player;
                    enemy.Position = spawnPosition;
                    enemy.Rotation = spawnRotation;
                    enemies.Add(enemy);
                    
                    spawnWait = 0.5f;
                }
                else
                {
                    if (currentHazards >= hazardCount)
                    {
                        if (waveWait <= 0f)
                        {
                            waveWait = 10f;
                            currentHazards = 0;
                        }
                        else
                        {
                            waveWait -= DeltaTime;
                            return;
                        }
                    }
                    spawnWait -= DeltaTime;
                    return;
                }
            }
            else
            {
                startWait -= DeltaTime;
                return;
            }
        }


        private void CheckCollisions()
        {
            foreach (Bullet bullet in Bullets)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (IsColliding(bullet.Bounds,enemy.Bounds) && bullet.shotFrom == Bullet.ShotFrom.player)
                    {
                        Bullets.Remove(bullet);
                        enemies.Remove(enemy);
                        return;
                    }

                    if (IsColliding(bullet.Bounds, player.Bounds) && bullet.shotFrom == Bullet.ShotFrom.enemy)
                    {
                        Bullets.Remove(bullet);

                        if (Shield > 0)
                        {
                            Shield -= 10;
                            Shield = MathHelper.Clamp(Shield,0,100);
                        }
                        else
                        {
                            Health -= 10;
                            Health = MathHelper.Clamp(Shield, 0, 100);
                        }

                        return;
                    }
                }
            }
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
