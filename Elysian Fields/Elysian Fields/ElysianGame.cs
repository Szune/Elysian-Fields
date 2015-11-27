/*
    Elysian Fields is a 2D game programmed in C# with the framework MonoGame
    Copyright (C) 2015 Erik Iwarson

    If you have any questions, don't hesitate to send me an e-mail at erikiwarson@gmail.com

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.IO;
using System;
using Elysian_Fields.Modules.Controls;
using Elysian_Fields.Modules.EventHandlers;

namespace Elysian_Fields
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>

    public class ElysianGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map map = Map.Instance;
        List<SpriteObject> spriteList = new List<SpriteObject>();
        List<UI> listUI = new List<UI>();
        List<SpriteObject> MouseCursors = new List<SpriteObject>();
        Item dragItem = new Item();
        List<DamageObject> dmgDone = new List<DamageObject>();
        List<Spell> Spells = new List<Spell>();
        List<UI> spellUI = new List<UI>();
        List<Backpack> OpenBackpacks = new List<Backpack>();
        List<DamageObject> SpellDamage = new List<DamageObject>();
        List<Textbox> Textboxes = new List<Textbox>();

        List<Animation> TextPopUps = new List<Animation>();
        List<Animation> ChatPopUps = new List<Animation>();

        private int currentMouse = 0;
        private int lastMouse = 0;

        private const int No_Focused_Textbox = -1;

        private int lastKeyHash = -1;
        

        private int FocusedTextbox = 0;

        private int FPS = 0;

        private bool InHouse = false;
        private int MaxZ;

        private MouseState OldMouseState = new MouseState();
        private MouseState CurrentMouseState = new MouseState();

        private bool CapsLocked = false;

        private int UI_Start = 26;

        SpriteFont font;

        private bool RightClicked = false;
        private bool LeftClicked = false;
        private bool Looked = false;
        private bool Resizing = false;
        private bool Scrolling = false;
        private Backpack ResizingBag = new Backpack();
        private Backpack ScrollingBag = new Backpack();

        private MouseState OldMousePos = new MouseState();

        private int leftClickTime;

        private const int Direction_North = 1;
        private const int Direction_East = 2;
        private const int Direction_South = 3;
        private const int Direction_West = 4;

        private int Walking_Direction;
        private int TimeOfLastMovement;
        private int TimeOfLastKeyPress;

        private Keys lastPressedKey;
        private Keys mostRecentKey;
        private KeyboardState lastKeyPress;

        private bool Walking = false;

        private bool regenerated = false;

        private bool respawned = false;

        public ElysianGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = int.Parse(ConfigurationManager.AppSettings["ClientWidth"]);
            graphics.PreferredBackBufferHeight = int.Parse(ConfigurationManager.AppSettings["ClientHeight"]);

            graphics.ApplyChanges();
            EventInput.Initialize(Window);
            EventInput.CharEntered += new CharEnteredHandler(EventInput_CharEntered);
            //EventInput.KeyDown += new KeyEventHandler(EventInput_KeyDown);

            // Uncomment the following lines to change the fps to 100
            /*this.TargetElapsedTime = System.TimeSpan.FromSeconds(1.0f / 50.0f);
            graphics.SynchronizeWithVerticalRetrace = false;*/
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            map.windowSize = new Coordinates(UI_Start * Coordinates.Step, Window.ClientBounds.Height);
            //map.Players.Add(new Player("Aephirus", new Coordinates(0, 0), 150, 100, 1, 1));
            //player1 = new Player("Aephirus", new Coordinates(0, 0));

            //this.IsMouseVisible = true;

            base.Initialize();

            RightClicked = false;
            LeftClicked = false;
        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (e.Character == '\b' && GetTextboxByID(FocusedTextbox).Text.Length > 0)
            {
                GetTextboxByID(FocusedTextbox).Text = GetTextboxByID(FocusedTextbox).Text.Substring(0, GetTextboxByID(FocusedTextbox).Text.Length - 1);
            }
            else
            {
                if (e.Character != '\r' && e.Character != '\b' && e.Character != '\t')
                {
                    GetTextboxByID(FocusedTextbox).Text += e.Character;
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\player"), 1, Entity.CreatureEntity, "player"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\Grass"), 2, Entity.TileEntity, "tile"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\attackbox"), 3, Entity.UnknownEntity, "attackbox"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile2"), 4, Entity.TileEntity, "tile2"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_ui"), 5, Entity.SpellEntity, "UI_FistSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_animation"), 6, Entity.SpellEntity, "Spell_FistSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_ui"), 7, Entity.SpellEntity, "UI_HealSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_animation"), 8, Entity.SpellEntity, "Spell_HealSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\goldenarmor"), 9, Entity.ItemEntity, "Golden Armor"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\helmet"), 10, Entity.UnknownEntity, ItemSlot.Helmet));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\torso"), 11, Entity.UnknownEntity, ItemSlot.Armor));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fist"), 12, Entity.UnknownEntity, ItemSlot.LeftHand));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fist"), 13, Entity.UnknownEntity, ItemSlot.RightHand));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\hornedhelmet"), 14, Entity.ItemEntity, "Horned Helmet"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UI_background"), 15, Entity.UnknownEntity, "UI_background"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\magicsword"), 16, Entity.ItemEntity, "Magic Sword"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\ghost"), 17, Entity.CreatureEntity, "Ghost"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\legs"), 18, Entity.UnknownEntity, ItemSlot.Legs));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\masterlegs"), 19, Entity.ItemEntity, "Master Legs"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\ui_bag"), 20, Entity.UnknownEntity, ItemSlot.Bag));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\backpack"), 21, Entity.ItemEntity, "Backpack"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\backpackbackgrund"), 22, Entity.UnknownEntity, "UI_BackpackBackground"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\CloseButton"), 23, Entity.UnknownEntity, "UI_CloseButton"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UpArrow"), 24, Entity.UnknownEntity, "UI_UpArrow"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\DownArrow"), 25, Entity.UnknownEntity, "UI_DownArrow"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\Scrollbar"), 26, Entity.UnknownEntity, "UI_Scrollbar"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenWall"), 27, Entity.TileEntity, "WoodWall", false));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenWall2"), 28, Entity.TileEntity, "WoodWall2", false));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TiledRoof"), 29, Entity.TileEntity, "TiledRoof", true));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenStairsDown"), 30, Entity.TileEntity, "WStairsDown", true, true, new Coordinates(1, 2, -1)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenStairsUp"), 31, Entity.TileEntity, "WStairsUp", true, true, new Coordinates(-1, -2, 1)));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\WoodenFloor"), 32, Entity.TileEntity, "WFloor"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TextboxStart"), 40000, Entity.UnknownEntity, "TextboxStart"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TextboxMiddle"), 41000, Entity.UnknownEntity, "TextboxMiddle"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\TextboxEnd"), 42000, Entity.UnknownEntity, "TextboxEnd"));


            //map.ItemList.Add(new Item("Sword of magicnezz", new Coordinates(3 * Coordinates.Step, 3 * Coordinates.Step), 1, 1, 60, 1));
            map.ItemList.Add(new Item("Magic Sword", ItemSlot.LeftHand, new Coordinates(3 * Coordinates.Step, 5 * Coordinates.Step), GetSpriteIDByName("Magic Sword"), GetSpriteIDByName("Magic Sword"), 60, 1));
            map.ItemList.Add(new Item("Golden Armor", ItemSlot.Armor, new Coordinates(2 * Coordinates.Step, 2 * Coordinates.Step), GetSpriteIDByName("Golden Armor"), GetSpriteIDByName("Golden Armor"), 0, 15));
            map.ItemList.Add(new Item("Horned Helmet", ItemSlot.Helmet, new Coordinates(3 * Coordinates.Step, 2 * Coordinates.Step), GetSpriteIDByName("Horned Helmet"), GetSpriteIDByName("Horned Helmet"), 0, 12));
            map.ItemList.Add(new Item("Master Legs", ItemSlot.Legs, new Coordinates(3 * Coordinates.Step, 2 * Coordinates.Step), GetSpriteIDByName("Master Legs"), GetSpriteIDByName("Master Legs"), 0, 10));
            map.ItemList.Add(new Item("Backpack", ItemSlot.Bag, new Coordinates(3 * Coordinates.Step, 2 * Coordinates.Step), GetSpriteIDByName("Backpack"), GetSpriteIDByName("Backpack"), 0, 0));


            Spells.Add(new Spell(new bool[]
            {true, true, true,
            true, true, true,
            true, true, true}
            , 50, GetSpriteByName("Spell_FistSpell"), 20, false, false, 1));

            Spells.Add(new Spell(new bool[] { true }, 50, GetSpriteByName("Spell_HealSpell"), 5, true, true, 2));

            listUI.Add(new UI(GetSpriteByName(ItemSlot.LeftHand), listUI.Count, Entity.UnknownEntity, new Coordinates(UI_Start * Coordinates.Step, Coordinates.Step * 0), ItemSlot.LeftHand));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.RightHand), listUI.Count, Entity.UnknownEntity, new Coordinates((UI_Start + 1) * Coordinates.Step, Coordinates.Step * 0), ItemSlot.RightHand));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.Helmet), listUI.Count, Entity.UnknownEntity, new Coordinates((UI_Start + 2) * Coordinates.Step, Coordinates.Step * 0), ItemSlot.Helmet));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.Armor), listUI.Count, Entity.UnknownEntity, new Coordinates((UI_Start + 3) * Coordinates.Step, Coordinates.Step * 0), ItemSlot.Armor));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.Legs), listUI.Count, Entity.UnknownEntity, new Coordinates((UI_Start + 4) * Coordinates.Step, Coordinates.Step * 0), ItemSlot.Legs));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.Bag), listUI.Count, Entity.UnknownEntity, new Coordinates(UI_Start * Coordinates.Step, Coordinates.Step * 1), ItemSlot.Bag));

            spellUI.Add(new UI(GetSpriteByName("UI_FistSpell"), spellUI.Count, Entity.UnknownEntity, new Coordinates(UI_Start * Coordinates.Step, Coordinates.Step * 2), "Fist", 1));
            spellUI.Add(new UI(GetSpriteByName("UI_HealSpell"), spellUI.Count, Entity.UnknownEntity, new Coordinates((UI_Start + 1) * Coordinates.Step, Coordinates.Step * 2), "Heal", 2));

            Textboxes.Add(new Textbox(0, "Chat", 40000, 41000, 42000, "", Coordinates.Step * 25, new Coordinates(0, Coordinates.Step * 23), 100));

            LoadWorld();
            //map.Players[0].Name = LootList[1].LootChance.ToString();


            List<Loot> LootList = new List<Loot>();
            LootList.Add(new Loot(GetItemFromListByName("Master Legs")));
            LootList[0].LootChance = 1 / 100;
            LootList.Add(new Loot(GetItemFromListByName("Horned Helmet")));
            LootList[1].LootChance = 1 / 100;

            map.CreatureList.Add(new Creature("Ghost", new Coordinates(0, 0, 0), -1, 25, 100, 1, 10, 150, LootList, GetSpriteIDByName("Ghost")));
            map.CreatureList.Add(new Creature("Cat", new Coordinates(0, 0, 0), -1, 25, 100, 2, 10, 150, null, GetSpriteIDByName("Ghost")));


            int ghosts = 2;

            for (int i = 0; i < ghosts; i++)
            {
                map.CreateCreatureFromCreatureList(1, new Coordinates(2 + i, 0, 0));
            }

            //map.WorldItems.Add(CreateWorldItemFromListItem(GetItemFromListByName("Backpack").ID, new Coordinates(Coordinates.Step * 5, Coordinates.Step * 5)));
            //map.WorldItems.Add(map.CreateWorldItemFromListItem(GetItemFromListByName("Magic Sword").ID, new Coordinates(1, 5, 2)));
            //CreateWorldItemFromListItem(GetItemFromListByName("Backpack").ID, new Coordinates(Coordinates.Step * 10, Coordinates.Step * 10));

            //EquipItemFromAnywhere(map.Items[0], GetListUIByItemSlot(ItemSlot.LeftHand));

            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseRegular"), 1, Entity.UnknownEntity, "MouseRegular"));
            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseDrag"), 2, Entity.UnknownEntity, "MouseDrag"));
            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseDrag"), 3, Entity.UnknownEntity, "MouseDrag"));
            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseResize"), 3, Entity.UnknownEntity, "MouseResize"));

            LoadMap("Content\\fields.map");


            for (int i = 0; i < map.Tiles.Count; i++)
            {
                map.Tiles[i].Walkable = GetSpriteObjectByID(map.Tiles[i].SpriteID).Walkable;
            }

            font = Content.Load<SpriteFont>("EFont");

            map.LoadNPCs();

            // map.Players[0].EquippedItems.Bag.Container.AddItem(CreateWorldItemFromListItem(GetItemFromListByName("Magic Sword").ID));

            Window.Title = "Elysian Fields";
            //Window.Title = Utility.ExperienceNeededForLevel(1).ToString() + " " + Utility.ExperienceNeededForLevel(2).ToString() + " " + Utility.ExperienceNeededForLevel(3).ToString() + " " + Utility.ExperienceNeededForLevel(4).ToString() + " " + Utility.ExperienceNeededForLevel(5).ToString() + " " + Utility.ExperienceNeededForLevel(6).ToString() + " " + Utility.ExperienceNeededForLevel(7).ToString() + " " + Utility.ExperienceNeededForLevel(8).ToString() + " " + Utility.ExperienceNeededForLevel(9).ToString() + " " + Utility.ExperienceNeededForLevel(10).ToString() + " " + Utility.ExperienceNeededForLevel(100).ToString();
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
            {
                SaveWorld();
                Exit();
            }

            // For debug purposes: Window.Title = gameTime.TotalGameTime.Seconds.ToString();

            /*if(gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                Window.Title = FPS.ToString();
                FPS = 0;
            }*/

            if (this.IsActive)
            {
                DoKeyboardEvents(gameTime);
                DoMouseEvents(gameTime);
            }

            if (gameTime.TotalGameTime.TotalMilliseconds - TimeOfLastMovement > 250)
            {
                if (map.Players[0].hasPath())
                {
                    map.MovePlayer();
                    TimeOfLastMovement = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }

            if (gameTime.TotalGameTime.Milliseconds % 500 == 0)
            {
                MoveCreatures(gameTime);
            }

            if (gameTime.TotalGameTime.Milliseconds % 100 == 0)
            {
                InHouse = map.TileAbove(map.Players[0].Position);
                if (InHouse)
                {
                    MaxZ = map.Players[0].Position.Z;
                }
                else
                {
                    MaxZ = Utility.MaxZ;
                }
            }

            if (gameTime.TotalGameTime.Milliseconds % 950 == 0)
            {
                map.GeneratePaths();// <- Uncomment this to make monsters move
            }

            if (gameTime.TotalGameTime.Seconds % 2 == 0 && !regenerated)
            {
                regenerated = true;
                BaseRegeneration();
            }
            else if (gameTime.TotalGameTime.Seconds % 2 == 1 && regenerated)
            {
                regenerated = false;
            }

            if (gameTime.TotalGameTime.Seconds % 10 == 0 && !respawned)
            {
                respawned = true;
                Respawn();
            }
            else if (gameTime.TotalGameTime.Seconds % 10 == 1 && respawned)
            {
                respawned = false;
            }

            if (gameTime.TotalGameTime.TotalMilliseconds - map.Players[0].TimeOfLastAttack > 1000)
            {
                if (map.CanAttack(map.Players[0], map.GetCreatureByID(map.Players[0].TargetID)) && map.Players[0].hasTarget())
                {
                    int targetID = map.Players[0].TargetID;
                    int dmgDealt = map.PlayerAttack(map.Players[0]);
                    if (dmgDealt != -1)
                    {
                        int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                        dmgDone.Add(new DamageObject(map.GetCreatureByID(targetID), dmgDealt, false, currentTime, currentTime + DamageObject.DamageDuration));
                        map.Players[0].TimeOfLastAttack = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    }
                }
            }

            base.Update(gameTime);
        }

        private void DoMouseEvents(GameTime gameTime)
        {
            // TODO: Put all mouse checks in here
            ResetMouseStates();
            DoScrollWheelEvents();
            DoRightClickEvents(gameTime);
            DoLeftClickEvents(gameTime);

            UpdateCursorSprite();
        }

        private void ResetMouseStates()
        {
            if (Mouse.GetState().RightButton == ButtonState.Released && RightClicked)
            {
                RightClicked = false;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released && Looked)
            {
                Looked = false;
            }
        }

        private void DoScrollWheelEvents()
        {
            // Scroll bags

            CurrentMouseState = Mouse.GetState();
            int x = Mouse.GetState().X, y = Mouse.GetState().Y;
            Backpack Bag = GetBagFromCoordinates(x, y);
            if (Bag.ID != -1)
            {
                if (CurrentMouseState.ScrollWheelValue > OldMouseState.ScrollWheelValue)
                {
                    // Scroll up
                    int Wheel = (CurrentMouseState.ScrollWheelValue - OldMouseState.ScrollWheelValue) / 120;
                    if (Wheel == 1)
                        ScrollUpBag(Bag);
                }
                else if (CurrentMouseState.ScrollWheelValue < OldMouseState.ScrollWheelValue)
                {
                    // Scroll down
                    int Wheel = (OldMouseState.ScrollWheelValue - CurrentMouseState.ScrollWheelValue) / 120;
                    if (Wheel == 1)
                        ScrollDownBag(Bag);
                }
                OldMouseState = CurrentMouseState;
            }
        }

        private void DoRightClickEvents(GameTime gameTime)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && !RightClicked)
            {
                int x = Mouse.GetState().X, y = Mouse.GetState().Y;
                int creatureID = GetCreatureByMousePosition(x, y).ID;
                if (creatureID != -1)
                {
                    if (map.Players[0].TargetID == creatureID)
                    {
                        map.Players[0].TargetID = -1;
                    }
                    else
                    {
                        map.Players[0].TargetID = creatureID;
                    }
                }
                else
                {
                    Spell CastSpell = GetSpellByMousePosition(x, y);
                    if (CastSpell.ID != -1)
                    {
                        if (map.Players[0].CastSpell(CastSpell, (int)gameTime.TotalGameTime.TotalMilliseconds))
                        {
                            SpellDamage.Add(new DamageObject(map.Players[0], CastSpell.Damage, CastSpell.HealSpell, (int)gameTime.TotalGameTime.TotalMilliseconds, (int)gameTime.TotalGameTime.TotalMilliseconds + 1500, CastSpell.ID, map.Players[0].Position));
                            dmgDone.AddRange(map.PlayerCastSpell(map.Players[0], CastSpell, map.GetCreatureByID(map.Players[0].TargetID), gameTime));
                        }
                    }
                    else
                    {
                        UI container = GetListUIByMousePosition(x, y);
                        if (container.Name == ItemSlot.Bag)
                        {
                            Item bag = GetItemByMousePosition(x, y);
                            if (bag.ID != -1)
                            {
                                if (!bag.Container.Open)
                                {
                                    OpenBag(bag);
                                }
                                else
                                {
                                    CloseBag(bag);
                                }
                            }
                        }
                        else
                        {
                            // TODO: Add potion usage here, use the following code as a base:
                            Item item = GetItemFromBagCoordinates(x, y);
                            if (item.ID != -1)
                            {
                                if (item.Container != null)
                                {
                                    if (!item.Container.Open)
                                    {
                                        if (item.Parent.Open)
                                        {
                                            OpenBag(item);
                                        }
                                    }
                                    else
                                    {
                                        CloseBag(item);
                                    }
                                }
                            }
                            else
                            {
                                Item groundItem = GetItemByMousePosition(x, y);
                                if (groundItem.Container != null)
                                {
                                    if (!groundItem.Container.Open)
                                    {
                                        if (groundItem.Parent.ID != -1)
                                        {
                                            if (groundItem.Parent.Open)
                                            {
                                                OpenBag(groundItem);
                                            }
                                        }
                                        else
                                        {
                                            OpenBag(groundItem);
                                        }
                                    }
                                    else
                                    {
                                        if (groundItem.Parent.ID != -1)
                                        {
                                            if (groundItem.Parent.Open)
                                            {
                                                CloseBag(groundItem);
                                            }
                                        }
                                        else
                                        {
                                            CloseBag(groundItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();
                RightClicked = true;
            }
        }

        private void DoLeftClickEvents(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Released && LeftClicked)
            {
                int mx = Mouse.GetState().X, my = Mouse.GetState().Y;
                int x = (mx / 32) * 32;
                int y = (my / 32) * 32;
                Coordinates target = new Coordinates(x, y);
                Coordinates truePos = new Coordinates(map.Players[0].Position.X + (x / 32) - 12, map.Players[0].Position.Y + (y / 32) - 8);
                if (!Resizing)
                {
                    if (gameTime.TotalGameTime.TotalMilliseconds - leftClickTime < 200)
                    {
                        Backpack bagToClose = GetCloseButtonFromCoordinates(mx, my);
                        if (bagToClose.ID == -1)
                        {
                            Backpack bagToScrollUp = GetUpArrow(mx, my);
                            if (bagToScrollUp.ID == -1)
                            {
                                Backpack bagToScrollDown = GetDownArrow(mx, my);
                                if (bagToScrollDown.ID == -1)
                                {
                                    if (!map.OutOfBoundaries(truePos) && !OutOfClientBoundaries(target))
                                    {
                                        if (map.IsTileWalkable(truePos))
                                        {
                                            map.GeneratePathFromPlayer(map.Players[0], truePos);
                                        }
                                    }
                                }
                                else
                                {
                                    ScrollDownBag(bagToScrollDown);
                                }
                            }
                            else
                            {
                                ScrollUpBag(bagToScrollUp);
                            }
                        }
                        else
                        {
                            //Window.Title = bagToClose.ID.ToString();
                            Item bagClosing = GetItemByID(bagToClose.ID);
                            if (bagClosing.ID != -1)
                            {
                                CloseBag(bagClosing);
                            }
                        }
                    }
                    else
                    {
                        if (dragItem.ID != -1)
                        {
                            UI targetEquipment = GetListUIByMousePosition(x, y);
                            UI sourceEquipment = GetListUIByMousePosition(dragItem.Position.X, dragItem.Position.Y);
                            Item FromBag = GetItemFromBagCoordinates(dragItem.Position.X, dragItem.Position.Y);
                            Backpack ToBag = GetBagFromCoordinates(x, y);
                            if (FromBag.ID == -1)
                            {
                                if (targetEquipment.ID == -1)
                                {
                                    if (ToBag.Open)
                                    {
                                        if (sourceEquipment.ID == -1)
                                        {
                                            ThrowItemToBag(ToBag, target);
                                        }
                                        else
                                        {
                                            ThrowItemToBag(ToBag, target, sourceEquipment);
                                        }
                                    }
                                    else
                                    {
                                        if (sourceEquipment.ID == -1)
                                        {
                                            if (!map.OutOfBoundaries(truePos) && !map.OutOfBoundaries(target))
                                            {
                                                // TODO: Make it possible to throw items onto roofs for example
                                                truePos.Z = map.TopTileZ(truePos);
                                                if (InHouse && truePos.Z > map.Players[0].Position.Z)
                                                {
                                                    truePos.Z = map.Players[0].Position.Z;
                                                }
                                                map.DragItem(dragItem, truePos);
                                            }
                                        }
                                        else
                                        {
                                            if (!map.OutOfBoundaries(truePos))
                                            {
                                                truePos.Z = map.TopTileZ(truePos);
                                                if (InHouse && truePos.Z > map.Players[0].Position.Z)
                                                {
                                                    truePos.Z = map.Players[0].Position.Z;
                                                }
                                                UnequipItem(sourceEquipment, truePos);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (sourceEquipment.ID == -1)
                                    {
                                        EquipItem(targetEquipment);
                                    }
                                    else
                                    {
                                        EquipItem(targetEquipment, sourceEquipment);
                                    }
                                }
                            }
                            else
                            {
                                if (targetEquipment.ID == -1)
                                {
                                    if (ToBag.Open)
                                    {
                                        ThrowItemToBag(ToBag, target);
                                    }
                                    else
                                    {
                                        if (!map.OutOfBoundaries(truePos))
                                        {
                                            truePos.Z = map.TopTileZ(truePos);
                                            Window.Title = truePos.Z.ToString();
                                            if (InHouse && truePos.Z > map.Players[0].Position.Z)
                                            {
                                                truePos.Z = map.Players[0].Position.Z;
                                            }
                                            ThrowItemFromBag(truePos);
                                        }
                                    }
                                }
                                else
                                {
                                    EquipItemFromBag(targetEquipment);
                                }
                            }
                        }
                    }
                }

                Textbox clickedTextbox = GetTextboxByMousePosition(x, y);
                if (clickedTextbox.ID != -1)
                {
                    FocusedTextbox = clickedTextbox.ID;
                }
                else
                {
                    FocusedTextbox = GetTextboxByName("Chat").ID;
                }

                // For debug purposes: Window.Title = map.Players[0].TargetID.ToString();*/
                // For debug purposes: Window.Title = x.ToString() + " " + y.ToString();
                LeftClicked = false;
                currentMouse = 0;
                CheckOpenBags();
                Resizing = false;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && (!Keyboard.GetState().IsKeyDown(Keys.LeftControl) && !Keyboard.GetState().IsKeyDown(Keys.RightControl)))
            {
                if (!LeftClicked)
                {
                    int mx = Mouse.GetState().X, my = Mouse.GetState().Y;
                    int x = (mx / 32) * 32;
                    int y = (my / 32) * 32;
                    if (currentMouse == 3)
                    {
                        Resizing = true;
                        LeftClicked = true;
                        ResizingBag = GetBagFromCoordinates(x, y);
                    }
                    else
                    {
                        ScrollingBag = GetScrollbar(mx, my);
                        if (ScrollingBag.ID == -1)
                        {
                            leftClickTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                            dragItem = GetItemByMousePosition(mx, my);
                            LeftClicked = true;


                            if (dragItem.Parent != null && dragItem.Parent.ID != -1)
                            {
                                Item newz = GetItemFromBagCoordinates(mx, my);
                                if (GetItemFromBagCoordinates(mx, my).ID == -1)
                                {
                                    dragItem = new Item();
                                }
                            }
                        }
                        else
                        {
                            LeftClicked = true;
                            Scrolling = true;
                        }
                    }
                }
                else
                {
                    if (Resizing)
                    {
                        ResizeBag(ResizingBag);
                    }
                    else
                    {
                        if (Scrolling)
                        {
                            ScrollBag(ScrollingBag);
                        }
                        else
                        {
                            if (gameTime.TotalGameTime.TotalMilliseconds - leftClickTime > 200)
                            {
                                if (dragItem.ID != -1)
                                {
                                    MouseCursors[2].Sprite = GetSpriteByName(dragItem.Name);
                                    lastMouse = currentMouse;
                                    currentMouse = 2;
                                }
                                else
                                {
                                    lastMouse = currentMouse;
                                    currentMouse = 1;
                                }
                            }
                        }
                    }
                }
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Pressed && (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl)) && !LeftClicked && !Looked)
            {
                int mx = Mouse.GetState().X, my = Mouse.GetState().Y;
                int x = (mx / 32) * 32;
                int y = (my / 32) * 32;
                Item lookItem = GetItemByMousePosition(mx, my);
                if (lookItem.ID != -1)
                {
                    TextPopUps.Add(new Animation(1000, 0, 1, (int)gameTime.TotalGameTime.TotalMilliseconds, "You see " + lookItem.Name + " (ID: " + lookItem.ID + ")."));
                }
                Looked = true;
            }
        }

        private void UpdateCursorSprite()
        {
            Vector2 CursorPos = Mouse.GetState().Position.ToVector2();
            if (IsMouseAbleToResizeBag(CursorPos) && currentMouse != 3 && currentMouse != 2)
            {
                lastMouse = currentMouse;
                currentMouse = 3;
            }
            else if (!IsMouseAbleToResizeBag(CursorPos) && currentMouse == 3 && !Resizing)
            {
                currentMouse = lastMouse;
            }
        }

        private void DoKeyboardEvents(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - TimeOfLastMovement > 250)
            {
                ResetKeyHash();
                UpdateRecentlyPressedKey();
                UpdateWalkingState();
                Keyboard_MovePlayer(gameTime);
                //Keyboard_Chat(gameTime);
                Keyboard_SendChat(gameTime);
            }
        }

        private void Keyboard_SendChat(GameTime gameTime)
        {
            if (TimeOfLastKeyPress + 50 < gameTime.TotalGameTime.TotalMilliseconds)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    /*if(GetTextboxByName("Chat").Text == "clear")
                    {
                        map.DebugTiles_Pathfinding.Clear();
                        return;
                    }*/
                    ChatPopUps.Add(new Animation(3000, 0, 0, (int)gameTime.TotalGameTime.TotalMilliseconds, GetTextboxByName("Chat").Text, 0, new Coordinates(map.Players[0].Position.X, map.Players[0].Position.Y)));
                    for (int i = 0; i < map.NPCs.Count; i++)
                    {
                        string npcChat = map.NPCs[i].Chat(GetTextboxByName("Chat").Text.ToLower(), map.Players[0]);
                        if (npcChat.Length > 0)
                        {
                            ChatPopUps.Add(new Animation(3000, 0, 0, (int)gameTime.TotalGameTime.TotalMilliseconds, npcChat, 0, new Coordinates(map.NPCs[i].Position.X, map.NPCs[i].Position.Y)));
                        }
                    }
                    GetTextboxByName("Chat").Text = "";
                    TimeOfLastKeyPress = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }
        private void Keyboard_Chat(GameTime gameTime)
        {
            if (!lastKeyPress.IsKeyDown(Keys.CapsLock) && Keyboard.GetState().IsKeyDown(Keys.CapsLock))
            {
                CapsLocked = !CapsLocked;
                lastKeyPress = Keyboard.GetState();
            }
            if (FocusedTextbox != No_Focused_Textbox)
            {
                if ((gameTime.TotalGameTime.TotalMilliseconds > TimeOfLastKeyPress && lastKeyPress.GetHashCode() != Keyboard.GetState().GetHashCode()) || (gameTime.TotalGameTime.TotalMilliseconds > TimeOfLastKeyPress + 1050 && lastKeyPress.GetHashCode() == Keyboard.GetState().GetHashCode()))
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
                        else if(pressedKeys[0] == Keys.Space)
                        {
                            if((gameTime.TotalGameTime.TotalMilliseconds > TimeOfLastKeyPress + 100))
                            GetTextboxByID(FocusedTextbox).Text += " ";
                        }
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
                                else if ((pressedKeys[0].GetHashCode() > 64 && pressedKeys[0].GetHashCode() < 91))
                                {
                                    if (lastKeyPress.IsKeyDown(Keys.LeftShift) || lastKeyPress.IsKeyDown(Keys.RightShift) || CapsLocked)
                                    {
                                        GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString();
                                    }
                                    else
                                    {
                                        GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString().ToLower();
                                    }
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
                        else if(pressedKeys[0] == Keys.Space)
                        { 
                            GetTextboxByID(FocusedTextbox).Text += " ";
                        }
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
                                else if ((pressedKeys[0].GetHashCode() > 64 && pressedKeys[0].GetHashCode() < 91))
                                {
                                    if (lastKeyPress.IsKeyDown(Keys.LeftShift) || lastKeyPress.IsKeyDown(Keys.RightShift) || CapsLocked)
                                    {
                                        GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString();
                                    }
                                    else
                                    {
                                        GetTextboxByID(FocusedTextbox).Text += pressedKeys[0].ToString().ToLower();
                                    }
                                }
                            }
                        }
                        lastKeyHash = pressedKeys[0].GetHashCode();
                    }
                    if (lastKeyPress.GetHashCode() != Keyboard.GetState().GetHashCode())
                        TimeOfLastKeyPress = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }

        /*private void Keyboard_Type(GameTime gameTime)
        {
            if (!lastKeyPress.IsKeyDown(Keys.CapsLock) && Keyboard.GetState().IsKeyDown(Keys.CapsLock))
            {
                CapsLocked = !CapsLocked;
                lastKeyPress = Keyboard.GetState();
            }
            if (FocusedTextbox != No_Focused_Textbox)
            {
                if (lastKeyPress.GetHashCode() != Keyboard.GetState().GetHashCode())
                {
                    lastKeyPress = Keyboard.GetState();
                    Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                    List<Keys> filteredKeyList = new List<Keys>();
                    if (pressedKeys.Length > 0)
                    {
                        for(int i = 0; i < pressedKeys.Length; i++)
                        {
                            if(filteredKeyList.Count < 1 && lastKeyHash != pressedKeys[i].GetHashCode())
                            {
                                filteredKeyList.Add(pressedKeys[i]);
                                lastKeyHash = pressedKeys[i].GetHashCode();
                            }
                            else
                            {
                                if(!filteredKeyList.Contains(pressedKeys[i]) && lastKeyHash != pressedKeys[i].GetHashCode())
                                {
                                    filteredKeyList.Add(pressedKeys[i]);
                                    lastKeyHash = pressedKeys[i].GetHashCode();
                                }
                            }
                        }
                        Keyboard_ParseTypedKeys(filteredKeyList);
                        //lastKeyHash = pressedKeys[0].GetHashCode();
                        TimeOfLastKeyPress = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    }

                }
                else if (gameTime.TotalGameTime.TotalMilliseconds > TimeOfLastKeyPress + 450 && lastKeyPress.GetHashCode() == Keyboard.GetState().GetHashCode())
                {
                    lastKeyPress = Keyboard.GetState();
                    Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                    List<Keys> filteredKeyList = new List<Keys>();
                    if (pressedKeys.Length > 0)
                    {
                        filteredKeyList.AddRange(pressedKeys);
                        Keyboard_ParseTypedKeys(filteredKeyList);
                        lastKeyHash = pressedKeys[0].GetHashCode();
                        //TimeOfLastKeyPress = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    }
                }
            }
        }*/

        private void Keyboard_ParseTypedKeys(List<Keys> pressedKeys)
        {
            for (int i = 0; i < pressedKeys.Count; i++)
            {
                if (pressedKeys[i] == Keys.Back)
                {
                    string tmpText = GetTextboxByID(FocusedTextbox).Text;
                    if (tmpText.Length > 0)
                    {
                        GetTextboxByID(FocusedTextbox).Text = tmpText.Substring(0, tmpText.Length - 1);
                    }
                }
                else if (pressedKeys[i] == Keys.Space)
                {
                    GetTextboxByID(FocusedTextbox).Text += " ";
                }
                else
                {
                    if (GetTextboxByID(FocusedTextbox).Text.Length < GetTextboxByID(FocusedTextbox).MaxChars)
                    {
                        if ((pressedKeys[i].GetHashCode() > 95 && pressedKeys[i].GetHashCode() < 106))
                        {
                            GetTextboxByID(FocusedTextbox).Text += pressedKeys[i].ToString().Substring(6);
                        }
                        else if ((pressedKeys[i].GetHashCode() > 47 && pressedKeys[i].GetHashCode() < 58))
                        {
                            GetTextboxByID(FocusedTextbox).Text += pressedKeys[i].ToString().Substring(1);
                        }
                        else if ((pressedKeys[i].GetHashCode() > 64 && pressedKeys[i].GetHashCode() < 91))
                        {
                            if (lastKeyPress.IsKeyDown(Keys.LeftShift) || lastKeyPress.IsKeyDown(Keys.RightShift) || CapsLocked)
                            {
                                GetTextboxByID(FocusedTextbox).Text += pressedKeys[i].ToString();
                            }
                            else
                            {
                                GetTextboxByID(FocusedTextbox).Text += pressedKeys[i].ToString().ToLower();
                            }
                        }
                    }
                }
            }
        }

        private void ResetKeyHash()
        {
            if (Keyboard.GetState().GetPressedKeys().Length < 1)
            {
                lastKeyHash = -1;
            }
        }

        private void ResetPath()
        {
            if (map.Players[0].hasPath())
            {
                map.Players[0].ResetPath();
            }
        }

        private void UpdateRecentlyPressedKey()
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

        private void Keyboard_MovePlayer(GameTime gameTime)
        {
            if (Walking && map.Players[0].Health > 0)
            {
                if (Walking_Direction == Direction_East)
                {
                    map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X + 1, map.Players[0].Position.Y, map.Players[0].Position.Z));
                }
                else if (Walking_Direction == Direction_West)
                {
                    map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X - 1, map.Players[0].Position.Y, map.Players[0].Position.Z));
                }
                else if (Walking_Direction == Direction_South)
                {
                    map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X, map.Players[0].Position.Y + 1, map.Players[0].Position.Z));
                }
                else if (Walking_Direction == Direction_North)
                {
                    map.MoveCreature(map.Players[0], new Coordinates(map.Players[0].Position.X, map.Players[0].Position.Y - 1, map.Players[0].Position.Z));
                }
                ResetWalkingState(gameTime);
                CheckOpenBags();
                ResetPath();
            }
        }

        private void ResetWalkingState(GameTime gameTime)
        {
            TimeOfLastMovement = (int)gameTime.TotalGameTime.TotalMilliseconds;
            Walking = false;
            mostRecentKey = Keys.None;
            lastPressedKey = Keys.None;
        }

        private void UpdateWalkingState()
        {
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
        }

        public void Respawn()
        {
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                if (map.Creatures[i].Health < 1)
                {
                    map.Creatures[i].Spawn();
                }
            }
        }

        private void ScrollDownBag(Backpack Bag)
        {
            // Scroll down
            if (Bag.Scroll.CurrentStep < (Bag.Scroll.MaxHeight / 10) - (Bag.Height / 10))
            {
                Bag.Scroll.CurrentStep += 1;
            }
            else
            {
                Bag.Scroll.CurrentStep = (Bag.Scroll.MaxHeight / 10) - (Bag.Height / 10);
            }
        }

        private void ScrollUpBag(Backpack Bag)
        {
            // Scroll up
            if (Bag.Scroll.CurrentStep > 0)
            {
                Bag.Scroll.CurrentStep -= 1;
            }
            else
            {
                Bag.Scroll.CurrentStep = 0;
            }
        }

        private void CheckOpenBags()
        {
            // TODO: Change when adding multiplayer
            List<Item> BagsToClose = new List<Item>();
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                Item Bag = TopBagItem(GetItemByID(OpenBackpacks[i].ID));
                Item BagToClose = GetItemByID(OpenBackpacks[i].ID);

                if (Bag.Slot != ItemSlot.Bag && !map.AdjacentToItem(map.Players[0], Bag))
                {
                    BagsToClose.Add(BagToClose);
                }
            }
            for (int i = 0; i < BagsToClose.Count; i++)
            {
                CloseBag(BagsToClose[i]);
            }
        }

        private void OpenBag(Item bag)
        {
            if (bag.Parent.ID == -1 || bag.Parent.Open)
            {
                if (map.AdjacentToItem(map.Players[0], bag) || (bag.Slot == ItemSlot.Bag || bag.Slot == ItemSlot.InsideBag))
                {
                    bag.Container.Open = true;
                    OpenBackpacks.Add(bag.Container);
                }
            }
        }

        private void CloseBag(Item bag)
        {
            //if (bag.Parent.ID == -1 || bag.Parent.Open)
            //{
            bag.Container.Open = false;
            OpenBackpacks.Remove(bag.Container);
            //}
        }

        private bool IsInBag(Item item)
        {
            return (item.Parent != null);
        }

        internal void LoadMap(string mapn) // Redan kommenterad i PacMan - MapEditor
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

                    bool MovePlayer = GetSpriteObjectByID(spriteID).MovePlayer;
                    Coordinates RelativeMovePosition = GetSpriteObjectByID(spriteID).RelativeMovePosition;

                    

                    if (GetSpriteByID(spriteID).Height == 64 && GetSpriteByID(spriteID).Width != 64)
                    {
                        map.Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x, y - 1, z), i, true, false, z_order, MovePlayer, RelativeMovePosition));
                    }
                    else if (GetSpriteByID(spriteID).Width == 64 && GetSpriteByID(spriteID).Height != 64)
                    {
                        map.Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x - 1, y, z), i, true, false, z_order, MovePlayer, RelativeMovePosition));
                    }
                    else if (GetSpriteByID(spriteID).Width == 64 && GetSpriteByID(spriteID).Height == 64)
                    {
                        map.Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x - 1, y - 1, z), i, true, false, z_order, MovePlayer, RelativeMovePosition));
                    }
                    else
                    {
                        map.Tiles.Add(new Tile("grass", spriteID, new Coordinates(x, y, z), new Coordinates(x, y, z), i, true, false, z_order, MovePlayer, RelativeMovePosition));
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
                map.Players[0].Name = "Couldn't load map";
            }
        }


        private Backpack TopBag(Item BagInsideBag)
        {
            bool LoopDone = false;
            Item currentBag = BagInsideBag;
            Backpack topBag = new Backpack();
            if (BagInsideBag.Slot == ItemSlot.Bag)
            {
                topBag = BagInsideBag.Container;
            }
            else
            {
                while (!LoopDone)
                {
                    if (currentBag.Parent != null)
                    {
                        if (currentBag.Parent.ID != -1)
                        {
                            topBag = currentBag.Parent;
                        }
                        currentBag = GetItemByID(currentBag.Parent.ID);
                    }
                    else
                    {
                        LoopDone = true;
                    }
                }
            }

            return topBag;
        }

        private Item TopBagItem(Item BagInsideBag)
        {
            bool LoopDone = false;
            Item currentBag = BagInsideBag;
            Item topBag = currentBag;

            if (BagInsideBag.Slot == ItemSlot.Bag)
            {
                topBag = BagInsideBag;
            }
            else
            {
                while (!LoopDone)
                {
                    if (currentBag.Parent != null)
                    {
                        if (currentBag.Parent.ID != -1)
                        {
                            topBag = GetItemByID(currentBag.Parent.ID);
                        }
                        currentBag = GetItemByID(currentBag.Parent.ID);
                    }
                    else
                    {
                        LoopDone = true;
                    }
                }
            }

            return topBag;
        }

        private bool IsBagInBag(Item BagInsideBag, Item InBag)
        {
            bool _IsInBag = false;
            bool LoopDone = false;
            Item currentBag = BagInsideBag;

            while (!LoopDone)
            {
                if (currentBag.Parent != null)
                {
                    if (currentBag.Parent.ID == InBag.ID)
                    {
                        return true;
                    }
                    else
                    {
                        currentBag = GetItemByID(currentBag.Parent.ID);
                    }
                }
                else
                {
                    return false;
                }
            }

            return _IsInBag;
        }

        private void EquipItem(UI targetEquipment, UI SourceEquipment = null)
        {
            if (SourceEquipment == null)
            {
                map.EquipItem(dragItem, targetEquipment);
            }
            else
            {
                map.EquipItem(dragItem, targetEquipment, SourceEquipment, false);
            }
        }

        private void EquipItemFromBag(UI targetEquipment)
        {
            map.EquipItemFromBag(dragItem, targetEquipment);
        }

        private void ThrowItemFromBag(Coordinates Destination)
        {
            map.ThrowItemFromBag(dragItem, Destination);
        }

        private void ThrowItemToBag(Backpack Bag, Coordinates Destination, UI SourceEquipment = null)
        {
            // TODO: Don't allow main bag to be thrown into any of its nested bags
            if (dragItem.Container == null || dragItem.Container.ID != Bag.ID)
            {
                if (!IsBagInBag(GetItemByID(Bag.ID), dragItem))
                {
                    if (SourceEquipment == null)
                    {
                        if (dragItem.Parent.IsEmpty())
                        {
                            map.ThrowItemToBag(dragItem, Bag, Destination);
                        }
                        else
                        {
                            map.ThrowItemToBag(dragItem, Bag, Destination, null, dragItem.Parent);
                        }
                    }
                    else
                    {
                        SourceEquipment.Sprite = GetSpriteByName(SourceEquipment.Name);
                        map.ThrowItemToBag(dragItem, Bag, Destination, SourceEquipment);
                    }
                }
            }
        }

        private void EquipItemFromAnywhere(Item item, UI targetEquipment, UI SourceEquipment = null)
        {
            if (SourceEquipment == null)
            {
                map.EquipItem(item, targetEquipment, null, false);
            }
            else
            {
                map.EquipItem(item, targetEquipment, SourceEquipment, false);
            }
        }

        private void UnequipItem(UI SourceEquipment, Coordinates target)
        {
            SourceEquipment.Sprite = GetSpriteByName(SourceEquipment.Name);
            map.UnequipItem(dragItem, SourceEquipment, target);
        }

        public void BaseRegeneration()
        {
            int HP_Regeneration = 5;
            int MP_Regeneration = 10;
            for (int h = 0; h < map.Players.Count; h++)
            {
                if (map.Players[h].Health < map.Players[h].MaxHealth)
                {
                    if (map.Players[h].Health + HP_Regeneration > map.Players[h].MaxHealth)
                    {
                        map.Players[h].Health = map.Players[h].MaxHealth;
                    }
                    else
                    {
                        map.Players[h].Health += HP_Regeneration;
                    }
                }
                if (map.Players[h].Mana < map.Players[h].MaxMana)
                {
                    if (map.Players[h].Mana + MP_Regeneration > map.Players[h].MaxMana)
                    {
                        map.Players[h].Mana = map.Players[h].MaxMana;
                    }
                    else
                    {
                        map.Players[h].Mana += 10;
                    }
                }
            }
        }

        public void MoveCreatures(GameTime gameTime)
        {
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                if (map.Creatures[i].Health > 0 && map.Creatures[i].hasPath())
                {
                    map.MoveCreature(map.Creatures[i], map.Creatures[i].NextStep());
                }
                else
                {
                    int targetID = map.Creatures[i].TargetID;
                    if (targetID != -1)
                    {
                        if (map.IsAdjacent(map.Creatures[i], map.GetPlayerByID(targetID)) && map.Creatures[i].Position.Z == map.GetPlayerByID(targetID).Position.Z)
                        {
                            if (gameTime.TotalGameTime.TotalMilliseconds - map.Creatures[i].TimeOfLastAttack > 1000)
                            {
                                int dmgDealt = map.CreatureAttack(map.Creatures[i], map.GetPlayerByID(targetID));
                                if (dmgDealt != -1)
                                {
                                    int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                                    map.Creatures[i].TimeOfLastAttack = currentTime;
                                    dmgDone.Add(new DamageObject(map.GetPlayerByID(targetID), dmgDealt, false, currentTime, currentTime + DamageObject.DamageDuration));

                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            FPS += 1;
            Vector2 cursorPos = Mouse.GetState().Position.ToVector2();

            GraphicsDevice.Clear(Color.DarkSlateGray);

            //Window.Title = map.Players[0].Position.X + " " + map.Players[0].Position.Y + " " + map.Players[0].Position.Z;
            spriteBatch.Begin();

            DrawUI();

            DrawEquipment();

            RasterizerState r = new RasterizerState();
            r.ScissorTestEnable = true;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, r);
            Rectangle scrollRect = new Rectangle();
            scrollRect.Width = 800;
            scrollRect.Height = 544;
            scrollRect.X = 0;
            scrollRect.Y = 0;
            spriteBatch.GraphicsDevice.ScissorRectangle = scrollRect;

            for (int z = 0; z <= MaxZ; z++)
            {
                for (int p = 0; p < Utility.MaxZ_Order; p++)
                {
                    for (int i = 0; i < map.Tiles.Count; i++)
                    {
                        if (map.Tiles[i].Position.Z == z && map.Tiles[i].Z_order == p)
                            spriteBatch.Draw(GetSpriteByID(map.Tiles[i].SpriteID), new Vector2((float)(map.Tiles[i].drawPosition.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.Tiles[i].drawPosition.Y - map.Players[0].Position.Y + 8) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int i = 0; i < map.WorldItems.Count; i++)
                    {
                        //if(GetListUIByMousePosition(map.MapItems[i].Position.X, map.MapItems[i].Position.Y).ID == -1)
                        if (map.WorldItems[i].Slot == null && map.WorldItems[i].Parent.IsEmpty() && map.WorldItems[i].Position.Z == z && map.WorldItems[i].Z_order == p)
                            spriteBatch.Draw(GetSpriteByID(map.WorldItems[i].SpriteID), new Vector2((float)(map.WorldItems[i].Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.WorldItems[i].Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }

                if(map.DebugTiles_Pathfinding.Count > 0)
                {
                    for (int i = 0; i < map.DebugTiles_Pathfinding.Count; i++)
                    {
                        if (map.DebugTiles_Pathfinding[i].Position.Z == z)
                            spriteBatch.Draw(GetSpriteByID(map.DebugTiles_Pathfinding[i].SpriteID), new Vector2((float)(map.DebugTiles_Pathfinding[i].drawPosition.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.DebugTiles_Pathfinding[i].drawPosition.Y - map.Players[0].Position.Y + 8) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }

                // Draw creatures names:
                DrawCreatureNames(z);

                // Draw player name:
                if (map.Players[0].Position.Z == z)
                {
                    DrawOutlinedString(font, map.Players[0].Name, new Vector2((float)Utility.CenterCoordinates.X, (float)Utility.CenterCoordinates.Y - 16), Color.White);
                }

                // Draw Creatures:
                DrawCreatures(z);

                DrawNPCs(z);

                DrawNPCNames(z);

                // Draw player sprite:
                if (map.Players[0].Position.Z == z)
                {
                    spriteBatch.Draw(GetSpriteByID(map.Players[0].SpriteID), new Vector2((float)Utility.CenterCoordinates.X, (float)Utility.CenterCoordinates.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }

            }

            DrawSpells(gameTime);

            // Draw damage dealt
            DrawDamageDone(gameTime);

            // Draw look-objects
            DrawTextPopUps(gameTime);

            DrawChatPopUps(gameTime);

            spriteBatch.End();
            spriteBatch.Begin();

            int start = (spellUI[spellUI.Count - 1].Position.Y / 32) + 1;
            int startStep = 0;
            int distance = 20;
            DrawOutlinedString(font, "Health: " + map.Players[0].Health + " / " + map.Players[0].MaxHealth, new Vector2((float)Coordinates.Step * UI_Start, (float)Coordinates.Step * start + startStep++ * distance), Color.White);
            DrawOutlinedString(font, "Mana: " + map.Players[0].Mana + " / " + map.Players[0].MaxMana, new Vector2((float)Coordinates.Step * UI_Start, (float)Coordinates.Step * start + startStep++ * distance), Color.White);
            DrawOutlinedString(font, "Level: " + map.Players[0].Level, new Vector2((float)Coordinates.Step * UI_Start, (float)Coordinates.Step * start + startStep++ * distance), Color.White);
            DrawOutlinedString(font, "Experience: " + map.Players[0].Experience, new Vector2((float)Coordinates.Step * UI_Start, (float)Coordinates.Step * start + startStep++ * distance), Color.White);
            DrawOutlinedString(font, "Magic Strength: " + map.Players[0].MagicStrength, new Vector2((float)Coordinates.Step * UI_Start, (float)Coordinates.Step * start + startStep++ * distance), Color.White);
            DrawOutlinedString(font, "Strength: " + map.Players[0].TotalStrength(), new Vector2((float)Coordinates.Step * UI_Start, (float)Coordinates.Step * start + startStep++ * distance), Color.White);
            DrawOutlinedString(font, "Defense: " + map.Players[0].TotalDefense(), new Vector2((float)Coordinates.Step * UI_Start, (float)Coordinates.Step * start + startStep++ * distance), Color.White);
            startStep++;
            DrawBags(Coordinates.Step * start + startStep * distance, cursorPos);
            DrawTextboxes();
            spriteBatch.Draw(MouseCursors[currentMouse].Sprite, cursorPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
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

        private void DrawEquipment()
        {
            UI UI_Pos = GetListUIByItemSlot(ItemSlot.Helmet);
            if (map.Players[0].EquippedItems.Helmet.ID != -1)
            {
                spriteBatch.Draw(GetSpriteByName("UI_background"), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(GetSpriteByID(map.Players[0].EquippedItems.Helmet.SpriteID), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            UI_Pos = GetListUIByItemSlot(ItemSlot.Armor);
            if (map.Players[0].EquippedItems.Armor.ID != -1)
            {
                spriteBatch.Draw(GetSpriteByName("UI_background"), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(GetSpriteByID(map.Players[0].EquippedItems.Armor.SpriteID), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            UI_Pos = GetListUIByItemSlot(ItemSlot.LeftHand);
            if (map.Players[0].EquippedItems.LeftHand.ID != -1)
            {
                spriteBatch.Draw(GetSpriteByName("UI_background"), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(GetSpriteByID(map.Players[0].EquippedItems.LeftHand.SpriteID), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            UI_Pos = GetListUIByItemSlot(ItemSlot.RightHand);
            if (map.Players[0].EquippedItems.RightHand.ID != -1)
            {
                spriteBatch.Draw(GetSpriteByName("UI_background"), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(GetSpriteByID(map.Players[0].EquippedItems.RightHand.SpriteID), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            UI_Pos = GetListUIByItemSlot(ItemSlot.Legs);
            if (map.Players[0].EquippedItems.Legs.ID != -1)
            {
                spriteBatch.Draw(GetSpriteByName("UI_background"), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(GetSpriteByID(map.Players[0].EquippedItems.Legs.SpriteID), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            UI_Pos = GetListUIByItemSlot(ItemSlot.Bag);
            if (map.Players[0].EquippedItems.Bag.ID != -1)
            {
                spriteBatch.Draw(GetSpriteByName("UI_background"), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(GetSpriteByID(map.Players[0].EquippedItems.Bag.SpriteID), new Vector2((float)UI_Pos.Position.X, (float)UI_Pos.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        private void DrawBags(int OffsetY, Vector2 cursorPos)
        {
            int ScrollBarOffset = 32;
            RasterizerState r = new RasterizerState();
            r.ScissorTestEnable = true;
            int y = 0;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                if (i == 0)
                {
                    y = i;
                    spriteBatch.End();
                    spriteBatch.Begin();
                    Item BagItem = GetItemByID(OpenBackpacks[i].ID);
                    int Backpack_OffsetY_Scroll = (y * (OpenBackpacks[i].Height + Coordinates.Step));
                    DrawOutlinedString(font, BagItem.Name + ":", new Vector2((float)Coordinates.Step * UI_Start, (float)(((OffsetY) - 16) + Backpack_OffsetY_Scroll)), Color.White);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, r);
                    Rectangle scrollRect = new Rectangle();
                    scrollRect.Width = 192;
                    scrollRect.Height = OpenBackpacks[i].Height;
                    scrollRect.X = Coordinates.Step * UI_Start;
                    scrollRect.Y = ((OffsetY)) + Backpack_OffsetY_Scroll;
                    spriteBatch.GraphicsDevice.ScissorRectangle = scrollRect;
                    spriteBatch.Draw(GetSpriteByName("UI_Scrollbar"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)((OffsetY) + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.End();
                    spriteBatch.Begin();
                    spriteBatch.Draw(GetSpriteByName("UI_CloseButton"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)((OffsetY) - 10 + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(GetSpriteByName("UI_UpArrow"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)((OffsetY) + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(GetSpriteByName("UI_DownArrow"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)((OffsetY) + OpenBackpacks[i].Height - 10 + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, r);
                    Rectangle currentRect = new Rectangle();
                    int Backpack_OffsetY = (y * (OpenBackpacks[i].Height + Coordinates.Step) - OpenBackpacks[i].Scroll.CurrentStep * 10);
                    currentRect.Width = 192;
                    currentRect.Height = OpenBackpacks[i].Height;
                    currentRect.X = Coordinates.Step * UI_Start;
                    currentRect.Y = ((OffsetY)) + Backpack_OffsetY_Scroll;
                    spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;

                    // TODO: Change when writing multiplayer code
                    spriteBatch.Draw(GetSpriteByName("UI_BackpackBackground"), new Vector2((float)Coordinates.Step * UI_Start, (float)((OffsetY) + Backpack_OffsetY)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    OpenBackpacks[i].Position = new Coordinates(Coordinates.Step * UI_Start, ((OffsetY) + Backpack_OffsetY - (y * 16))); //(y * OpenBackpacks[i].Height)));
                    OpenBackpacks[i].ClosePosition = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) - 10 + Backpack_OffsetY_Scroll));
                    OpenBackpacks[i].Scroll.DownArrow = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) + OpenBackpacks[i].Height - 10 + Backpack_OffsetY_Scroll));
                    OpenBackpacks[i].Scroll.Position = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) + Backpack_OffsetY_Scroll + 10));
                    OpenBackpacks[i].Scroll.UpArrow = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) + Backpack_OffsetY_Scroll));
                    OpenBackpacks[i].Scroll.MaxHeight = OpenBackpacks[i].MaxHeight;
                    List<Item> containerItems = OpenBackpacks[i].GetItems();
                    int j = 0;
                    int addedY = 0;
                    for (int c = 0; c < containerItems.Count; c++)
                    {
                        if (j % 4 == 0)
                        {
                            j = 0;
                            if (c > 0)
                                addedY++;
                        }
                        containerItems[c].Position = new Coordinates(((Coordinates.Step * (UI_Start + j)) + 4 + (8 * j)), (((OffsetY) + (Coordinates.Step * addedY)) + 4 + (8 * addedY) + Backpack_OffsetY));
                        spriteBatch.Draw(GetSpriteByID(containerItems[c].SpriteID), new Vector2((float)((Coordinates.Step * (UI_Start + j)) + 4 + (8 * j)), (float)(((OffsetY) + (Coordinates.Step * addedY)) + 4 + (8 * addedY) + Backpack_OffsetY)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        if (GetItemFromBagCoordinates((int)cursorPos.X, (int)cursorPos.Y).ID == containerItems[c].ID)
                        {
                            spriteBatch.Draw(GetSpriteByName("attackbox"), new Vector2((float)((Coordinates.Step * (UI_Start + j)) + 4 + (8 * j)), (float)(((OffsetY) + (Coordinates.Step * addedY)) + 4 + (8 * addedY) + Backpack_OffsetY)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        j++;
                    }
                }
                else
                {
                    y = i;
                    spriteBatch.End();
                    spriteBatch.Begin();
                    Item BagItem = GetItemByID(OpenBackpacks[i].ID);
                    OffsetY = OpenBackpacks[y - 1].Position.Y;
                    int Backpack_OffsetY = OpenBackpacks[y - 1].Height + (OpenBackpacks[i - 1].Scroll.CurrentStep * 10) + (y * Coordinates.Step) - (16 * y) - (OpenBackpacks[i].Scroll.CurrentStep * 10);
                    int Backpack_OffsetY_Scroll = (OpenBackpacks[y - 1].Height + OpenBackpacks[i - 1].Scroll.CurrentStep * 10) + (y * Coordinates.Step) - (16 * y);
                    DrawOutlinedString(font, BagItem.Name + ":", new Vector2((float)Coordinates.Step * UI_Start, (float)((OffsetY - 16) + Backpack_OffsetY_Scroll)), Color.White);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, r);
                    Rectangle scrollRect = new Rectangle();
                    scrollRect.Width = 192;
                    scrollRect.Height = OpenBackpacks[i].Height;
                    scrollRect.X = Coordinates.Step * UI_Start;
                    scrollRect.Y = (OffsetY) + Backpack_OffsetY_Scroll;
                    spriteBatch.GraphicsDevice.ScissorRectangle = scrollRect;
                    spriteBatch.Draw(GetSpriteByName("UI_Scrollbar"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)(OffsetY + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.End();
                    spriteBatch.Begin();
                    spriteBatch.Draw(GetSpriteByName("UI_CloseButton"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)((OffsetY) - 10 + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(GetSpriteByName("UI_UpArrow"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)((OffsetY) + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(GetSpriteByName("UI_DownArrow"), new Vector2((float)(Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, (float)((OffsetY) + OpenBackpacks[i].Height - 10 + Backpack_OffsetY_Scroll)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, r);
                    Rectangle currentRect = new Rectangle();
                    currentRect.Width = 192;
                    currentRect.Height = OpenBackpacks[i].Height;
                    currentRect.X = Coordinates.Step * UI_Start;
                    currentRect.Y = ((OffsetY)) + Backpack_OffsetY_Scroll;
                    spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;

                    // TODO: Change when writing multiplayer code
                    spriteBatch.Draw(GetSpriteByName("UI_BackpackBackground"), new Vector2((float)Coordinates.Step * UI_Start, (float)((OffsetY) + Backpack_OffsetY)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    OpenBackpacks[i].Position = new Coordinates(Coordinates.Step * UI_Start, ((OffsetY) + Backpack_OffsetY - (y * 16))); //(y * OpenBackpacks[i].Height)));
                    OpenBackpacks[i].ClosePosition = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) - 10 + Backpack_OffsetY_Scroll));
                    OpenBackpacks[i].Scroll.Position = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) + Backpack_OffsetY_Scroll));
                    OpenBackpacks[i].Scroll.UpArrow = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) + Backpack_OffsetY_Scroll));
                    OpenBackpacks[i].Scroll.Position = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) + Backpack_OffsetY_Scroll + 10));
                    OpenBackpacks[i].Scroll.DownArrow = new Coordinates((Coordinates.Step * (UI_Start + 4)) + ScrollBarOffset, ((OffsetY) + OpenBackpacks[i].Height - 10 + Backpack_OffsetY_Scroll));
                    OpenBackpacks[i].Scroll.MaxHeight = OpenBackpacks[i].MaxHeight;
                    List<Item> containerItems = OpenBackpacks[i].GetItems();
                    int j = 0;
                    int addedY = 0;
                    for (int c = 0; c < containerItems.Count; c++)
                    {
                        if (j % 4 == 0)
                        {
                            j = 0;
                            if (c > 0)
                                addedY++;
                        }
                        containerItems[c].Position = new Coordinates(((Coordinates.Step * (UI_Start + j)) + 4 + (8 * j)), ((OffsetY + (Coordinates.Step * addedY)) + 4 + (8 * addedY) + Backpack_OffsetY));
                        spriteBatch.Draw(GetSpriteByID(containerItems[c].SpriteID), new Vector2((float)((Coordinates.Step * (UI_Start + j)) + 4 + (8 * j)), (float)((OffsetY + (Coordinates.Step * addedY)) + 4 + (8 * addedY) + Backpack_OffsetY)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        if (GetItemFromBagCoordinates((int)cursorPos.X, (int)cursorPos.Y).ID == containerItems[c].ID)
                        {
                            spriteBatch.Draw(GetSpriteByName("attackbox"), new Vector2((float)((Coordinates.Step * (UI_Start + j)) + 4 + (8 * j)), (float)((OffsetY + (Coordinates.Step * addedY)) + 4 + (8 * addedY) + Backpack_OffsetY)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        j++;
                    }
                }

                //y += 1;
            }
            spriteBatch.End();
            spriteBatch.Begin();
        }

        private void DrawTextPopUps(GameTime gameTime)
        {
            int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            if (TextPopUps.Count > 0)
            {
                for (int f = 0; f < TextPopUps.Count; f++)
                {
                    DrawOutlinedString(font, TextPopUps[f].Text, new Vector2((float)Coordinates.Step * 10, (float)Coordinates.Step * 9 + (f * 16)), Color.LimeGreen);
                }

                for (int j = 0; j < TextPopUps.Count; j++)
                {
                    if (currentTime > TextPopUps[j].EndTime)
                    {
                        TextPopUps.RemoveAt(j);
                    }
                }
            }
        }

        private void DrawChatPopUps(GameTime gameTime)
        {
            int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            if (ChatPopUps.Count > 0)
            {
                for (int f = 0; f < ChatPopUps.Count; f++)
                {
                    DrawOutlinedString(font, ChatPopUps[f].Text, new Vector2((float)(ChatPopUps[f].Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)((ChatPopUps[f].Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step) + ((f + 1) * 3)), Color.Yellow);
                }

                for (int j = 0; j < ChatPopUps.Count; j++)
                {
                    if (currentTime > ChatPopUps[j].EndTime)
                    {
                        ChatPopUps.RemoveAt(j);
                    }
                }
            }
        }

        private void DrawCreatures(int z = 0)
        {
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                if (map.Creatures[i].Position.Z == z)
                {
                    if (map.Creatures[i].Health > 0)
                    {
                        spriteBatch.Draw(GetSpriteByID(map.Creatures[i].SpriteID), new Vector2((float)(map.Creatures[i].Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.Creatures[i].Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        if (map.Creatures[i].ID == map.Players[0].TargetID)
                        {
                            // Draw attackbox
                            spriteBatch.Draw(GetSpriteByID(3), new Vector2((float)(map.Creatures[i].Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.Creatures[i].Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }

        private void DrawNPCs(int z = 0)
        {
            for (int i = 0; i < map.NPCs.Count; i++)
            {
                if (map.NPCs[i].Position.Z == z)
                {
                    spriteBatch.Draw(GetSpriteByID(map.NPCs[i].SpriteID), new Vector2((float)(map.NPCs[i].Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.NPCs[i].Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        private void DrawCreatureNames(int z = 0)
        {
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                if (map.Creatures[i].Health > 0 && map.Creatures[i].Position.Z == z)
                {
                    DrawOutlinedString(font, map.Creatures[i].Name, new Vector2((float)(map.Creatures[i].Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.Creatures[i].Position.Y - map.Players[0].Position.Y + 8 - 0.5) * Coordinates.Step), Color.White);
                }
            }
        }

        private void DrawNPCNames(int z = 0)
        {
            for (int i = 0; i < map.NPCs.Count; i++)
            {
                if (map.NPCs[i].Position.Z == z)
                {
                    DrawOutlinedString(font, map.NPCs[i].Name, new Vector2((float)(map.NPCs[i].Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(map.NPCs[i].Position.Y - map.Players[0].Position.Y + 8 - 0.5) * Coordinates.Step), Color.White);
                }
            }
        }

        private void DrawDamageDone(GameTime gameTime)
        {
            int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            if (dmgDone.Count > 0)
            {
                for (int f = 0; f < dmgDone.Count; f++)
                {
                    if (dmgDone[f].Healing)
                    {
                        DrawOutlinedString(font, dmgDone[f].damageDealt.ToString(), new Vector2((float)((dmgDone[f].creature.Position.X - map.Players[0].Position.X + 12) * Coordinates.Step) + 2, (float)((dmgDone[f].creature.Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step) + (float)dmgDone[f].OffsetY(currentTime)), Color.LimeGreen);
                    }
                    else
                    {
                        DrawOutlinedString(font, dmgDone[f].damageDealt.ToString(), new Vector2((float)((dmgDone[f].creature.Position.X - map.Players[0].Position.X + 12) * Coordinates.Step) + 2, (float)((dmgDone[f].creature.Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step) + (float)dmgDone[f].OffsetY(currentTime)), Color.Red);
                    }
                }

                for (int j = 0; j < dmgDone.Count; j++)
                {
                    if (currentTime > dmgDone[j].EndTime)
                    {
                        dmgDone.RemoveAt(j);
                    }
                }
            }
        }

        private void DrawSpells(GameTime gameTime)
        {
            for (int i = 0; i < SpellDamage.Count; i++)
            {
                DrawSpell(GetSpellByID(SpellDamage[i].ID), gameTime, i);
            }
        }

        private void DrawSpell(Spell spell, GameTime gameTime, int index)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds < SpellDamage[index].EndTime)
            {
                if (!spell.TargetSpell)
                {
                    for (int i = 0; i < spell.Area.Length / 3; i++)
                    {
                        for (int j = 0; j < spell.Area.Length / 3; j++)
                        {
                            if (spell.Area[i + j])
                            {
                                spriteBatch.Draw(spell.Sprite, new Vector2((float)(SpellDamage[index].Position.X - map.Players[0].Position.X + 11 + i) * Coordinates.Step, (float)(SpellDamage[index].Position.Y - map.Players[0].Position.Y + 7 + j) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }
                else
                {
                    spriteBatch.Draw(spell.Sprite, new Vector2((float)(SpellDamage[index].creature.Position.X - map.Players[0].Position.X + 12) * Coordinates.Step, (float)(SpellDamage[index].creature.Position.Y - map.Players[0].Position.Y + 8) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
            else
            {
                SpellDamage.RemoveAt(index);
            }
        }

        public void DrawUI()
        {
            for (int i = 0; i < listUI.Count; i++)
            {
                spriteBatch.Draw(listUI[i].Sprite, new Vector2((float)listUI[i].Position.X, (float)listUI[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < spellUI.Count; i++)
            {
                spriteBatch.Draw(spellUI[i].Sprite, new Vector2((float)spellUI[i].Position.X, (float)spellUI[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public void DrawOutlinedString(SpriteFont font, string Text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, Text, new Vector2(position.X - 1, position.Y), Color.Black);
            spriteBatch.DrawString(font, Text, new Vector2(position.X + 1, position.Y), Color.Black);
            spriteBatch.DrawString(font, Text, new Vector2(position.X, position.Y + 1), Color.Black);
            spriteBatch.DrawString(font, Text, new Vector2(position.X, position.Y - 1), Color.Black);
            spriteBatch.DrawString(font, Text, position, color);
        }

        private Item GetItemFromBagCoordinates(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                OpenBackpacks[i].Sort();
                List<Item> CurrentBackpack = OpenBackpacks[i].GetItems();
                int MaxItems = ((OpenBackpacks[i].Height / 32) * 4);
                MaxItems = CurrentBackpack.Count;
                if (CurrentBackpack.Count < MaxItems)
                {
                    MaxItems = CurrentBackpack.Count;
                }
                for (int c = 0; c < MaxItems; c++)
                {
                    tmpRect.X = CurrentBackpack[c].Position.X;
                    tmpRect.Y = CurrentBackpack[c].Position.Y;
                    if (tmpRect.Y < OpenBackpacks[i].Position.Y + OpenBackpacks[i].Height + (OpenBackpacks[i].Scroll.CurrentStep * 10))
                    {
                        if (tmpRect.Y > OpenBackpacks[i].Position.Y + (OpenBackpacks[i].Scroll.CurrentStep * 10) - 32)
                        {
                            if (tmpRect.Contains(x, y))
                            {
                                return CurrentBackpack[c];
                            }
                        }
                    }
                }
            }
            return new Item();
        }

        private void ResizeBag(Backpack Bag)
        {
            int y = Mouse.GetState().Y;
            int MaxHeight = Bag.MaxHeight;
            int MinHeight = 40;
            int Height = 0;

            Height = y - Bag.Position.Y - (GetBackpackNumber(Bag) * 10);

            if (Bag.ID != -1)
            {
                if (Height < MinHeight)
                {
                    Height = MinHeight;
                }
                else if (Height > MaxHeight)
                {
                    Height = MaxHeight;
                }
                if (Height + Bag.Scroll.CurrentStep * 10 > MaxHeight)
                {
                    Bag.Scroll.CurrentStep = 0;
                }
                Bag.Height = Height;
            }

        }

        private void ScrollBag(Backpack Bag)
        {
            int y = Mouse.GetState().Y;

            if (y > OldMousePos.Y)
            {
                ScrollDownBag(Bag);
                if (y != OldMousePos.Y)
                    OldMousePos = Mouse.GetState();
            }
            else if (y < OldMousePos.Y)
            {
                ScrollUpBag(Bag);
                if (y != OldMousePos.Y)
                    OldMousePos = Mouse.GetState();
            }

        }

        private int GetBackpackNumber(Backpack bag)
        {
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                if (OpenBackpacks[i].ID == bag.ID)
                {
                    return i;
                }
            }
            return 0;
        }

        private Backpack GetBagFromCoordinates(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Width = 160;
            tmpRect.Height = 200;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                tmpRect.Height = OpenBackpacks[i].Height + (16 * i);
                tmpRect.X = OpenBackpacks[i].Position.X;
                tmpRect.Y = OpenBackpacks[i].Position.Y + (OpenBackpacks[i].Scroll.CurrentStep * 10);
                if (tmpRect.Contains(x, y))
                {
                    return OpenBackpacks[i];
                }
            }
            return new Backpack();
        }

        /*private Backpack GetBagFromResizeCoordinates(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Width = 160;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                tmpRect.Height = OpenBackpacks[i].Height + 16;
                tmpRect.X = OpenBackpacks[i].Position.X;
                tmpRect.Y = OpenBackpacks[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return OpenBackpacks[i];
                }
            }
            return new Backpack();
        }*/

        private Backpack GetCloseButtonFromCoordinates(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Width = 10;
            tmpRect.Height = 10;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                tmpRect.X = OpenBackpacks[i].ClosePosition.X;
                tmpRect.Y = OpenBackpacks[i].ClosePosition.Y;
                if (tmpRect.Contains(x, y))
                {
                    return OpenBackpacks[i];
                }
            }
            return new Backpack();
        }

        private Backpack GetUpArrow(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Width = 10;
            tmpRect.Height = 10;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                tmpRect.X = OpenBackpacks[i].Scroll.UpArrow.X;
                tmpRect.Y = OpenBackpacks[i].Scroll.UpArrow.Y;
                if (tmpRect.Contains(x, y))
                {
                    return OpenBackpacks[i];
                }
            }
            return new Backpack();
        }

        private Backpack GetDownArrow(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Width = 10;
            tmpRect.Height = 10;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                tmpRect.X = OpenBackpacks[i].Scroll.DownArrow.X;
                tmpRect.Y = OpenBackpacks[i].Scroll.DownArrow.Y;
                if (tmpRect.Contains(x, y))
                {
                    return OpenBackpacks[i];
                }
            }
            return new Backpack();
        }

        private Backpack GetScrollbar(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Width = 10;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                tmpRect.X = OpenBackpacks[i].Scroll.Position.X;
                tmpRect.Y = OpenBackpacks[i].Scroll.Position.Y;
                tmpRect.Height = OpenBackpacks[i].Height - 20;
                if (tmpRect.Contains(x, y))
                {
                    return OpenBackpacks[i];
                }
            }
            return new Backpack();
        }

        private bool IsMouseAbleToResizeBag(Vector2 CursorPos)
        {
            Rectangle tmpRect;
            tmpRect.Width = 160;
            tmpRect.Height = 5;
            for (int i = 0; i < OpenBackpacks.Count; i++)
            {
                if (OpenBackpacks[i].Position != null)
                {
                    tmpRect.X = OpenBackpacks[i].Position.X;
                    tmpRect.Y = OpenBackpacks[i].Position.Y + OpenBackpacks[i].Height - 5 + (16 * i) + (OpenBackpacks[i].Scroll.CurrentStep * 10);
                    if (tmpRect.Contains(CursorPos.X, CursorPos.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Backpack GetBagFromID(int id)
        {
            for (int i = 0; i < map.WorldItems.Count; i++)
            {
                if (map.WorldItems[i].ID == id)
                {
                    return map.WorldItems[i].Container;
                }
            }
            return new Backpack();
        }

        /* TODO: Write this function
        private Item GetItemFromBagSlot(int Slot, Item Bag)
        {
            return new Item();
        }*/

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

        private bool OutOfClientBoundaries(Coordinates Coordinates)
        {
            return (Coordinates.X > Coordinates.Step * 24 || Coordinates.Y > Coordinates.Step * 16);
        }

        public Texture2D GetSpriteByName(string Name)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].SpriteName == Name)
                {
                    return spriteList[i].Sprite;
                }

            }
            return spriteList[0].Sprite;
        }

        public int GetSpriteIDByName(string Name)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].SpriteName == Name)
                {
                    return spriteList[i].ID;
                }

            }
            return spriteList[0].ID;
        }

        private SpriteObject GetSpriteObjectByID(int id)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].ID == id)
                {
                    return spriteList[i];
                }

            }
            return spriteList[0];
        }

        private Creature GetCreatureByMousePosition(int x, int y)
        {
            /*Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;*/
            int newX = map.Players[0].Position.X + (x / 32) - 12;
            int newY = map.Players[0].Position.Y + (y / 32) - 8;

            //Window.Title = newX + " " + newY;
            //Window.Title = map.GetTopItemFromTile(new Coordinates(newX, newY)).Z_order.ToString();
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                /*tmpRect.X = map.Creatures[i].Position.X;
                tmpRect.Y = map.Creatures[i].Position.Y;
                if (tmpRect.Contains(x, y) && map.Creatures[i].Health > 0)
                {
                    return map.Creatures[i];
                }*/
                if (map.Creatures[i].Position.X == newX && map.Creatures[i].Position.Y == newY && map.Creatures[i].Health > 0)
                {
                    return map.Creatures[i];
                }
            }
            return new Creature("null");
        }

        private Item GetItemByMousePosition(int x, int y)
        {
            Item tmpitem = GetItemFromBagCoordinates(x, y);
            if (tmpitem.ID != -1)
            {
                return GetItemFromBagCoordinates(x, y);
            }

            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            int newX = map.Players[0].Position.X + (x / 32) - 12;
            int newY = map.Players[0].Position.Y + (y / 32) - 8;

            for (int i = 0; i < map.WorldItems.Count; i++)
            {
                tmpRect.X = map.WorldItems[i].Position.X;
                tmpRect.Y = map.WorldItems[i].Position.Y;
                if (tmpRect.Contains(x, y) && map.WorldItems[i].WearingPlayerID == map.Players[0].ID)
                {
                    return map.WorldItems[i];
                }
            }

            /*for(int i = 0; i < map.WorldItems.Count; i++)
            {
                if(newX == map.WorldItems[i].Position.X && newY == map.WorldItems[i].Position.Y)
                {
                    return map.WorldItems[i];
                }
            }*/
            Item topItem = map.GetTopItemFromTile(new Coordinates(newX, newY, map.Players[0].Position.Z));
            if (topItem.ID != -1)
            {
                return topItem;
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

        private Spell GetSpellByMousePosition(int x, int y)
        {
            Rectangle tmpRect;
            tmpRect.Height = 32;
            tmpRect.Width = 32;
            for (int i = 0; i < spellUI.Count; i++)
            {
                tmpRect.X = spellUI[i].Position.X;
                tmpRect.Y = spellUI[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return GetSpellByID(spellUI[i].SpellID);
                }
            }
            return new Spell();
        }

        private Spell GetSpellByID(int ID)
        {
            for (int i = 0; i < Spells.Count; i++)
            {
                if (Spells[i].ID == ID) return Spells[i];
            }

            return new Spell();
        }

        private Item GetItemByID(int ID)
        {
            for (int i = 0; i < map.WorldItems.Count; i++)
            {
                if (map.WorldItems[i].ID == ID) return map.WorldItems[i];
            }

            return new Item();
        }

        private Item GetItemFromListByID(int ItemID)
        {
            for (int i = 0; i < map.ItemList.Count; i++)
            {
                if (map.ItemList[i].ID == ItemID)
                {
                    return map.ItemList[i];
                }
            }

            return new Item();
        }

        private Item GetItemFromListByName(string ItemName)
        {
            for (int i = 0; i < map.ItemList.Count; i++)
            {
                if (map.ItemList[i].Name == ItemName)
                {
                    return map.ItemList[i];
                }
            }

            return new Item();
        }


        private UI GetListUIByItemSlot(string _ItemSlot)
        {
            for (int i = 0; i < listUI.Count; i++)
            {
                if (listUI[i].Name == _ItemSlot)
                {
                    return listUI[i];
                }

            }

            return new UI();
        }

        private void SaveWorld()
        {
            /*FileManager saveFile = new FileManager();
            saveFile.AddField("ID", map.Players[0].ID.ToString());*/
            string saveString = "";
            StreamWriter writer = new StreamWriter("Content\\World.save");

            for (int i = 0; i < map.Players.Count; i++)
            {
                saveString = map.Players[i].ID + "|" + map.Players[i].Name + "|" + map.Players[i].EquippedItems.ToString() + "|" + map.Players[i].Level + "|" + map.Players[i].Experience + "|" + map.Players[i].MaxHealth + "|" + map.Players[i].MaxMana + "|" + map.Players[i].MagicStrength + "|" + map.Players[i].ManaSpent + "|" + map.Players[i].Position.X + "|" + map.Players[i].Position.Y + "|" + map.Players[i].Position.Z + "|" + map.Players[i].SpriteID;
                writer.WriteLine(saveString);
            }
            writer.Close();
        }
        private void LoadWorld()
        {
            FileInfo file = new FileInfo("Content\\World.save");

            if (file.Exists)
            {
                StreamReader read = new StreamReader("Content\\World.save");

                int currentArg = 0;

                string[] currentPlayer = read.ReadLine().Split("|".ToCharArray());

                int Player_ID = int.Parse(currentPlayer[currentArg++]); // 0
                string Player_Name = currentPlayer[currentArg++]; // 1
                string[] EquippedItems = currentPlayer[currentArg++].Split(",".ToCharArray()); // 2
                int Player_Level = int.Parse(currentPlayer[currentArg++]); // 3
                int Player_Experience = int.Parse(currentPlayer[currentArg++]); // 4
                int Player_Health = int.Parse(currentPlayer[currentArg++]); // 5
                int Player_Mana = int.Parse(currentPlayer[currentArg++]); // 6
                int Player_MagicStrength = int.Parse(currentPlayer[currentArg++]); // 7
                int Player_ManaSpent = int.Parse(currentPlayer[currentArg++]); // 8
                int Player_X = int.Parse(currentPlayer[currentArg++]); // 9
                int Player_Y = int.Parse(currentPlayer[currentArg++]); // 10
                int Player_Z = int.Parse(currentPlayer[currentArg++]); // 11
                int Player_SpriteID = int.Parse(currentPlayer[currentArg++]); // 12

                // TODO: Add the possibility of loading more than one player
                map.Players.Add(new Player(Player_Name, new Coordinates(Player_X, Player_Y, Player_Z), Player_Health, Player_Mana, Player_Level, Player_ID));
                int currentID = map.Players.Count - 1;

                map.Players[currentID].Experience = Player_Experience;
                map.Players[currentID].MagicStrength = Player_MagicStrength;
                map.Players[currentID].ManaSpent = Player_ManaSpent;
                map.Players[currentID].SpriteID = Player_SpriteID;

                if (int.Parse(EquippedItems[0]) > 0)
                {
                    Item newItem = map.CreateWorldItemFromListItem(int.Parse(EquippedItems[0]));
                    newItem.Slot = ItemSlot.LeftHand;
                    newItem.WearingPlayerID = Player_ID;
                    newItem.Position = GetListUIByItemSlot(ItemSlot.LeftHand).Position;
                    map.Players[currentID].EquippedItems.LeftHand = newItem;
                    GetListUIByItemSlot(ItemSlot.LeftHand).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[1]) > 0)
                {
                    Item newItem = map.CreateWorldItemFromListItem(int.Parse(EquippedItems[1]));
                    newItem.Slot = ItemSlot.RightHand;
                    newItem.WearingPlayerID = Player_ID;
                    newItem.Position = GetListUIByItemSlot(ItemSlot.RightHand).Position;
                    map.Players[currentID].EquippedItems.RightHand = newItem;
                    GetListUIByItemSlot(ItemSlot.RightHand).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[2]) > 0)
                {
                    Item newItem = map.CreateWorldItemFromListItem(int.Parse(EquippedItems[2]));
                    newItem.Slot = ItemSlot.Helmet;
                    newItem.WearingPlayerID = Player_ID;
                    newItem.Position = GetListUIByItemSlot(ItemSlot.Helmet).Position;
                    map.Players[currentID].EquippedItems.Helmet = newItem;
                    GetListUIByItemSlot(ItemSlot.Helmet).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[3]) > 0)
                {
                    Item newItem = map.CreateWorldItemFromListItem(int.Parse(EquippedItems[3]));
                    newItem.Slot = ItemSlot.Armor;
                    newItem.WearingPlayerID = Player_ID;
                    newItem.Position = GetListUIByItemSlot(ItemSlot.Armor).Position;
                    map.Players[currentID].EquippedItems.Armor = newItem;
                    GetListUIByItemSlot(ItemSlot.Armor).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[4]) > 0)
                {
                    Item newItem = map.CreateWorldItemFromListItem(int.Parse(EquippedItems[4]));
                    newItem.Slot = ItemSlot.Legs;
                    newItem.WearingPlayerID = Player_ID;
                    newItem.Position = GetListUIByItemSlot(ItemSlot.Legs).Position;
                    map.Players[currentID].EquippedItems.Legs = newItem;
                    GetListUIByItemSlot(ItemSlot.Legs).Sprite = GetSpriteByName("UI_background");
                }

                // TODO: Finish the code below and add more error checks
                string BagsContainingItems = EquippedItems[5].Substring("Bags:".Length, EquippedItems[5].IndexOf(".") - "Bags:".Length);
                string ContainedItems = EquippedItems[5].Substring(EquippedItems[5].IndexOf("Items:") + "Items:".Length);
                string[] Parameters;
                if (BagsContainingItems.IndexOf("+") > 0)
                {

                    // Create bags before adding items to them

                    string[] Bags = BagsContainingItems.Split("+".ToCharArray());
                    for (int i = 0; i < Bags.Length; i++)
                    {
                        Parameters = Bags[i].Split(";".ToCharArray());
                        if (Parameters[2] == "-1")
                        {
                            Item newItem = map.CreateWorldItemFromListItem(int.Parse(Parameters[0]));
                            newItem.ID = int.Parse(Parameters[1]);
                            newItem.Slot = ItemSlot.Bag;
                            newItem.WearingPlayerID = Player_ID;
                            newItem.Position = GetListUIByItemSlot(ItemSlot.Bag).Position;
                            map.Players[currentID].EquippedItems.Bag = newItem;
                            GetListUIByItemSlot(ItemSlot.Bag).Sprite = GetSpriteByName("UI_background");
                        }
                        else
                        {
                            Item newItem = map.CreateWorldItemFromListItem(int.Parse(Parameters[0]));
                            newItem.ID = int.Parse(Parameters[1]);
                            newItem.Slot = null;
                            newItem.WearingPlayerID = Player_ID;
                            GetBagFromID(int.Parse(Parameters[2])).AddItem(newItem);
                        }
                    }

                    // Add items to bags

                    if (ContainedItems.IndexOf("+") > 0)
                    {
                        string[] ItemsInBags = ContainedItems.Split("+".ToCharArray());
                        for (int c = 0; c < ItemsInBags.Length; c++)
                        {
                            Parameters = ItemsInBags[c].Split(";".ToCharArray());
                            Item newBagItem = map.CreateWorldItemFromListItem(int.Parse(Parameters[0]));
                            newBagItem.Slot = null;
                            newBagItem.WearingPlayerID = Player_ID;
                            GetBagFromID(int.Parse(Parameters[1])).AddItem(newBagItem);
                        }
                    }
                    else
                    {
                        Parameters = ContainedItems.Split(";".ToCharArray());
                        Item newBagItem = map.CreateWorldItemFromListItem(int.Parse(Parameters[0]));
                        newBagItem.Slot = null;
                        newBagItem.WearingPlayerID = Player_ID;
                        GetBagFromID(int.Parse(Parameters[1])).AddItem(newBagItem);
                    }

                }
                else
                {
                    Parameters = BagsContainingItems.Split(";".ToCharArray());
                    Item newItem = map.CreateWorldItemFromListItem(int.Parse(Parameters[0]));
                    newItem.Slot = ItemSlot.Bag;
                    newItem.WearingPlayerID = Player_ID;
                    newItem.Position = GetListUIByItemSlot(ItemSlot.Bag).Position;
                    map.Players[currentID].EquippedItems.Bag = newItem;
                    GetListUIByItemSlot(ItemSlot.Bag).Sprite = GetSpriteByName("UI_background");

                    if (ContainedItems.IndexOf("+") > 0)
                    {
                        string[] ItemsInBags = ContainedItems.Split("+".ToCharArray());
                        for (int c = 0; c < ItemsInBags.Length; c++)
                        {
                            Parameters = ItemsInBags[c].Split(";".ToCharArray());
                            Item newBagItem = map.CreateWorldItemFromListItem(int.Parse(Parameters[0]));
                            newBagItem.WearingPlayerID = Player_ID;
                            newBagItem.Slot = null;
                            //newBagItem.Position = GetListUIByItemSlot(ItemSlot.Bag).Position;
                            map.Players[currentID].EquippedItems.Bag.Container.AddItem(newBagItem);
                        }
                    }
                    else
                    {
                        Parameters = ContainedItems.Split(";".ToCharArray());
                        Item newBagItem = map.CreateWorldItemFromListItem(int.Parse(Parameters[0]));
                        newBagItem.WearingPlayerID = Player_ID;
                        newBagItem.Slot = null;
                        GetBagFromID(int.Parse(Parameters[1])).AddItem(newBagItem);
                    }
                }
                //TextPopUps.Add(new Animation(10000, 0, 1, 0, ContainedItems));
                //saveString = Players[i].ID + "|" + Players[i].Name + "|" + Players[i].EquippedItems.ToString() + "|" + Players[i].Level + "|" + Players[i].Experience + "|" + Players[i].MaxHealth + "|" + Players[i].MaxMana + "|" + Players[i].MagicStrength + "|" + Players[i].ManaSpent + "|" + Players[i].Position.X + "|" + Players[i].Position.Y + "|" + Players[i].SpriteID;

                read.Close();
            }
        }
    }
}