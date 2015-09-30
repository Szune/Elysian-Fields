using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Elysian_Fields_Map_Editor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ElysianMapEditor : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<SpriteObject> spriteList = new List<SpriteObject>();
        private bool RightClicked = false;
        List<Tile> Tiles = new List<Tile>();

        SpriteObject currentSprite = new SpriteObject();

        public ElysianMapEditor()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = int.Parse(ConfigurationManager.AppSettings["ClientWidth"]);
            graphics.PreferredBackBufferHeight = int.Parse(ConfigurationManager.AppSettings["ClientHeight"]);
            graphics.ApplyChanges();
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

            this.IsMouseVisible = true;

            base.Initialize();

            RightClicked = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\player"), spriteList.Count + 1, Entity.CreatureEntity, new Coordinates((spriteList.Count + 1) * Coordinates.Step, Coordinates.Step * 23)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile"), spriteList.Count + 1, Entity.TileEntity, new Coordinates((spriteList.Count + 1) * Coordinates.Step, Coordinates.Step * 23)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\attackbox"), spriteList.Count + 1, Entity.UnknownEntity, new Coordinates((spriteList.Count + 1) * Coordinates.Step, Coordinates.Step * 23)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile2"), spriteList.Count + 1, Entity.TileEntity, new Coordinates((spriteList.Count + 1) * Coordinates.Step, Coordinates.Step * 23)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\save"), spriteList.Count + 1, Entity.UnknownEntity, new Coordinates((spriteList.Count + 1) * Coordinates.Step, Coordinates.Step * 23)));

            LoadMap("Content\\fields.map");

            Window.Title = "Elysian Fields - Map Editor";

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

        public void SaveMap()
        {
            string saveString = "";
            for(int i = 0; i < Tiles.Count; i++)
            {
                if (i == 0)
                    saveString = Tiles[i].Position.X + "|" + Tiles[i].Position.Y + "|" + Tiles[i].SpriteID;
                else
                    saveString += "," + Tiles[i].Position.X + "|" + Tiles[i].Position.Y + "|" + Tiles[i].SpriteID;
            }

            StreamWriter writer = new StreamWriter("Content\\fields.map");
            writer.WriteLine(saveString);
            writer.Close();

            Window.Title = saveString;
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

            // TODO: Add your update logic here

            if (Mouse.GetState().RightButton == ButtonState.Released && RightClicked)
            {
                RightClicked = false;
            }

            if (this.IsActive)
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed && !RightClicked)
                {
                    int x = Mouse.GetState().X, y = Mouse.GetState().Y;
                    currentSprite = GetToolbarByMousePosition(x, y);
                    if(currentSprite.ID == 5)
                    {
                        Window.Title = "Saving...";
                        SaveMap();
                        Window.Title = "Elysian Fields - Map Editor";
                    }
                    // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();
                    RightClicked = true;
                }
            }

            /*if (Mouse.GetState().LeftButton == ButtonState.Released && LeftClicked)
            {
                LeftClicked = false;
            }*/

            if (this.IsActive)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    int mx = Mouse.GetState().X, my = Mouse.GetState().Y;
                    int x = (mx / 32) * 32;
                    int y = (my / 32) * 32;
                    Coordinates target = new Coordinates(x, y);
                    if (currentSprite.ID != -1 && GetTileByMousePosition(x, y).ID == -1 && currentSprite.ID != 3 && currentSprite.ID != 5)
                    {
                        Tiles.Add(new Tile("grass", currentSprite.ID, target, 1));
                    }
                    else if(currentSprite.ID == 3)
                    {
                        int tmpint = GetTileIndexByMousePosition(x, y);
                        if (tmpint > -1)
                        {
                            Tiles.RemoveAt(tmpint);
                        }
                    }

                    // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();*/
                    // For debug purposes: Window.Title = x.ToString() + " " + y.ToString();
                }
            }

            base.Update(gameTime);
        }

        public void LoadMap(string mapn) // Redan kommenterad i PacMan - MapEditor
        {
            /*int SpawnX = 0;
            int SpawnY = 0;*/
            FileInfo file = new FileInfo(mapn);

            if (file.Exists)
            {
                StreamReader read = new StreamReader(mapn);

                string[] coordinateList = read.ReadLine().Split(",".ToCharArray());

                for (int i = 0; i < coordinateList.Length; i++)
                {
                    string[] Coordinates = coordinateList[i].Split("|".ToCharArray());
                    int x = int.Parse(Coordinates[0]);
                    int y = int.Parse(Coordinates[1]);
                    int spriteID = int.Parse(Coordinates[2]);

                    Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y), i, true));
                }

                /*
                    TODO: Change Players[0] to a variable ID to allow for multiplayer
                */

                //coordinateList = read.ReadLine().Split("|".ToCharArray());
                /*SpawnX = int.Parse(coordinateList[0]);
                SpawnY = int.Parse(coordinateList[1]);
                Players[0].Position = new Coordinates(SpawnX, SpawnY);
                Players[0].SpawnPosition = new Coordinates(SpawnX, SpawnY);*/

                read.Close();
            }
            else
            {
                Tiles.Add(new Tile("grass", 1, new Coordinates(0, 0), 0));
            }
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

            for (int i = 0; i < Tiles.Count; i++)
            {
                spriteBatch.Draw(GetSpriteByID(Tiles[i].SpriteID), new Vector2((float)Tiles[i].Position.X, (float)Tiles[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }


            /*spriteBatch.DrawString(font, "Experience: ", new Vector2((float)0, (float)736), Color.Black);*/
            /*spriteBatch.DrawString(font, "Health: ", new Vector2((float)0, (float)726), Color.Black);*/

            DrawToolbox();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawToolbox()
        {
            for(int i = 0; i < spriteList.Count; i++)
            {
                spriteBatch.Draw(spriteList[i].Sprite, new Vector2((float)spriteList[i].Position.X, (float)spriteList[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public Texture2D GetSpriteByID(int ID)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].ID == ID)
                {
                    return spriteList[i].Sprite;
                }

            }
            return spriteList[0].Sprite;
        }

        private Tile GetTileByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < Tiles.Count; i++)
            {
                tmpRect.X = Tiles[i].Position.X;
                tmpRect.Y = Tiles[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return Tiles[i];
                }
            }
            return new Tile("null");
        }

        private int GetTileIndexByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < Tiles.Count; i++)
            {
                tmpRect.X = Tiles[i].Position.X;
                tmpRect.Y = Tiles[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return i;
                }
            }
            return -1;
        }

        private SpriteObject GetToolbarByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < spriteList.Count; i++)
            {
                tmpRect.X = spriteList[i].Position.X;
                tmpRect.Y = spriteList[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return spriteList[i];
                }
            }
            return new SpriteObject("null");
        }
    }
}
