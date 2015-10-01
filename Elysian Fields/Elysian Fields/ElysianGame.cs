﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

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
        List<SpriteObject> spriteList = new List<SpriteObject>();
        List<UI> listUI = new List<UI>();
        List<SpriteObject> MouseCursors = new List<SpriteObject>();
        Item dragItem = new Item();
        Stack<DamageObject> dmgDone = new Stack<DamageObject>();

        private int currentMouse = 0;

        SpriteFont font;

        private bool RightClicked = false;
        private bool LeftClicked = false;

        private int leftClickTime;

        private int attackTime;
        private int damageTime;

        private const int Direction_North = 1;
        private const int Direction_East = 2;
        private const int Direction_South = 3;
        private const int Direction_West = 4;

        private int Walking_Direction;
        private int TimeOfLastMovement;
        private Keys lastPressedKey;
        private Keys mostRecentKey;
        private bool Walking = false;
        //Player player1;



        public ElysianGame()
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

            map = new Map(new Coordinates(Window.ClientBounds.Width, Window.ClientBounds.Height));
            map.Players.Add(new Player("Aephirus", new Coordinates(0, 0), 150, 1));
            //player1 = new Player("Aephirus", new Coordinates(0, 0));

            for(int i = 0; i < 5; i++)
            {
                map.Creatures.Add(new Creature("Ghost" + i.ToString(), new Coordinates(Coordinates.Step * 2 + i * Coordinates.Step, 0), map.Players[0].ID, System.ConsoleColor.White, 100, i + 1));
            }

            //this.IsMouseVisible = true;

            base.Initialize();

            RightClicked = false;
            LeftClicked = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\player"), spriteList.Count + 1, Entity.CreatureEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile"), spriteList.Count + 1, Entity.TileEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\attackbox"), spriteList.Count + 1, Entity.UnknownEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile2"), spriteList.Count + 1, Entity.TileEntity));

            map.Items.Add(new Item("Sword of magicnezz", new Coordinates(13 * Coordinates.Step, 13 * Coordinates.Step), 1, 0, 60));

            listUI.Add(new UI(Content.Load<Texture2D>("Graphics\\fist"), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 25, Coordinates.Step * 0), "LHand"));

            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseRegular"), 1, Entity.UnknownEntity));
            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseDrag"), 2, Entity.UnknownEntity));

            map.Players[0].SpriteID = 1;
            
            for(int i = 0; i < map.Creatures.Count; i++)
            {
                map.Creatures[i].SpriteID = 1;
            }

            /*map.Tiles.Add(new Tile("grass", 2, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 2), 1, true));
            map.Tiles.Add(new Tile("grass", 4, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 3), 2, true));
            map.Tiles.Add(new Tile("grass", 4, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 4), 3, true));
            map.Tiles.Add(new Tile("grass", 2, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 5), 4, true));*/

            map.LoadMap("Content\\fields.map");

            font = Content.Load<SpriteFont>("EFont");

            Window.Title = "Elysian Fields";
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


            // For debug purposes: Window.Title = gameTime.TotalGameTime.Seconds.ToString();

            // TODO: Add your update logic here



            if (gameTime.TotalGameTime.TotalMilliseconds - TimeOfLastMovement > 250)
            {
                if (this.IsActive)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        mostRecentKey = Keys.Right;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        mostRecentKey = Keys.Left;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        mostRecentKey = Keys.Up;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        mostRecentKey = Keys.Down;
                    }
                }

                if (!Walking && mostRecentKey != lastPressedKey)
                {
                    if (mostRecentKey == Keys.Right)
                    {
                        Walking_Direction = Direction_East;
                        Walking = true;
                    }
                    else if (mostRecentKey == Keys.Left)
                    {
                        Walking_Direction = Direction_West;
                        Walking = true;
                    }
                    else if (mostRecentKey == Keys.Up)
                    {
                        Walking_Direction = Direction_North;
                        Walking = true;
                    }
                    else if (mostRecentKey == Keys.Down)
                    {
                        Walking_Direction = Direction_South;
                        Walking = true;
                    }
                    lastPressedKey = mostRecentKey;
                    mostRecentKey = Keys.None;
                }


                if (Walking && map.Players[0].Health > 0)
                {
                    if (Walking_Direction == Direction_East)
                    {
                        map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X + Coordinates.Step, map.Players[0].Position.Y));
                    }
                    else if (Walking_Direction == Direction_West)
                    {
                        map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X - Coordinates.Step, map.Players[0].Position.Y));
                    }
                    else if (Walking_Direction == Direction_South)
                    {
                        map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X, map.Players[0].Position.Y + Coordinates.Step));
                    }
                    else if (Walking_Direction == Direction_North)
                    {
                        map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X, map.Players[0].Position.Y - Coordinates.Step));
                    }
                    TimeOfLastMovement = (int) gameTime.TotalGameTime.TotalMilliseconds;
                    Walking = false;
                    mostRecentKey = Keys.None;
                    lastPressedKey = Keys.None;

                    if(map.Players[0].hasPath())
                    {
                        map.Players[0].ResetPath();
                    }
                }                
            }

            if (gameTime.TotalGameTime.TotalMilliseconds - TimeOfLastMovement > 250)
            {
                if (map.Players[0].hasPath())
                {
                    map.MovePlayer();
                    TimeOfLastMovement = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }

            if (Mouse.GetState().RightButton == ButtonState.Released && RightClicked)
            {
                RightClicked = false;
            }

            if (this.IsActive)
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed && !RightClicked)
                {
                    int x = Mouse.GetState().X, y = Mouse.GetState().Y;
                    int creatureID = GetCreatureByMousePosition(x, y).ID;
                    if (map.Players[0].TargetID == creatureID)
                    {
                        map.Players[0].TargetID = -1;
                    }
                    else
                    {
                        map.Players[0].TargetID = creatureID;
                    }
                    // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();
                    // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();
                    RightClicked = true;
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released && LeftClicked)
            {
                int mx = Mouse.GetState().X, my = Mouse.GetState().Y;
                int x = (mx / 32) * 32;
                int y = (my / 32) * 32;
                Coordinates target = new Coordinates(x, y);
                if (gameTime.TotalGameTime.TotalMilliseconds - leftClickTime < 200)
                {
                    if (map.IsTileWalkable(target))
                    {
                        map.GeneratePathFromCreature(map.Players[0], target);
                    }
                }
                else
                {
                    if(dragItem.ID != -1)
                    {
                        UI equipment = GetListUIByMousePosition(x, y);
                        if (equipment.ID == -1)
                        {
                            equipment = GetListUIByMousePosition(dragItem.Position.X, dragItem.Position.Y);
                            if(equipment.ID == -1)
                            { 
                                map.DragItem(dragItem, target);
                            }
                            else
                            {
                                map.UnequipItem(dragItem, equipment, target);
                            }
                        }
                        else
                        {
                            map.EquipItem(dragItem, equipment);
                        }
                    }
                }
                // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();*/
                // For debug purposes: Window.Title = x.ToString() + " " + y.ToString();
                LeftClicked = false;
                currentMouse = 0;
            }

            if (this.IsActive)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!LeftClicked)
                    {
                        int mx = Mouse.GetState().X, my = Mouse.GetState().Y;
                        int x = (mx / 32) * 32;
                        int y = (my / 32) * 32;
                        leftClickTime = (int) gameTime.TotalGameTime.TotalMilliseconds;
                        dragItem = GetItemByMousePosition(x, y);
                        LeftClicked = true;
                    }
                    else
                    {
                        if(gameTime.TotalGameTime.TotalMilliseconds - leftClickTime > 200)
                        {
                            currentMouse = 1;
                        }
                    }
                }
            }

            if (gameTime.TotalGameTime.Milliseconds % 250 == 0)
            {
                map.MoveCreatures();
            }

            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                map.GeneratePaths();// <- Remove this to make monsters move
            }
            if (gameTime.TotalGameTime.TotalMilliseconds - attackTime > 1000)
            {
                if (map.CanAttack(map.Players[0], map.GetCreatureByID(map.Players[0].TargetID)))
                {
                    int dmgDealt = map.PlayerAttack(map.Players[0]);
                    if (dmgDealt != -1)
                    {
                        dmgDone.Push(new DamageObject(map.GetCreatureByID(map.Players[0].TargetID), dmgDealt));
                        attackTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            Vector2 cursorPos = Mouse.GetState().Position.ToVector2();
           
            GraphicsDevice.Clear(Color.ForestGreen);

            // TODO: Add your drawing code here


            spriteBatch.Begin();

            DrawUI();

            for (int i = 0; i < map.Tiles.Count; i++)
            {
                spriteBatch.Draw(GetSpriteByID(map.Tiles[i].SpriteID), new Vector2((float)map.Tiles[i].Position.X, (float)map.Tiles[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            for(int i = 0; i < map.Items.Count; i++)
            {
                spriteBatch.Draw(GetSpriteByID(map.Items[i].SpriteID), new Vector2((float)map.Items[i].Position.X, (float)map.Items[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(GetSpriteByID(map.Players[0].SpriteID), new Vector2((float)map.Players[0].Position.X, (float)map.Players[0].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, map.Players[0].Name, new Vector2((float)map.Players[0].Position.X, (float)map.Players[0].Position.Y + Coordinates.Step), Color.Black);
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                if (map.Creatures[i].Health > 0)
                {
                    spriteBatch.Draw(GetSpriteByID(map.Creatures[i].SpriteID), new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, map.Creatures[i].Name, new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y + Coordinates.Step), Color.Black);

                    /* TODO: Draw all names on screen before drawing the monsters, that way their names won't block their friends' sprites */
                    if (map.Creatures[i].ID == map.Players[0].TargetID)
                    {
                        // Draw attackbox
                        spriteBatch.Draw(GetSpriteByID(3), new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        // Draw damage dealt
                        if (dmgDone.Count > 0)
                        {
                            DamageObject dmgDealt = dmgDone.Peek();
                            /*if (dmgDealt.creature.ID == map.Creatures[i].ID)
                            {*/
                                spriteBatch.DrawString(font, dmgDealt.damageDealt.ToString(), new Vector2((float)map.Creatures[i].Position.X + 5, (float)map.Creatures[i].Position.Y + 5), Color.Red);
                                //damageTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                            //}
                        }

                        if (gameTime.TotalGameTime.TotalMilliseconds - attackTime > 750)
                        {
                            if (dmgDone.Count > 0 && map.Players[0].TargetID != -1)
                            {
                                dmgDone.Pop();
                                //damageTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                            }
                        }
                    }
                }
            }


            spriteBatch.DrawString(font, "Experience: " + map.Players[0].Experience, new Vector2((float)Coordinates.Step * 25, (float)Coordinates.Step), Color.Black);
            spriteBatch.DrawString(font, "Health: " + map.Players[0].Health + " / " + map.Players[0].MaxHealth, new Vector2((float)Coordinates.Step * 25, (float)Coordinates.Step * 2), Color.Black);
            spriteBatch.DrawString(font, "Strength: " + map.Players[0].TotalStrength(), new Vector2((float)Coordinates.Step * 25, (float)Coordinates.Step * 3), Color.Black);

            spriteBatch.Draw(MouseCursors[currentMouse].Sprite, cursorPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawUI()
        {
            for (int i = 0; i < listUI.Count; i++)
            {
                spriteBatch.Draw(listUI[i].Sprite, new Vector2((float)listUI[i].Position.X, (float)listUI[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public Texture2D GetSpriteByID(int ID)
        {
            for(int i = 0; i < spriteList.Count; i++)
            {
                if(spriteList[i].ID == ID)
                {
                    return spriteList[i].Sprite;
                }

            }
            return spriteList[0].Sprite;
        }

        private Creature GetCreatureByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                tmpRect.X = map.Creatures[i].Position.X;
                tmpRect.Y = map.Creatures[i].Position.Y;
                if (tmpRect.Contains(x, y) && map.Creatures[i].Health > 0)
                {
                    return map.Creatures[i];
                }
            }
            return new Creature("null");
        }

        private Item GetItemByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < map.Items.Count; i++)
            {
                tmpRect.X = map.Items[i].Position.X;
                tmpRect.Y = map.Items[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return map.Items[i];
                }
            }
            return new Item();
        }

        private UI GetListUIByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < listUI.Count; i++)
            {
                tmpRect.X = listUI[i].Position.X;
                tmpRect.Y = listUI[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return listUI[i];
                }
            }
            return new UI();
        }
    }
}