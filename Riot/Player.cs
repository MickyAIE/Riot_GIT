using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Riot
{
    class Player
    {
        Game1 game = null;

        Vector2 velocity = Vector2.Zero;
        Vector2 positon = Vector2.Zero;
        float rotation = 0f; // In Radians

        #region Sounds

        SoundEffect pewSound;
        SoundEffectInstance pew;

        #endregion

        #region Sprites / Textures / AnimatedTextures

        Texture2D bulletTexture;

        public Texture2D playerTexture;
        Sprite sprite = new Sprite();
        AnimatedTexture animationPlayer = new AnimatedTexture(Vector2.Zero, 0, 1, 0);

        #endregion

        #region KeepStats

        #endregion

        #region Methods

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Rectangle Bounds
        {
            get { return sprite.Bounds; }
        }

        public Vector2 Position
        {
            get
            {
                return sprite.position;
            }
            set
            {
                sprite.position = value;
            }
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        #endregion

        #region Custom Functions

        float Deg2Rad(float Deg)
        {
            float Rad = Deg;
            Rad = Rad * (float)Math.PI / 180f;
            return Rad;
        }

        // Returns the forward facing direction of the sprite.
        Vector2 Forward()
        {
            Vector2 forwardDirection = Vector2.Zero;
            float rotation = this.rotation + Deg2Rad(270);
            //rotation = MathHelper.ToDegrees(rotation);
            Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            direction.Normalize();

            forwardDirection = direction;

            return forwardDirection;
        }

        Vector2 Left()
        {
            Vector2 forwardDirection = Vector2.Zero;
            float rotation = this.rotation + Deg2Rad(180);
            //rotation = MathHelper.ToDegrees(rotation);
            Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            direction.Normalize();

            forwardDirection = direction;

            return forwardDirection;
        }

        Vector2 Backward()
        {
            Vector2 forwardDirection = Vector2.Zero;
            float rotation = this.rotation + Deg2Rad(90);
            //rotation = MathHelper.ToDegrees(rotation);
            Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            direction.Normalize();

            forwardDirection = direction;

            return forwardDirection;
        }

        Vector2 Right()
        {
            Vector2 forwardDirection = Vector2.Zero;
            float rotation = this.rotation;
            //rotation = MathHelper.ToDegrees(rotation);
            Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            direction.Normalize();

            forwardDirection = direction;

            return forwardDirection;
        }

        #endregion

        public Player(Game1 game)
        {
            this.game = game;
            velocity = Vector2.Zero;
            positon = Vector2.Zero;
            rotation = 0f;
        }

        public void Load(ContentManager content)
        {
            bulletTexture = content.Load<Texture2D>("PNG/Lasers/laserBlue01");
            playerTexture = content.Load<Texture2D>("PNG/playerShip1_blue");
            animationPlayer.Load(content, "PNG/playerShip1_blue", 1, 1);
            animationPlayer.Origin = new Vector2(playerTexture.Width / 2, playerTexture.Height / 2);
            sprite.Add(animationPlayer, playerTexture.Width / 2, playerTexture.Height / 2);
            sprite.Pause();

            pewSound = content.Load<SoundEffect>("Bonus/sfx_laser1");
            pew = pewSound.CreateInstance();

        }

        public void Update(float deltaTime)
        {
            positon = sprite.position;
            animationPlayer.Rotation = rotation;
            sprite.Update(deltaTime);
            Inputs(deltaTime);
            //CollisionDetection();
            LockToScreen();

            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);

            //spriteBatch.DrawRectangle(Bounds, Color.Red, 1);
            //spriteBatch.DrawLine(positon.X, positon.Y, Forward().X, Foward().Y, Color.Red, 5);
            //spriteBatch.DrawLine(positon.X, positon.Y, Right().X, Right().Y, Color.Blue, 5);
            //spriteBatch.DrawLine(positon.X, positon.Y, Backward().X, Backward().Y, Color.Green, 5);
            //spriteBatch.DrawLine(positon.X, positon.Y, Left().X, Left().Y, Color.Yellow, 5);
        }

        #region InputMethods

        bool KeyLeft()
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) || GamePad.GetState(0).IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool KeyRight()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) || GamePad.GetState(0).IsButtonDown(Buttons.LeftThumbstickRight))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool KeyUp()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) || GamePad.GetState(0).IsButtonDown(Buttons.LeftThumbstickUp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool KeyDown()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) || GamePad.GetState(0).IsButtonDown(Buttons.LeftThumbstickDown))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool KeyRotateLeft()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q) || GamePad.GetState(0).IsButtonDown(Buttons.RightThumbstickLeft))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool KeyRotateRight()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.E) || GamePad.GetState(0).IsButtonDown(Buttons.RightThumbstickRight))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool ActionOne()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed || GamePad.GetState(0).IsButtonDown(Buttons.B))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void ReactionOne()
        {
            GamePad.SetVibration(0,1,1);
        }

        #endregion

        #region InputVariables

        float attackDelay = 0.25f;
        float timerDelay = 0.25f;
        bool shotLeft = false;

        #endregion

        void Inputs(float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;

            //bool falling = isFalling;

            Vector2 acceleration = new Vector2(0, 0);

            
            if (ActionOne() == true && timerDelay <= 0)
            {
                if (shotLeft == false)
                {
                    Bullet bullet = new Bullet(game, bulletTexture, 800, Position, rotation, Bullet.ShotFrom.player);
                    bullet.Load(game.Content);
                    game.Bullets.Add(bullet);
                    shotLeft = true;
                    pew.Play();
                }
                else
                {
                    Bullet bullet = new Bullet(game, bulletTexture, 800, new Vector2((Position.X + playerTexture.Width - 5), Position.Y), rotation, Bullet.ShotFrom.player);
                    bullet.Load(game.Content);
                    game.Bullets.Add(bullet);
                    shotLeft = false;
                    pew.Play();
                }

                //Code to Run
                

                timerDelay = attackDelay;
            }
            else
            {
                timerDelay -= deltaTime;
            }

            if (KeyLeft() == true)
            {
                rotation -= Deg2Rad(3);
            }

            if (KeyRight() == true)
            {
                rotation += Deg2Rad(3);
            }

            bool wasMovingUp = velocity.Y < 0;
            bool wasMovingDown = velocity.Y > 0;

            //bool falling = isFalling;

            if (KeyUp() == true)
            {
                //acceleration.Y -= Game1.accelerationY;
                velocity += Forward() * 300 * deltaTime;
            }
            
            if (KeyDown() == true)
            {
                //acceleration.Y += Game1.accelerationY;
                velocity += Backward() * 300 * deltaTime;
            }

            Console.WriteLine(velocity);

            if ( velocity != Vector2.Zero && KeyUp() == false && KeyDown() == false )
            {
                Console.WriteLine("Running");
                velocity = Vector2.Lerp(velocity, Vector2.Zero, 1f * deltaTime);
            }

            //velocity += acceleration * deltaTime;

            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            sprite.position += velocity * deltaTime;
        }     
        
        void LockToScreen()
        {
            if (Position.X <= game.camera.BoundingRectangle.Left)
            {
                sprite.position.X = game.camera.BoundingRectangle.Left;
            }
            if (Position.X + playerTexture.Width >= game.camera.BoundingRectangle.Right)
            {
                sprite.position.X = game.camera.BoundingRectangle.Right - playerTexture.Width;
            }
            if (Position.Y <= game.camera.BoundingRectangle.Top)
            {
                sprite.position.Y = game.camera.BoundingRectangle.Top;
            }
            if (Position.Y + playerTexture.Height >= game.camera.BoundingRectangle.Bottom)
            {
                sprite.position.Y = game.camera.BoundingRectangle.Bottom - playerTexture.Height;
            }
        }

    }
}
