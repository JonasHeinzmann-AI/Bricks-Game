using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bricks
{
    class Brick
    {
        public float X { get; set; } //x position of brick on screen
        public float Y { get; set; } //y position of brick on screen
        public float Width { get; set; } //width of brick
        public float Height { get; set; } //height of brick
        public bool Visible { get; set; } //does brick still exist?
        private Color color;

        private Texture2D imgBrick { get; set; }  //cached image of the brick
        private SpriteBatch spriteBatch;  //allows us to write on backbuffer when we need to draw self

        public Brick(float x, float y, Color color, SpriteBatch spriteBatch, GameContent gameContent)
        {
            X = x;
            Y = y;
            imgBrick = gameContent.imgBrick;
            Width = imgBrick.Width;
            Height = imgBrick.Height;
            this.spriteBatch = spriteBatch;
            Visible = true;
            this.color = color;
        }
        public void Draw()
        {
            if (Visible)
            {
                spriteBatch.Draw(imgBrick, new Vector2(X, Y), null, color, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            }
        }
    }
}
