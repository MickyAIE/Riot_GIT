using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riot
{
    class Enemy
    {
        Game1 game = null;

        Vector2 velocity = Vector2.Zero;
        Vector2 positon = Vector2.Zero;
        float rotation = 0;

        #region KeepStats

        #endregion

        #region Sounds

        SoundEffect pewSound;
        SoundEffectInstance pew;

        #endregion

        #region Sprites / Textures / AnimatedTextures

        Texture2D bulletTexture;

        Texture2D enemyTexture;
        Sprite enemySprite = new Sprite();
        AnimatedTexture enemyAnimTexture = new AnimatedTexture(Vector2.Zero, 0, 1, 0);

        #endregion

        #region Methods

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Rectangle Bounds
        {
            get { return enemySprite.Bounds; }
        }
        public Vector2 Position
        {
            get
            {
                return enemySprite.position;
            }
            set
            {
                enemySprite.position = value;
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
            float rotation = this.rotation + Deg2Rad(90);
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

        float RotateTo(Vector2 pointTo)
        {
            float rot = 0;

            Vector2 direction = positon - pointTo;
            direction.Normalize();

            rot = (float)Math.Atan2((double)direction.Y, (double)direction.X);

            rot += MathHelper.ToRadians(90);

            return rot;
        }

        #endregion

        public Enemy(Game1 game)
        {
            this.game = game;
            velocity = Vector2.Zero;
            positon = Vector2.Zero;
            rotation = 0;
        }

        public void Load(ContentManager content)
        {
            bulletTexture = content.Load<Texture2D>("PNG/Lasers/laserRed01");
            enemyTexture = content.Load<Texture2D>("PNG/Enemies/enemyRed1");
            enemyAnimTexture.Load(content, "PNG/Enemies/enemyRed1", 1, 1);
            enemyAnimTexture.Origin = new Vector2(enemyTexture.Width / 2, enemyTexture.Height / 2);
            enemyAnimTexture.Depth = 1;
            enemySprite.Add(enemyAnimTexture, enemyTexture.Width / 2, enemyTexture.Height / 2);
            enemySprite.Pause();

            pewSound = content.Load<SoundEffect>("Bonus/sfx_laser2");
            pew = pewSound.CreateInstance();
        }

        public void Update(float deltaTime)
        {
            rotation = RotateTo(GetPlayer.Position);
            enemyAnimTexture.Rotation = rotation;

            positon = enemySprite.position;
            enemySprite.Update(deltaTime);
            Think(deltaTime);
            //CollisionDetection();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            enemySprite.Draw(spriteBatch);
            

            //spriteBatch.DrawRectangle(enemySprite.Bounds, Color.Red, 1);
        }

        #region Think Variables

        float attackDelay = 3f;
        float timerDelay = 3f;
        public Player GetPlayer { get; set; }
        bool shotLeft = false;

        #endregion

        void Think(float deltaTime)
        {
            timerDelay -= deltaTime;
            if (timerDelay <= 0)
            {
                if (shotLeft == false)
                {
                    Bullet bullet = new Bullet(game, bulletTexture, 300, Position, rotation + Deg2Rad(180), Bullet.ShotFrom.enemy);
                    bullet.Load(game.Content);
                    game.Bullets.Add(bullet);
                    shotLeft = true;
                }
                else
                {
                    Bullet bullet = new Bullet(game, bulletTexture, 300, new Vector2((Position.X + enemyTexture.Width - 5), Position.Y), rotation + Deg2Rad(180), Bullet.ShotFrom.enemy);
                    bullet.Load(game.Content);
                    game.Bullets.Add(bullet);
                    shotLeft = false;
                }

                Random rand = new Random();
                attackDelay = rand.Next(3,6);

                timerDelay = attackDelay;
            }

            Vector2 acceleration = new Vector2(0, 0);

            velocity = Forward() * 5 * Game1.maxVelocity * deltaTime;

            velocity += acceleration * deltaTime;

            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            enemySprite.position += velocity * deltaTime;
        }

    }
}
