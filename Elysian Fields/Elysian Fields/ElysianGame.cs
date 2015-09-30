using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Elysian_Fields
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ElysianGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map map;
        //Player player1;
        
        

        public ElysianGame()
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

            map = new Map(new Coordinates(Window.ClientBounds.Width, Window.ClientBounds.Height));
            map.Players.Add(new Player("Aephirus", new Coordinates(0, 0)));
            //player1 = new Player("Aephirus", new Coordinates(0, 0));

            for(int i = 0; i < 5; i++)
            {
                map.Creatures.Add(new Creature("Ghost" + i.ToString(), new Coordinates(64 + i * 32, 0), map.Players[0].ID, System.ConsoleColor.White, 1, i + 1));
            }
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

            map.Players[0].Sprite = Content.Load<Texture2D>("Graphics\\player");

            for(int i = 0; i < map.Creatures.Count; i++)
            {
                map.Creatures[i].Sprite = Content.Load<Texture2D>("Graphics\\player");
            }

            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            Window.Title = gameTime.TotalGameTime.Seconds.ToString();

            // TODO: Add your update logic here
            if (gameTime.TotalGameTime.Milliseconds % 250 == 0)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    map.Players[0].Position = new Coordinates(map.Players[0].Position.X + 32, map.Players[0].Position.Y);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    map.Players[0].Position = new Coordinates(map.Players[0].Position.X - 32, map.Players[0].Position.Y);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    map.Players[0].Position = new Coordinates(map.Players[0].Position.X, map.Players[0].Position.Y + 32);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    map.Players[0].Position = new Coordinates(map.Players[0].Position.X, map.Players[0].Position.Y - 32);
                }

                //map.MoveCreatures();

            }

            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                //map.GeneratePaths();
            }

                base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            SpriteFont font;
            font = Content.Load<SpriteFont>("EFont");
           
            GraphicsDevice.Clear(Color.ForestGreen);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            spriteBatch.Draw(map.Players[0].Sprite, new Vector2((float)map.Players[0].Position.X, (float)map.Players[0].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, map.Players[0].Name, new Vector2((float)map.Players[0].Position.X, (float)map.Players[0].Position.Y + 32), Color.Black);
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                spriteBatch.Draw(map.Creatures[i].Sprite, new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, map.Creatures[i].Name, new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y + 32), Color.Black);
            }
            

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
