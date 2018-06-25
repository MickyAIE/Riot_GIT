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

        #region Content

        SoundEffect swordSwingSound = null;
        SoundEffectInstance swordSwingInstance = null;

        #endregion

        #region Sprites / Textures / AnimatedTextures

        Texture2D playerTexture;
        Sprite sprite = new Sprite();
        AnimatedTexture animationPlayer = new AnimatedTexture(Vector2.Zero, 0, 1, 0);

        Texture2D swordTexture;
        public Sprite swordSprite = new Sprite();
        AnimatedTexture animationSword = new AnimatedTexture(Vector2.Zero, 0, 1, 0);
        BoundingBox swordBoundingBox;

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

        #endregion

        #region Custom Functions

        BoundingBox CreateBoundingBox(float PosX, float PosY, float Width, float Height, float Rotation)
        {
            Vector2 v1, v2, v3, v4;
            v1 = new Vector2(PosX,PosY);
            v2 = new Vector2(PosX + Width, PosY);
            v3 = new Vector2(PosX, PosY + Height);
            v4 = new Vector2(PosX + Width,PosY + Height);

            Matrix rotate = Matrix.CreateRotationY(Rotation);
            v1 = Vector2.Transform(v1, rotate);
            v4 = Vector2.Transform(v4, rotate);


            Vector3 newMin, newMax;
            newMin = new Vector3(v1.X,v1.Y,0);
            newMax = new Vector3(v4.X, v4.Y, 0);

            return new BoundingBox(newMin, newMax);
        }

        
        Vector2 FowardDirection(float rotationDirection) // Requires float in radians, Local Direction
        {
            Vector2 forwardDirection = Vector2.Zero;
            float rotation = rotationDirection;
            //rotation = MathHelper.ToDegrees(rotation);
            Vector2 direction = new Vector2( (float)Math.Cos(rotation), (float)Math.Sin(rotation) );
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
        }

        public void Load(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>("Player");
            animationPlayer.Load(content, "Player", 1, 1);
            animationPlayer.Origin = new Vector2(playerTexture.Width / 2, playerTexture.Height / 2);
            sprite.Add(animationPlayer, playerTexture.Width / 2, playerTexture.Height / 2);
            sprite.Pause();

            swordTexture = content.Load<Texture2D>("Player");
            animationSword.Load(content, "bronzeSword", 1, 1);
            animationSword.Origin = new Vector2(swordTexture.Width/2,swordTexture.Height/2);
            animationSword.Rotation = Deg2Rad(90);
            swordSprite.Add(animationSword, swordTexture.Width / 2, swordTexture.Height / 2);
            swordSprite.Pause();

            swordSwingSound = content.Load<SoundEffect>("Hit_Hurt5");
            swordSwingInstance = swordSwingSound.CreateInstance();

            swordBoundingBox = CreateBoundingBox(swordSprite.position.X, swordSprite.position.Y, swordTexture.Width/2, swordTexture.Height, animationSword.Rotation);
        }

        public void Update(float deltaTime)
        {

            sprite.Update(deltaTime);
            swordSprite.Update(deltaTime);
            Inputs(deltaTime);
            SwordLogic(deltaTime);
            CollisionDetection();
            swordBoundingBox = CreateBoundingBox(swordSprite.position.X, swordSprite.position.Y, swordTexture.Width/2, swordTexture.Height, animationSword.Rotation);
            //Console.WriteLine(FowardDirection(rotation));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //sprite.Draw(spriteBatch);

            spriteBatch.Draw(playerTexture, position: Position, rotation: rotation);
            spriteBatch.DrawLine(Position, (Position + (FowardDirection(rotation + MathHelper.ToRadians(270)) * 10)), Color.Yellow, 1);
            spriteBatch.DrawLine(Position, (Position + (FowardDirection(rotation) * 10)), Color.Blue, 1);
            spriteBatch.DrawLine(Position, (Position + (FowardDirection(rotation + MathHelper.ToRadians(180)) * 10)), Color.Red, 1);
            spriteBatch.DrawLine(Position, (Position + (FowardDirection(rotation + MathHelper.ToRadians(90)) * 10)), Color.Green, 1);
            if (timerDelay >= 0)
            {
                swordSprite.Draw(spriteBatch);
                //spriteBatch.DrawRectangle(swordSprite.Bounds, Color.Red, 1);
                //spriteBatch.DrawLine(swordBoundingBox.Min.X, swordBoundingBox.Min.Y, swordBoundingBox.Min.X, swordBoundingBox.Max.Y, Color.Red, 1);
                //spriteBatch.DrawLine(swordBoundingBox.Min.X, swordBoundingBox.Min.Y, swordBoundingBox.Max.X, swordBoundingBox.Min.Y, Color.Green, 1);
                //spriteBatch.DrawLine(swordBoundingBox.Max.X, swordBoundingBox.Min.Y, swordBoundingBox.Max.X, swordBoundingBox.Max.Y, Color.Blue, 1);
                //spriteBatch.DrawLine(swordBoundingBox.Max.X, swordBoundingBox.Max.Y, swordBoundingBox.Min.X, swordBoundingBox.Max.Y, Color.Yellow, 1);
            }
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

        float attackDelay = 0.5f;
        float timerDelay = 0.5f;

        #endregion

        void Inputs(float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;

            //bool falling = isFalling;

            Vector2 acceleration = new Vector2(0, 0);

            timerDelay -= deltaTime;
            if (ActionOne() == true)
            {
                if (timerDelay <= 0)
                {
                    SwingSword();
                    ReactionOne();
                    swordSwingInstance.Play();
                    timerDelay = attackDelay;
                }
            }

            if (KeyRotateLeft() == true)
            {
                rotation -= Deg2Rad(5);
            }
            else if (KeyRotateRight() == true)
            {
                rotation += Deg2Rad(5);
            }

            if (KeyLeft() == true)
            {
                acceleration.X -= Game1.accelerationX;
                sprite.SetFlipped(true);
                sprite.Play();
                swingDirection = swordDirection.Left;
            }
            else if (wasMovingLeft == true)
            {
                acceleration.X += Game1.frictionX;
            }

            if (KeyRight() == true)
            {
                acceleration.X += Game1.accelerationX;
                sprite.SetFlipped(false);
                sprite.Play();
                swingDirection = swordDirection.Right;
            }
            else if (wasMovingRight == true)
            {
                acceleration.X -= Game1.frictionX;
            }

            bool wasMovingUp = velocity.Y < 0;
            bool wasMovingDown = velocity.Y > 0;

            //bool falling = isFalling;

            if (KeyUp() == true)
            {
                acceleration.Y -= Game1.accelerationY;
                //sprite.SetFlipped(true);
                sprite.Play();
                swingDirection = swordDirection.Up;
            }
            else if (wasMovingUp == true)
            {
                acceleration.Y += Game1.frictionY;
            }

            if (KeyDown() == true)
            {
                acceleration.Y += Game1.accelerationY;
                sprite.SetFlipped(false);
                sprite.Play();
                swingDirection = swordDirection.Down;
            }
            else if (wasMovingDown == true)
            {
                acceleration.Y -= Game1.frictionY;
            }

            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true && this.isJumping == false && falling == false || autoJump == true)
            {
                autoJump = false;
                acceleration.Y -= Game1.jumpImpulse;
                this.isJumping = true;
                jumpSoundInstance.Play();
            }
            */

            velocity += acceleration * deltaTime;

            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            sprite.position += velocity * deltaTime;

            if ((wasMovingLeft && (velocity.X > 0)) || (wasMovingRight && (velocity.X < 0)))
            {
                velocity.X = 0;
                sprite.Pause();
            }
            if ((wasMovingUp && (velocity.Y > 0)) || (wasMovingDown && (velocity.Y < 0)))
            {
                velocity.Y = 0;
                sprite.Pause();
            }
        }

        void CollisionDetection()
        {
            //Console.WriteLine("IS RUNNING?");
            // collision detection
            // Our collision detection logic is greatly simplified by the fact that 
            // the player is a rectangle and is exactly the same size as a single tile.
            // So we know that the player can only ever occupy 1, 2 or 4 cells.
            // This means we can short-circuit and avoid building a general purpose 
            // collision detection engine by simply looking at the 1 to 4 cells that 
            // the player occupies:
            int tx = game.PixelToTile(Position.X);
            int ty = game.PixelToTile(Position.Y);
            // nx = true if player overlaps right
            bool nx = (Position.X) % Game1.tile != 0;
            // ny = true if player overlaps below
            bool ny = (Position.Y) % Game1.tile != 0;
            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            //Console.WriteLine(game.CellAtTileCoord(tx + 1, ty));

            // If the player has vertical velocity, then check to see if they have hit
            // a platform below or above, in which case, stop their vertical velocity, 
            // and clamp their y position:
            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    // clamp the y position to avoid falling into platform below
                    sprite.position.Y = MathHelper.Lerp(sprite.position.Y, game.TileToPixel(ty), 0.5f);
                    this.velocity.Y = 0;        // stop downward velocity
                    //this.isFalling = false;     // no longer falling
                    //this.isJumping = false;     // (or jumping)
                    ny = false;                 // - no longer overlaps the cells below
                }
            }
            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    // clamp the y position to avoid jumping into platform above
                    sprite.position.Y = MathHelper.Lerp(sprite.position.Y, game.TileToPixel(ty + 1), .5f);
                    this.velocity.Y = 0;   // stop upward velocity
                                           // player is no longer really in that cell, we clamped them 
                                           // to the cell below
                    cell = celldown;
                    cellright = celldiag;  // (ditto)
                    ny = false;            // player no longer overlaps the cells below
                }
            }

            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    // clamp the x position to avoid moving into the platform 
                    // we just hit
                    sprite.position.X = MathHelper.Lerp(sprite.position.X, game.TileToPixel(tx), .5f);
                    this.velocity.X = 0;      // stop horizontal velocity
                    sprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    // clamp the x position to avoid moving into the platform 
                    // we just hit
                    sprite.position.X = MathHelper.Lerp(sprite.position.X, game.TileToPixel(tx + 1), .5f);
                    this.velocity.X = 0;      // stop horizontal velocity
                    sprite.Pause();
                }
            }

            // The last calculation for our update() method is to detect if the 
            // player is now falling or not. We can do that by looking to see if 
            // there is a platform below them
            //this.isFalling = !(celldown || (nx && celldiag));
        }

        enum swordDirection
        {
            Up,
            Down,
            Left,
            Right
        }
        swordDirection swingDirection = swordDirection.Right;

        float Deg2Rad(float Deg)
        {
            float Rad = Deg;
            Rad = Rad * (float)Math.PI / 180f;
            return Rad;
        }

        void SwingSword()
        {
            
            switch (swingDirection)
            {
                case swordDirection.Up:
                    animationSword.Rotation = Deg2Rad(0);
                    break;
                case swordDirection.Down:
                    animationSword.Rotation = Deg2Rad(180);
                    break;
                case swordDirection.Left:
                    animationSword.Rotation = Deg2Rad(270);
                    break;
                case swordDirection.Right:
                    animationSword.Rotation = Deg2Rad(90);
                    break;
                default:
                    break;
            }
        }

        void SwordLogic(float deltaTime)
        {
            SwingSword();


            switch (swingDirection)
            {
                case swordDirection.Up:
                    swordSprite.position = sprite.position + new Vector2(0,-10);
                    break;
                case swordDirection.Down:
                    swordSprite.position = sprite.position + new Vector2(0, 15);
                    break;
                case swordDirection.Left:
                    swordSprite.position = sprite.position + new Vector2(-10, 0);
                    break;
                case swordDirection.Right:
                    swordSprite.position = sprite.position + new Vector2(10, 7);
                    break;
                default:
                    swordSprite.position = sprite.position;
                    break;
            }
            
        }

    }
}
