using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bricks
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameContent gameContent;

        private Paddle paddle;
        private Wall wall;
        private GameBorder gameBorder;
        private Ball ball;
        private Ball staticBall;  //used to draw image next to remaining ball count at top of screen


        private int screenWidth = 0;
        private int screenHeight = 0;
        private MouseState oldMouseState;
        private KeyboardState oldKeyboardState;
        private bool readyToServeBall = true;
        private int ballsRemaining = 3;

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
            gameContent = new GameContent(Content);
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //set game to 502x700 or screen max if smaller
            if (screenWidth >= 502)
            {
                screenWidth = 502;
            }
            if (screenHeight >= 700)
            {
                screenHeight = 700;
            }
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            //create game objects
            int paddleX = (screenWidth - gameContent.imgPaddle.Width) / 2; //we'll center the paddle on the screen to start
            int paddleY = screenHeight - 100;  //paddle will be 100 pixels from the bottom of the screen
            paddle = new Paddle(paddleX, paddleY, screenWidth, spriteBatch, gameContent);  // create the game paddle
            wall = new Wall(1, 50, spriteBatch, gameContent);
            gameBorder = new GameBorder(screenWidth, screenHeight, spriteBatch, gameContent);
            ball = new Ball(screenWidth, screenHeight, spriteBatch, gameContent);
            staticBall = new Ball(screenWidth, screenHeight, spriteBatch, gameContent);
            staticBall.X = 25;
            staticBall.Y = 25;
            staticBall.Visible = true;
            staticBall.UseRotation = false;
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
        protected override void Update(GameTime gameTime)
        {
            if (IsActive==false)
            {
                return;  //our window is not active don't update
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            //process mouse move
            if (oldMouseState.X != newMouseState.X)
            {
                if (newMouseState.X >= 0 && newMouseState.X < screenWidth && newMouseState.Y >= 0 && newMouseState.Y < screenHeight)
                {
                    paddle.MoveTo(newMouseState.X);
                }
            }

            //process left-click
            if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed && oldMouseState.X == newMouseState.X && oldMouseState.Y == newMouseState.Y && readyToServeBall)
            {
                ServeBall();
            }

            //process keyboard events
            if (newKeyboardState.IsKeyDown(Keys.Left))
            {
                paddle.MoveLeft();
            }
            if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                paddle.MoveRight();
            }
            if (oldKeyboardState.IsKeyUp(Keys.Space) && newKeyboardState.IsKeyDown(Keys.Space) && readyToServeBall)
            {
                ServeBall();
            }

            oldMouseState = newMouseState; // this saves the old state
            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        private void ServeBall()
        {
            if (ballsRemaining < 1)
            {
                ballsRemaining = 3;
                ball.Score = 0;
                wall = new Wall(1, 50, spriteBatch, gameContent);
            }
            readyToServeBall = false;
            float ballX = paddle.X + (paddle.Width) / 2;
            float ballY = paddle.Y - ball.Height;
            ball.Launch(ballX, ballY, -3, -3);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            paddle.Draw();
            wall.Draw();
            base.Draw(gameTime);
            gameBorder.Draw();
            if (ball.Visible)
            {
                bool inPlay = ball.Move(wall, paddle);
                if (inPlay)
                {
                    ball.Draw();
                }
                else
                {
                    ballsRemaining--;
                    readyToServeBall = true;
                }
            }
            staticBall.Draw();

            string scoreMsg = "Score: " + ball.Score.ToString("00000");
            Vector2 space = gameContent.labelFont.MeasureString(scoreMsg);
            spriteBatch.DrawString(gameContent.labelFont, scoreMsg, new Vector2((screenWidth - space.X) / 2, screenHeight - 40), Color.White);
            if (ball.bricksCleared >= 70)
            {
                ball.Visible = false;
                ball.bricksCleared = 0;
                wall = new Wall(1, 50, spriteBatch, gameContent);
                readyToServeBall = true;
            }
            if (readyToServeBall)
            {
                if (ballsRemaining > 0)
                {
                    string startMsg = "Press <Space> or Click Mouse to Start";
                    Vector2 startSpace = gameContent.labelFont.MeasureString(startMsg);
                    spriteBatch.DrawString(gameContent.labelFont, startMsg, new Vector2((screenWidth - startSpace.X) / 2, screenHeight / 2), Color.White);
                }
                else
                {
                    string endMsg = "Game Over";
                    Vector2 endSpace = gameContent.labelFont.MeasureString(endMsg);
                    spriteBatch.DrawString(gameContent.labelFont, endMsg, new Vector2((screenWidth - endSpace.X) / 2, screenHeight / 2), Color.White);
                }
            }

            spriteBatch.DrawString(gameContent.labelFont, ballsRemaining.ToString(), new Vector2(40, 10), Color.White);

            spriteBatch.End();
        }
    }
}
