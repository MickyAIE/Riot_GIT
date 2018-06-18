using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riot
{
    class Zombie
    {
        Game1 game = null;

        Vector2 velocity = Vector2.Zero;
        Vector2 positon = Vector2.Zero;

        #region Sprites / Textures / AnimatedTextures

        Texture2D zombieTexture;
        Sprite sprite = new Sprite();
        AnimatedTexture animationZombie = new AnimatedTexture(Vector2.Zero, 0, 1, 0);

        #endregion

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

        public Zombie(Game1 game)
        {
            this.game = game;
            velocity = Vector2.Zero;
            positon = Vector2.Zero;
        }

        public void Load(ContentManager content)
        {
            zombieTexture = content.Load<Texture2D>("Zombie");
            animationZombie.Load(content, "Zombie", 1, 1);
            animationZombie.Origin = new Vector2(zombieTexture.Width / 2, zombieTexture.Height / 2);
            animationZombie.Depth = 1;
            sprite.Add(animationZombie, zombieTexture.Width / 2, zombieTexture.Height / 2);
            sprite.Pause();
        }

        public void Update(float deltaTime)
        {

            sprite.Update(deltaTime);
            Think(deltaTime);
            //CollisionDetection();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
            spriteBatch.DrawRectangle(sprite.Bounds, Color.Red, 1);
        }

        #region Think Variables

        float attackDelay = 0.5f;
        float timerDelay = 0.5f;
        public Player GetPlayer { get; set; }

        #endregion

        void Think(float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool wasMovingUp = velocity.Y < 0;
            bool wasMovingDown = velocity.Y > 0;

            Vector2 acceleration = new Vector2(0, 0);

            timerDelay -= deltaTime;

            Vector2 direction;

            direction = GetPlayer.Position - Position;
            direction.Normalize();

            velocity = direction * 5 * Game1.maxVelocity * deltaTime;

            //velocity += acceleration * deltaTime;

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

            if (game.IsColliding(sprite.Bounds,GetPlayer.swordSprite.Bounds) == true)
            {
                Console.WriteLine("SWORD IS IN ME");
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
            int tx = game.PixelToTile(sprite.position.X);
            int ty = game.PixelToTile(sprite.position.Y);
            // nx = true if player overlaps right
            bool nx = (sprite.position.X) % Game1.tile == 0;
            // ny = true if player overlaps below
            bool ny = (sprite.position.Y) % Game1.tile == 0;
            bool cell = game.CellAtTileCoord(tx, ty) == 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) == 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) == 0;
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
                    sprite.position.Y = game.TileToPixel(ty);
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
                    sprite.position.Y = game.TileToPixel(ty + 1);
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
                    sprite.position.X = game.TileToPixel(tx);
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
                    sprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;      // stop horizontal velocity
                    sprite.Pause();
                }
            }

            // The last calculation for our update() method is to detect if the 
            // player is now falling or not. We can do that by looking to see if 
            // there is a platform below them
            //this.isFalling = !(celldown || (nx && celldiag));
        }
    }
}
