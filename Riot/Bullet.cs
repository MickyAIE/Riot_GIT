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
    public class Bullet
    {
        // Changed
        Game1 game = null;

        Vector2 positon = Vector2.Zero;
        float speed = 100;
        float rotation = 0f; // In Radians

        public enum ShotFrom
        {
            enemy,
            player
        }
        public ShotFrom shotFrom;

        #region Sprites / Textures / AnimatedTextures

        Texture2D bulletTexture;
        public Sprite sprite = new Sprite();
        AnimatedTexture animationBullet = new AnimatedTexture(Vector2.Zero, 0, 1, 0);

        #endregion

        #region Methods

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

        public Bullet(Game1 game, Texture2D bulletTexture, float speed, Vector2 positon, float rotation, ShotFrom shot )
        {
            this.game = game;
            this.speed = speed;
            this.positon = positon;
            this.rotation = rotation;
            this.shotFrom = shot;
            this.bulletTexture = bulletTexture;
        }

        public void Load(ContentManager content)
        {
            //bulletTexture = content.Load<Texture2D>("PNG/Lasers/laserBlue01");
            animationBullet.Load(content, bulletTexture.ToString(), 1, 1);
            animationBullet.Origin = new Vector2(bulletTexture.Width / 2, bulletTexture.Height / 2);
            animationBullet.Rotation = rotation;
            sprite.Add(animationBullet, bulletTexture.Width / 2, bulletTexture.Height / 2);
            sprite.Pause();
        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);
            Project(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }

        void Project(float dt)
        {
            positon += Forward() * speed * dt;
            sprite.position = positon;
        }

    }
}
