using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Elysian_Fields_Map_Editor.Modules.Controls;

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
        List<Textbox> Textboxes = new List<Textbox>();

        private bool RightClicked = false;
        private bool LeftClicked = false;
        List<Tile> Tiles = new List<Tile>();
        private int currentZ = 0;
        private int currentZ_order = 0;

        private int currentTileset = 0;

        private const int No_Focused_Textbox = -1;

        private int FocusedTextbox = No_Focused_Textbox;

        private MouseState oldMousePos;
        private KeyboardState lastKeyPress;
        private int lastKeyHash;

        private string currentAction = "None";

        private const int Tileset_Tiles = 0;
        private const int Tileset_Items = 1;
        private const int Tileset_Creatures = 2;

        private Coordinates lastTouchedPosition = new Coordinates(0, 0, 0);

        private int TimeOfLastMovement = 0;
        private int TimeOfLastKeyPress = 0;

        private const string Tileset_Tiles_Text = "Tiles";
        private const string Tileset_Items_Text = "Items";
        private const string Tileset_Creatures_Text = "Creatures";

        private int LastRemovedTime = 0;

        private Coordinates CurrentPosition = new Coordinates(12, 8);

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

            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\Error"), 61000, Entity.ThirtyTwoByThirtyTwoButton, "ErrorSprite"));

            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MapEditorBackground"), 75000, Entity.UnknownEntity, "Background"));

            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TextboxStart"), 40000, Entity.UnknownEntity, "TextboxStart"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TextboxMiddle"), 41000, Entity.UnknownEntity, "TextboxMiddle"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TextboxEnd"), 42000, Entity.UnknownEntity, "TextboxEnd"));

            Textboxes.Add(new Textbox(0, "GoToX", 40000, 41000, 42000, "", 50, new Coordinates((Coordinates.UI_Step * 29) + 20, (Coordinates.UI_Step * 3) - 1)));
            Textboxes.Add(new Textbox(1, "GoToY", 40000, 41000, 42000, "", 50, new Coordinates((Coordinates.UI_Step * 29) + 20, (Coordinates.UI_Step * 3) + 14)));


            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\save"), 50000, Entity.ThirtyTwoByThirtyTwoButton, "SaveButton", new Coordinates(29 * Coordinates.UI_Step, 7 * Coordinates.UI_Step)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\attackbox"), 60000, Entity.ThirtyTwoByThirtyTwoButton, "Attackbox", new Coordinates(29 * Coordinates.UI_Step, 8 * Coordinates.UI_Step)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseRegular"), 72000, Entity.ThirtyTwoByThirtyTwoButton, "MouseRegular", new Coordinates(29 * Coordinates.UI_Step, 9 * Coordinates.UI_Step)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UpArrow"), 51000, Entity.TenByTenButton, "Tileset_Up", new Coordinates((Coordinates.UI_Step * 27) + 20, (Coordinates.UI_Step * 6) + 8)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\DownArrow"), 52000, Entity.TenByTenButton, "Tileset_Down", new Coordinates((Coordinates.UI_Step * 26) + 5, (Coordinates.UI_Step * 6) + 8)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UpArrow"), 53000, Entity.TenByTenButton, "Z_Up", new Coordinates((Coordinates.UI_Step * 27) + 20, (Coordinates.UI_Step * 3) + 20)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\DownArrow"), 54000, Entity.TenByTenButton, "Z_Down", new Coordinates((Coordinates.UI_Step * 26) + 5, (Coordinates.UI_Step * 3) + 20)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UpArrow"), 55000, Entity.TenByTenButton, "Z_Up", new Coordinates((Coordinates.UI_Step * 27) + 52, (Coordinates.UI_Step * 3) + 5)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\DownArrow"), 56000, Entity.TenByTenButton, "Z_Down", new Coordinates((Coordinates.UI_Step * 26) + 5, (Coordinates.UI_Step * 3) + 5)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\DownArrow"), 57000, Entity.TenByTenButton, "GoToCoordinates", new Coordinates((Coordinates.UI_Step * 30) + 10, (Coordinates.UI_Step * 2) + 20)));


            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\player"), 1, Entity.UnknownEntity, "Player"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\Grass"), 2, Entity.TileEntity, "Grass"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile2"), 4, Entity.TileEntity, "Tiles"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_ui"), 5, Entity.UnknownEntity, "UI_FistSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_animation"), 6, Entity.UnknownEntity, "Spell_FistSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_ui"), 7, Entity.UnknownEntity, "UI_HealSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_animation"), 8, Entity.UnknownEntity, "Spell_HealSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\goldenarmor"), 9, Entity.ItemEntity, "Golden Armor"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\helmet"), 10, Entity.UnknownEntity, "helm"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\torso"), 11, Entity.UnknownEntity, "arm"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fist"), 12, Entity.UnknownEntity, "lhand"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fist"), 13, Entity.UnknownEntity, "rhand"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\hornedhelmet"), 14, Entity.ItemEntity, "Horned Helmet"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UI_background"), 15, Entity.UnknownEntity, "UI_background"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\magicsword"), 16, Entity.ItemEntity, "Magic Sword"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\ghost"), 17, Entity.CreatureEntity, "Ghost"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\legs"), 18, Entity.UnknownEntity, "legs"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\masterlegs"), 19, Entity.ItemEntity, "Master Legs"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\ui_bag"), 20, Entity.UnknownEntity, "bagslot"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\backpack"), 21, Entity.ItemEntity, "Backpack"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\backpackbackgrund"), 22, Entity.UnknownEntity, "UI_BackpackBackground"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\CloseButton"), 23, Entity.UnknownEntity, "UI_CloseButton"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UpArrow"), 24, Entity.UnknownEntity, "UI_UpArrow"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\DownArrow"), 25, Entity.UnknownEntity, "UI_DownArrow"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\Scrollbar"), 26, Entity.UnknownEntity, "UI_Scrollbar"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenWall"), 27, Entity.TileEntity, "WoodWall"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenWall2"), 28, Entity.TileEntity, "WoodWall2"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TiledRoof"), 29, Entity.TileEntity, "TiledRoof"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenStairsDown"), 30, Entity.TileEntity, "WStairsDown"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenStairsUp"), 31, Entity.TileEntity, "WStairsUp"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenFloor"), 32, Entity.TileEntity, "WFloor"));

            LoadMap("Content\\fields.map");

            Window.Title = "Elysian Fields - Map Editor";
            currentSprite.ID = 72000;
            lastKeyHash = 0;
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
                    saveString = Tiles[i].Position.X + "|" + Tiles[i].Position.Y + "|" + Tiles[i].Position.Z + "|" + Tiles[i].SpriteID + "|" + Tiles[i].Z_order; // TODO: Change 0 to z-order on map
                else
                    saveString += "," + Tiles[i].Position.X + "|" + Tiles[i].Position.Y + "|" + Tiles[i].Position.Z + "|" + Tiles[i].SpriteID + "|" + Tiles[i].Z_order; // TODO: Change 0 to z-order on map
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



            DoKeyboardEvents(gameTime);
            DoMouseEvents(gameTime);

            UpdateAction();


            Window.Title = "Sprite ID: " + currentSprite.ID + " | Focused textbox: " + GetTextboxByID(FocusedTextbox).Name + " | Tile count: " + Tiles.Count + " | Last touched position: " + lastTouchedPosition.X + ", " + lastTouchedPosition.Y + ", " + lastTouchedPosition.Z + " | Action: " + currentAction;

            base.Update(gameTime);
        }

        private void UpdateAction()
        {
            if (currentSprite.ID == 72000)
            {
                currentAction = "Moving map";
            }
            else if (currentSprite.ID == 60000)
            {
                currentAction = "Removing tiles";
            }
            else if (currentSprite.ID == 50000)
            {
                currentAction = "Saved map";
            }
            else if (currentSprite.ID > -1 && currentSprite.ID < 40000)
            {
                currentAction = "Tile placement";
            }
            else
            {
                currentAction = "None";
            }
        }

        private void DoMouseEvents(GameTime gameTime)
        {
            ResetMouseStates();
            DoRightClickEvents(gameTime);
            DoLeftClickEvents(gameTime);

        }

        private void ResetMouseStates()
        {
            if (Mouse.GetState().RightButton == ButtonState.Released && RightClicked)
            {
                RightClicked = false;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released && LeftClicked)
            {
                LeftClicked = false;
            }
        }

        private void DoRightClickEvents(GameTime gameTime)
        {
            if (this.IsActive)
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed && !RightClicked)
                {
                    bool tool = false;
                    int x = Mouse.GetState().X, y = Mouse.GetState().Y;
                    SpriteObject tmpSprite = GetToolbarByMousePosition(x, y);
                    if (tmpSprite.ID == 50000)
                    {
                        //Window.Title = "Saving...";
                        SaveMap();
                        //Window.Title = "Elysian Fields - Map Editor";
                    }
                    else if (tmpSprite.ID == 51000)
                    {
                        tool = true;
                        if (currentTileset < 2)
                        {
                            if (currentTileset == Tileset_Items)
                            {
                                ClearTilesetPositions(Entity.ItemEntity);
                            }
                            else if (currentTileset == Tileset_Tiles)
                            {
                                ClearTilesetPositions(Entity.TileEntity);
                            }
                            else if (currentTileset == Tileset_Creatures)
                            {
                                ClearTilesetPositions(Entity.CreatureEntity);
                            }
                            currentTileset++;
                        }
                    }
                    else if (tmpSprite.ID == 52000)
                    {
                        tool = true;
                        if (currentTileset > 0)
                        {
                            if (currentTileset == Tileset_Items)
                            {
                                ClearTilesetPositions(Entity.ItemEntity);
                            }
                            else if (currentTileset == Tileset_Tiles)
                            {
                                ClearTilesetPositions(Entity.TileEntity);
                            }
                            else if (currentTileset == Tileset_Creatures)
                            {
                                ClearTilesetPositions(Entity.CreatureEntity);
                            }
                            currentTileset--;
                        }
                    }
                    else if (tmpSprite.ID == 53000)
                    {
                        tool = true;
                        if (currentZ < Utility.MaxZ)
                            currentZ++;
                    }
                    else if (tmpSprite.ID == 54000)
                    {
                        tool = true;
                        if (currentZ > Utility.MinZ)
                            currentZ--;
                    }
                    else if (tmpSprite.ID == 55000)
                    {
                        tool = true;
                        if (currentZ_order < Utility.MaxZ_Order)
                            currentZ_order++;
                    }
                    else if (tmpSprite.ID == 56000)
                    {
                        tool = true;
                        if (currentZ_order > Utility.MinZ_Order)
                            currentZ_order--;
                    }

                    if (!tool)
                    {
                        currentSprite = tmpSprite;
                    }

                    Window.Title = "Sprite ID: " + currentSprite.ID.ToString();
                    RightClicked = true;
                }
            }
        }

        private void DoLeftClickEvents(GameTime gameTime)
        {
            if (this.IsActive)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && LeftClicked && currentSprite.ID == 72000)
                {
                    //GetTextboxByName("GoToX").Text = "Moving";
                    int newPosX = (oldMousePos.Position.X - Mouse.GetState().Position.X) / 10;
                    int newPosY = (oldMousePos.Position.Y - Mouse.GetState().Position.Y) / 10;
                    if (!OutOfBoundaries(new Coordinates(CurrentPosition.X + newPosX, CurrentPosition.Y + newPosY)))
                    {
                        CurrentPosition.X += newPosX;
                        CurrentPosition.Y += newPosY;
                    }

                    oldMousePos = Mouse.GetState();
                }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !LeftClicked)
                {
                    int x = Mouse.GetState().X, y = Mouse.GetState().Y;
                    SpriteObject tmpSprite = GetToolbarByMousePosition(x, y);
                    if (tmpSprite.ID == 51000)
                    {
                        if (currentTileset < 2)
                        {
                            if (currentTileset == Tileset_Items)
                            {
                                ClearTilesetPositions(Entity.ItemEntity);
                            }
                            else if (currentTileset == Tileset_Tiles)
                            {
                                ClearTilesetPositions(Entity.TileEntity);
                            }
                            else if (currentTileset == Tileset_Creatures)
                            {
                                ClearTilesetPositions(Entity.CreatureEntity);
                            }
                            currentTileset++;
                        }
                    }
                    else if (tmpSprite.ID == 52000)
                    {
                        if (currentTileset > 0)
                        {
                            if (currentTileset == Tileset_Items)
                            {
                                ClearTilesetPositions(Entity.ItemEntity);
                            }
                            else if (currentTileset == Tileset_Tiles)
                            {
                                ClearTilesetPositions(Entity.TileEntity);
                            }
                            else if (currentTileset == Tileset_Creatures)
                            {
                                ClearTilesetPositions(Entity.CreatureEntity);
                            }
                            currentTileset--;
                        }
                    }
                    else if (tmpSprite.ID == 53000)
                    {
                        if (currentZ < Utility.MaxZ)
                            currentZ++;
                    }
                    else if (tmpSprite.ID == 54000)
                    {
                        if (currentZ > Utility.MinZ)
                            currentZ--;
                    }
                    else if (tmpSprite.ID == 55000)
                    {
                        if (currentZ_order < Utility.MaxZ_Order)
                            currentZ_order++;
                    }
                    else if (tmpSprite.ID == 56000)
                    {
                        if (currentZ_order > Utility.MinZ_Order)
                            currentZ_order--;
                    }
                    else if (tmpSprite.ID == 57000)
                    {
                        int textX = int.Parse(GetTextboxByName("GoToX").Text);
                        int textY = int.Parse(GetTextboxByName("GoToY").Text);
                        if (textX < 12) { textX = 12; }
                        if (textY < 8) { textY = 8; }
                        CurrentPosition = new Coordinates(textX, textY, currentZ);
                    }

                    Textbox clickedTextbox = GetTextboxByMousePosition(x, y);
                    if (clickedTextbox.ID != -1)
                    {
                        FocusedTextbox = clickedTextbox.ID;
                    }
                    else
                    {
                        FocusedTextbox = No_Focused_Textbox;
                    }

                    Window.Title = FocusedTextbox.ToString();

                    LeftClicked = true;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    int mx = Mouse.GetState().X, my = Mouse.GetState().Y;
                    oldMousePos = Mouse.GetState();
                    int x = (mx / 32) * 32;
                    int y = (my / 32) * 32;
                    int z = currentZ;
                    int newx = CurrentPosition.X + (x / 32) - 12;
                    int newy = CurrentPosition.Y + (y / 32) - 8;
                    //Window.Title = "Total number of tiles: " + Tiles.Count + " Coordinates of last touched tile: " + newx + ", " + newy;
                    lastTouchedPosition = new Coordinates(newx, newy, currentZ);
                    if (currentSprite.ID != -1 && GetTileByMousePosition(x, y).ID == -1 && currentSprite.ID < 49000 && x < Coordinates.UI_Step * 26)
                    {
                        if (GetSpriteByID(currentSprite.ID).Height == 64 && GetSpriteByID(currentSprite.ID).Width != 64)
                        {
                            Tiles.Add(new Tile("grass", currentSprite.ID, new Coordinates(newx, newy, z), new Coordinates(newx, newy - 1, z), Tiles.Count, true, false, currentZ_order));
                        }
                        else if (GetSpriteByID(currentSprite.ID).Width == 64 && GetSpriteByID(currentSprite.ID).Height != 64)
                        {
                            Tiles.Add(new Tile("grass", currentSprite.ID, new Coordinates(newx, newy, z), new Coordinates(newx - 1, newy, z), Tiles.Count, true, false, currentZ_order));
                        }
                        else if (GetSpriteByID(currentSprite.ID).Width == 64 && GetSpriteByID(currentSprite.ID).Height == 64)
                        {
                            Tiles.Add(new Tile("grass", currentSprite.ID, new Coordinates(newx, newy, z), new Coordinates(newx - 1, newy - 1, z), Tiles.Count, true, false, currentZ_order));
                        }
                        else
                        {
                            Tiles.Add(new Tile("grass", currentSprite.ID, new Coordinates(newx, newy, z), new Coordinates(newx, newy, z), Tiles.Count, true, false, currentZ_order));
                        }
                    }
                    else if (currentSprite.ID == 60000)
                    {
                        int tmpint = GetTileIndexByMousePosition(x, y);
                        if (tmpint > -1)
                        {
                            if (LastRemovedTime + 500 < gameTime.TotalGameTime.TotalMilliseconds)
                            {
                                Tiles.Remove(GetTopItemFromTile(new Coordinates(newx, newy, z)));
                                LastRemovedTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                            }
                        }
                    }

                    // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();*/
                    // For debug purposes: Window.Title = x.ToString() + " " + y.ToString();
                }
            }
        }

        private void DoKeyboardEvents(GameTime gameTime)
        {
            // TODO: Put all keyboard checks in here

            if (Keyboard.GetState().GetPressedKeys().Length < 1)
            {
                lastKeyHash = -1;
            }

            if (FocusedTextbox != No_Focused_Textbox)
            {
                if ((gameTime.TotalGameTime.TotalMilliseconds > TimeOfLastKeyPress + 5 && lastKeyPress.GetHashCode() != Keyboard.GetState().GetHashCode()) || (gameTime.TotalGameTime.TotalMilliseconds > TimeOfLastKeyPress + 1050 && lastKeyPress.GetHashCode() == Keyboard.GetState().GetHashCode()))
                {
                    lastKeyPress = Keyboard.GetState();
                    Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                    if (pressedKeys.Length > 0 && lastKeyHash != pressedKeys[0].GetHashCode())
                    {
                        if (pressedKeys[0] == Keys.Back)
                        {
                            string tmpText = GetTextboxByID(FocusedTextbox).Text;
                            if (tmpText.Length > 0)
                            {
                                GetTextboxByID(FocusedTextbox).Text = tmpText.Substring(0, tmpText.Length - 1);
                            }
                        }
                        /*else if(pressedKeys[i] == Keys.Space)
                        {
                            GetTextboxByID(FocusedTextbox).Text += " ";
                        }*/
                        else
                        {
                            if (GetTextboxByID(FocusedTextbox).Text.Length < GetTextboxByID(FocusedTextbox).MaxChars)
                            {
                                if ((pressedKeys[0].GetHashCode() > 95 && pressedKeys[0].GetHashCode() < 106))
                                {
                                    GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString().Substring(6);
                                }
                                else if ((pressedKeys[0].GetHashCode() > 47 && pressedKeys[0].GetHashCode() < 58))
                                {
                                    GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString().Substring(1);
                                }
                            }
                            lastKeyHash = pressedKeys[0].GetHashCode();
                        }
                        TimeOfLastKeyPress = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    }
                }
                else if (gameTime.TotalGameTime.TotalMilliseconds > TimeOfLastKeyPress + 450 && lastKeyPress.GetHashCode() == Keyboard.GetState().GetHashCode())
                {
                    lastKeyPress = Keyboard.GetState();
                    Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                    if (pressedKeys.Length > 0)
                    {
                        if (pressedKeys[0] == Keys.Back)
                        {
                            string tmpText = GetTextboxByID(FocusedTextbox).Text;
                            if (tmpText.Length > 0)
                            {
                                GetTextboxByID(FocusedTextbox).Text = tmpText.Substring(0, tmpText.Length - 1);
                            }
                        }
                        //else if(pressedKeys[i] == Keys.Space)
                        //{
                        //    GetTextboxByID(FocusedTextbox).Text += " ";
                        //}
                        else
                        {
                            if (GetTextboxByID(FocusedTextbox).Text.Length < GetTextboxByID(FocusedTextbox).MaxChars)
                            {
                                if ((pressedKeys[0].GetHashCode() > 95 && pressedKeys[0].GetHashCode() < 106))
                                {
                                    GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString().Substring(6);
                                }
                                else if ((pressedKeys[0].GetHashCode() > 47 && pressedKeys[0].GetHashCode() < 58))
                                {
                                    GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString().Substring(1);
                                }
                            }
                        }
                        lastKeyHash = pressedKeys[0].GetHashCode();
                    }
                    if (lastKeyPress.GetHashCode() != Keyboard.GetState().GetHashCode())
                        TimeOfLastKeyPress = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }


            if (gameTime.TotalGameTime.TotalMilliseconds - TimeOfLastMovement > 250)
            {
                if (this.IsActive)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        if (!OutOfBoundaries(new Coordinates(CurrentPosition.X + 1, CurrentPosition.Y)))
                        {
                            CurrentPosition = new Coordinates(CurrentPosition.X + 1, CurrentPosition.Y);
                            TimeOfLastMovement = (int)gameTime.TotalGameTime.TotalMilliseconds;
                        }
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        if (!OutOfBoundaries(new Coordinates(CurrentPosition.X - 1, CurrentPosition.Y)))
                        {
                            CurrentPosition = new Coordinates(CurrentPosition.X - 1, CurrentPosition.Y);
                            TimeOfLastMovement = (int)gameTime.TotalGameTime.TotalMilliseconds;
                        }
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        if (!OutOfBoundaries(new Coordinates(CurrentPosition.X, CurrentPosition.Y - 1)))
                        {
                            CurrentPosition = new Coordinates(CurrentPosition.X, CurrentPosition.Y - 1);
                            TimeOfLastMovement = (int)gameTime.TotalGameTime.TotalMilliseconds;
                        }
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        if (!OutOfBoundaries(new Coordinates(CurrentPosition.X, CurrentPosition.Y + 1)))
                        {
                            CurrentPosition = new Coordinates(CurrentPosition.X, CurrentPosition.Y + 1);
                            TimeOfLastMovement = (int)gameTime.TotalGameTime.TotalMilliseconds;
                        }
                    }
                }
            }
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
                    int z = int.Parse(Coordinates[2]);
                    int spriteID = int.Parse(Coordinates[3]);
                    int z_order = int.Parse(Coordinates[4]);

                    if (GetSpriteByID(spriteID).Height == 64 && GetSpriteByID(spriteID).Width != 64)
                    {
                        Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x, y - 1, z), i, true, false, z_order));
                    }
                    else if (GetSpriteByID(spriteID).Width == 64 && GetSpriteByID(spriteID).Height != 64)
                    {
                        Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x - 1, y, z), i, true, false, z_order));
                    }
                    else if (GetSpriteByID(spriteID).Height == 64 && GetSpriteByID(spriteID).Width == 64)
                    {
                        Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x - 1, y - 1, z), i, true, false, z_order));
                    }
                    else
                    {
                        Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x, y, z), i, true, false, z_order));
                    }
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
                //Tiles.Add(new Tile("grass", 1, new Coordinates(0, 0), 0));
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

            float scale = 1f;

            Coordinates.Scale = scale;

            GraphicsDevice.Clear(Color.Black);


            spriteBatch.Begin();

            /*            for (int i = 0; i < Tiles.Count; i++)
                        {
                            spriteBatch.Draw(GetSpriteByID(Tiles[i].SpriteID), new Vector2((float)Tiles[i].Position.X * Coordinates.Step, (float)Tiles[i].Position.Y * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }*/

            RasterizerState r = new RasterizerState();
            r.ScissorTestEnable = true;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, r);
            Rectangle scrollRect = new Rectangle();
            scrollRect.Width = 832;
            scrollRect.Height = 768;
            scrollRect.X = 0;
            scrollRect.Y = 0;
            spriteBatch.GraphicsDevice.ScissorRectangle = scrollRect;

            for (int z = 0; z <= currentZ; z++)
            {
                for (int p = 0; p < Utility.MaxZ_Order; p++)
                {
                    for (int i = 0; i < Tiles.Count; i++)
                    {
                        if (Tiles[i].Position.Z == z && Tiles[i].Z_order == p)
                            spriteBatch.Draw(GetSpriteByID(Tiles[i].SpriteID), new Vector2((float)(Tiles[i].drawPosition.X - CurrentPosition.X + 12) * (Coordinates.Screen_Step), (float)(Tiles[i].drawPosition.Y - CurrentPosition.Y + 8) * (Coordinates.Screen_Step)), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }
            }

            spriteBatch.End();
            spriteBatch.Begin();

            spriteBatch.Draw(GetSpriteByID(75000), new Vector2(Coordinates.UI_Step * 26, 0), Color.White);

            /*spriteBatch.DrawString(font, "Experience: ", new Vector2((float)0, (float)736), Color.Black);*/
            /*spriteBatch.DrawString(font, "Health: ", new Vector2((float)0, (float)726), Color.Black);*/

            DrawOutlinedString(font, "Position: " + CurrentPosition.X + ", " + CurrentPosition.Y + ", " + currentZ, new Vector2(Coordinates.UI_Step * 26 + 5, Coordinates.UI_Step * 0), Color.White);

            DrawOutlinedString(font, "Z-order: " + currentZ_order, new Vector2((Coordinates.UI_Step * 26) + 17, (Coordinates.UI_Step * 3) + 2), Color.White);

            DrawOutlinedString(font, "Z: " + currentZ, new Vector2((Coordinates.UI_Step * 26) + 17, (Coordinates.UI_Step * 3) + 16), Color.White);

            DrawOutlinedString(font, "Action:", new Vector2((Coordinates.UI_Step * 29) + 5, (Coordinates.UI_Step * 1) + 16), Color.White);
            spriteBatch.Draw(GetSpriteByID(currentSprite.ID), new Vector2((Coordinates.UI_Step * 30) + 15, (Coordinates.UI_Step * 1) + 10), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            DrawOutlinedString(font, "Go to", new Vector2((Coordinates.UI_Step * 29) + 5, (Coordinates.UI_Step * 2) + 16), Color.White);
            DrawOutlinedString(font, "X:", new Vector2((Coordinates.UI_Step * 29) + 5, (Coordinates.UI_Step * 3) - 1), Color.White);
            DrawOutlinedString(font, "Y:", new Vector2((Coordinates.UI_Step * 29) + 5, (Coordinates.UI_Step * 3) + 14), Color.White);


            if (currentTileset == Tileset_Tiles)
            {
                GetSpriteObjectByID(51000).Position = new Coordinates((Coordinates.UI_Step * 27) + 20, (Coordinates.UI_Step * 6) + 8);
                DrawOutlinedString(font, Tileset_Tiles_Text, new Vector2((Coordinates.UI_Step * 26) + 17, (Coordinates.UI_Step * 6) + 4), Color.White);
                DrawTiles(Coordinates.UI_Step * 7);
            }
            else if(currentTileset == Tileset_Items)
            {
                GetSpriteObjectByID(51000).Position = new Coordinates((Coordinates.UI_Step * 27) + 25, (Coordinates.UI_Step * 6) + 8);
                DrawOutlinedString(font, Tileset_Items_Text, new Vector2((Coordinates.UI_Step * 26) + 17, (Coordinates.UI_Step * 6) + 4), Color.White);
                DrawItems(Coordinates.UI_Step * 7);
            }
            else if(currentTileset == Tileset_Creatures)
            {
                GetSpriteObjectByID(51000).Position = new Coordinates((Coordinates.UI_Step * 27) + 49, (Coordinates.UI_Step * 6) + 8);
                DrawOutlinedString(font, Tileset_Creatures_Text, new Vector2((Coordinates.UI_Step * 26) + 17, (Coordinates.UI_Step * 6) + 4), Color.White);
                DrawCreatures(Coordinates.UI_Step * 7);
            }

            DrawToolbox();
            DrawTextboxes();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        internal bool OutOfBoundaries(Coordinates Coordinates)
        {
            return !(Coordinates.X >= 12 && Coordinates.Y >= 8 && Coordinates.X < Utility.MaxX && Coordinates.Y < Utility.MaxY);
        }

        public void DrawOutlinedString(SpriteFont font, string Text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, Text, new Vector2(position.X - 1, position.Y), Color.Black);
            spriteBatch.DrawString(font, Text, new Vector2(position.X + 1, position.Y), Color.Black);
            spriteBatch.DrawString(font, Text, new Vector2(position.X, position.Y + 1), Color.Black);
            spriteBatch.DrawString(font, Text, new Vector2(position.X, position.Y - 1), Color.Black);
            spriteBatch.DrawString(font, Text, position, color);
        }

        public void DrawTextboxes()
        {
            SpriteFont font;
            font = Content.Load<SpriteFont>("EFont");
            for (int i = 0; i < Textboxes.Count; i++)
            {
                spriteBatch.Draw(GetSpriteByID(Textboxes[i].Start_SpriteID), new Vector2((float)Textboxes[i].Position.X, (float)Textboxes[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                for (int j = 0; j < (Textboxes[i].Width / 5); j++)
                {
                    spriteBatch.Draw(GetSpriteByID(Textboxes[i].Middle_SpriteID), new Vector2((float)(Textboxes[i].Position.X + ((j + 1) * 5)), (float)Textboxes[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(GetSpriteByID(Textboxes[i].End_SpriteID), new Vector2((float)(Textboxes[i].Position.X + Textboxes[i].Width), (float)Textboxes[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                spriteBatch.DrawString(font, Textboxes[i].Text, new Vector2((float)(Textboxes[i].Position.X + 2), (float)Textboxes[i].Position.Y - 1), Color.Black);
            }
        }


        public void DrawTiles(int start)
        {
            int c = 0;

            // draw tiles
            for(int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].EntityType == Entity.TileEntity)
                {
                    spriteBatch.Draw(spriteList[i].Sprite, new Vector2((float)(26 * Coordinates.UI_Step) + 5, (float)start + (c * Coordinates.UI_Step)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteList[i].Position = new Coordinates((26 * Coordinates.UI_Step) + 5, start + (c * Coordinates.UI_Step));
                    c++;
                }
            }
        }

        public void ClearTilesetPositions(int EntityType)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].EntityType == EntityType)
                {
                    spriteList[i].Position = new Coordinates((26000 * Coordinates.UI_Step), (26000 * Coordinates.UI_Step));
                }
            }
        }

        public void DrawItems(int start)
        {
            int c = 0;

            // draw tiles
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].EntityType == Entity.ItemEntity)
                {
                    spriteBatch.Draw(spriteList[i].Sprite, new Vector2((float)(26 * Coordinates.UI_Step) + 5, (float)start + (c * Coordinates.UI_Step)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteList[i].Position = new Coordinates((26 * Coordinates.UI_Step) + 5, start + (c * Coordinates.UI_Step));
                    c++;
                }
            }
        }

        public void DrawCreatures(int start)
        {
            int c = 0;

            // draw tiles
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].EntityType == Entity.CreatureEntity)
                {
                    spriteBatch.Draw(spriteList[i].Sprite, new Vector2((float)(26 * Coordinates.UI_Step) + 5, (float)start + (c * Coordinates.UI_Step)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteList[i].Position = new Coordinates((26 * Coordinates.UI_Step) + 5, start + (c * Coordinates.UI_Step));
                    c++;
                }
            }
        }

        public void DrawToolbox()
        {
            // draw toolbox
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].ID > 49000 && spriteList[i].Position != null)
                {
                    spriteBatch.Draw(spriteList[i].Sprite, new Vector2((float)spriteList[i].Position.X, (float)spriteList[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
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

        private SpriteObject GetSpriteObjectByID(int ID)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].ID == ID)
                {
                    return spriteList[i];
                }

            }
            return spriteList[0];
        }

        private Tile GetTileByMousePosition(int x, int y)
        {
            /*Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;*/

            int newX = CurrentPosition.X + (x / 32) - 12;
            int newY = CurrentPosition.Y + (y / 32) - 8;

            for (int i = 0; i < Tiles.Count; i++)
            {
                //tmpRect.X = Tiles[i].Position.X;
                //tmpRect.Y = Tiles[i].Position.Y;
                if (SamePosition(Tiles[i].Position, new Coordinates(newX, newY, currentZ)) && Tiles[i].Z_order == currentZ_order)
                {
                    return Tiles[i];
                }
            }
            return new Tile("null");
        }

        private int GetTileIndexByMousePosition(int x, int y)
        {
            /*Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;*/

            int newX = CurrentPosition.X + (x / 32) - 12;
            int newY = CurrentPosition.Y + (y / 32) - 8;
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (SamePosition(Tiles[i].Position, new Coordinates(newX, newY, currentZ)))
                {
                    return i;
                }
            }
            return -1;
        }

        private Tile GetTopItemFromTile(Coordinates Tile)
        {
            List<Tile> TileItems = new List<Tile>();
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (SamePosition(Tiles[i].Position, Tile))
                {
                    TileItems.Add(Tiles[i]);
                }
            }
            if (TileItems.Count > 0)
            {
                TileItems.Sort((a, b) => a.Z_order.CompareTo(b.Z_order));
                return TileItems[TileItems.Count - 1];
            }
            return new Tile("null");
        }

        private bool SamePosition(Coordinates Source, Coordinates Destination, bool CheckZ = true)
        {
            if (CheckZ)
                return (Source.X == Destination.X && Source.Y == Destination.Y && Source.Z == Destination.Z);
            else
                return (Source.X == Destination.X && Source.Y == Destination.Y);
        }

        private SpriteObject GetToolbarByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].Position != null)
                {
                    if (spriteList[i].EntityType == Entity.TenByTenButton)
                    {
                        tmpRect.Height = 10;
                        tmpRect.Width = 10;
                    }
                    else
                    {
                        tmpRect.Height = 32;
                        tmpRect.Width = 32;
                    }
                    tmpRect.X = spriteList[i].Position.X;
                    tmpRect.Y = spriteList[i].Position.Y;
                    if (tmpRect.Contains(x, y))
                    {
                        return spriteList[i];
                    }
                }
            }
            return new SpriteObject("null");
        }

        private Textbox GetTextboxByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 15;
            tmpRect.Width = 30;
            for (int i = 0; i < Textboxes.Count; i++)
            {
                if (Textboxes[i].Position != null)
                {
                    tmpRect.X = Textboxes[i].Position.X;
                    tmpRect.Y = Textboxes[i].Position.Y;
                    tmpRect.Width = Textboxes[i].Width;
                    if (tmpRect.Contains(x, y))
                    {
                        return Textboxes[i];
                    }
                }
            }
            return new Textbox();
        }

        private Textbox GetTextboxByName(string Name)
        {
            for (int i = 0; i < Textboxes.Count; i++)
            {
                if (Textboxes[i].Position != null)
                {
                    if (Textboxes[i].Name == Name)
                    {
                        return Textboxes[i];
                    }
                }
            }
            return new Textbox();
        }

        private Textbox GetTextboxByID(int ID)
        {
            for (int i = 0; i < Textboxes.Count; i++)
            {
                if (Textboxes[i].Position != null)
                {
                    if (Textboxes[i].ID == ID)
                    {
                        return Textboxes[i];
                    }
                }
            }
            return new Textbox("None");
        }
    }
}
