using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.IO;

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
        List<DamageObject> dmgDone = new List<DamageObject>();
        List<Spell> Spells = new List<Spell>();
        List<UI> spellUI = new List<UI>();

        List<DamageObject> SpellDamage = new List<DamageObject>();

        private int currentMouse = 0;

        SpriteFont font;

        private bool RightClicked = false;
        private bool LeftClicked = false;

        private int leftClickTime;

        private const int Direction_North = 1;
        private const int Direction_East = 2;
        private const int Direction_South = 3;
        private const int Direction_West = 4;

        private int Walking_Direction;
        private int TimeOfLastMovement;
        private Keys lastPressedKey;
        private Keys mostRecentKey;
        private bool Walking = false;

        private bool regenerated = false;

        private bool respawned = false;
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

            map = new Map(new Coordinates(Window.ClientBounds.Width, Window.ClientBounds.Height));
            //map.Players.Add(new Player("Aephirus", new Coordinates(0, 0), 150, 100, 1, 1));
            //player1 = new Player("Aephirus", new Coordinates(0, 0));

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

            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\player"), spriteList.Count + 1, Entity.CreatureEntity, "player"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile"), spriteList.Count + 1, Entity.TileEntity, "tile"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\attackbox"), spriteList.Count + 1, Entity.UnknownEntity, "attackbox"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\tile2"), spriteList.Count + 1, Entity.TileEntity, "tile2"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_ui"), spriteList.Count + 1, Entity.SpellEntity, "UI_FistSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_animation"), spriteList.Count + 1, Entity.SpellEntity, "Spell_FistSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_ui"), spriteList.Count + 1, Entity.SpellEntity, "UI_HealSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_animation"), spriteList.Count + 1, Entity.SpellEntity, "Spell_HealSpell"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\goldenarmor"), spriteList.Count + 1, Entity.ItemEntity, "Golden Armor"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\helmet"), spriteList.Count + 1, Entity.UnknownEntity, ItemSlot.Helmet));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\torso"), spriteList.Count + 1, Entity.UnknownEntity, ItemSlot.Armor));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fist"), spriteList.Count + 1, Entity.UnknownEntity, ItemSlot.LeftHand));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fist"), spriteList.Count + 1, Entity.UnknownEntity, ItemSlot.RightHand));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\hornedhelmet"), spriteList.Count + 1, Entity.ItemEntity, "Horned Helmet"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\UI_background"), spriteList.Count + 1, Entity.UnknownEntity, "UI_background"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\magicsword"), spriteList.Count + 1, Entity.ItemEntity, "Magic Sword"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\ghost"), spriteList.Count + 1, Entity.CreatureEntity, "ghost"));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\legs"), spriteList.Count + 1, Entity.UnknownEntity, ItemSlot.Legs));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\masterlegs"), spriteList.Count + 1, Entity.ItemEntity, "Master Legs"));

            //map.ItemList.Add(new Item("Sword of magicnezz", new Coordinates(3 * Coordinates.Step, 3 * Coordinates.Step), 1, 1, 60, 1));
            map.ItemList.Add(new Item("Magic Sword", ItemSlot.LeftHand, new Coordinates(3 * Coordinates.Step, 5 * Coordinates.Step), GetSpriteIDByName("Magic Sword"), GetSpriteIDByName("Magic Sword"), 60, 1));
            map.ItemList.Add(new Item("Golden Armor", ItemSlot.Armor, new Coordinates(2 * Coordinates.Step, 2 * Coordinates.Step), GetSpriteIDByName("Golden Armor"), GetSpriteIDByName("Golden Armor"), 0, 15));
            map.ItemList.Add(new Item("Horned Helmet", ItemSlot.Helmet, new Coordinates(3 * Coordinates.Step, 2 * Coordinates.Step), GetSpriteIDByName("Horned Helmet"), GetSpriteIDByName("Horned Helmet"), 0, 12));
            map.ItemList.Add(new Item("Master Legs", ItemSlot.Legs, new Coordinates(3 * Coordinates.Step, 2 * Coordinates.Step), GetSpriteIDByName("Master Legs"), GetSpriteIDByName("Master Legs"), 0, 10));

            Spells.Add(new Spell(new bool[]
            {true, true, true,
            true, true, true,
            true, true, true}
            , 50, GetSpriteByName("Spell_FistSpell"), 20, false, false, 1));

            Spells.Add(new Spell(new bool[] { true }, 50, GetSpriteByName("Spell_HealSpell"), 5, true, true, 2));

            listUI.Add(new UI(GetSpriteByName(ItemSlot.LeftHand), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 27, Coordinates.Step * 0), ItemSlot.LeftHand));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.RightHand), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 28, Coordinates.Step * 0), ItemSlot.RightHand));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.Helmet), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 29, Coordinates.Step * 0), ItemSlot.Helmet));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.Armor), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 30, Coordinates.Step * 0), ItemSlot.Armor));
            listUI.Add(new UI(GetSpriteByName(ItemSlot.Legs), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 31, Coordinates.Step * 0), ItemSlot.Legs));

            spellUI.Add(new UI(GetSpriteByName("UI_FistSpell"), spellUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 27, Coordinates.Step * 1),"Fist", 1));
            spellUI.Add(new UI(GetSpriteByName("UI_HealSpell"), spellUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 28, Coordinates.Step * 1), "Heal", 2));

            LoadWorld();

            for (int i = 0; i < 5; i++)
            {
                map.Creatures.Add(new Creature("Ghost", new Coordinates(Coordinates.Step * 2 + i * Coordinates.Step, 0), map.Players[0].ID, 25, 100, i + 1, 10, 150));
            }

            map.WorldItems.Add(CreateWorldItemFromListItem(GetItemFromListByName("Master Legs").ID, new Coordinates(Coordinates.Step * 5, Coordinates.Step * 5)));

            /*EquipItemFromAnywhere(map.Items[0], GetListUIByItemSlot(ItemSlot.LeftHand));
            EquipItemFromAnywhere(map.Items[1], GetListUIByItemSlot(ItemSlot.RightHand));
            EquipItemFromAnywhere(map.Items[2], GetListUIByItemSlot(ItemSlot.Armor));
            EquipItemFromAnywhere(map.Items[3], GetListUIByItemSlot(ItemSlot.Helmet));*/

            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseRegular"), 1, Entity.UnknownEntity, "MouseRegular"));
            MouseCursors.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\MouseDrag"), 2, Entity.UnknownEntity, "MouseDrag"));

            map.Players[0].SpriteID = 1;
            
            for(int i = 0; i < map.Creatures.Count; i++)
            {
                map.Creatures[i].SpriteID = GetSpriteIDByName("ghost");
            }

            /*map.Tiles.Add(new Tile("grass", 2, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 2), 1, true));
            map.Tiles.Add(new Tile("grass", 4, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 3), 2, true));
            map.Tiles.Add(new Tile("grass", 4, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 4), 3, true));
            map.Tiles.Add(new Tile("grass", 2, new Coordinates(Coordinates.Step * 4, Coordinates.Step * 5), 4, true));*/

            map.LoadMap("Content\\fields.map");

            font = Content.Load<SpriteFont>("EFont");

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
                    }

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
                if (!map.OutOfBoundaries(target))
                {
                    if (gameTime.TotalGameTime.TotalMilliseconds - leftClickTime < 200)
                    {
                        if (map.IsTileWalkable(target))
                        {
                            map.GeneratePathFromCreature(map.Players[0], target);
                        }
                    }
                    else
                    {
                        if (dragItem.ID != -1)
                        {
                            UI targetEquipment = GetListUIByMousePosition(x, y);
                            UI sourceEquipment = GetListUIByMousePosition(dragItem.Position.X, dragItem.Position.Y);

                            if (targetEquipment.ID == -1)
                            {
                                if (sourceEquipment.ID == -1)
                                {
                                    map.DragItem(dragItem, target);
                                }
                                else
                                {
                                    UnequipItem(sourceEquipment, target);
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
                MoveCreatures(gameTime);
            }

            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                map.GeneratePaths();// <- Uncomment this to make monsters move
            }

            if (gameTime.TotalGameTime.Seconds % 2 == 0 && !regenerated)
            {
                regenerated = true;
                BaseRegeneration();
            }
            else if(gameTime.TotalGameTime.Seconds % 2 == 1 && regenerated)
            {
                regenerated = false;
            }

            if(gameTime.TotalGameTime.Seconds % 10 == 0 && !respawned)
            {
                respawned = true;
                Respawn();
            }
            else if(gameTime.TotalGameTime.Seconds % 10 == 1 && respawned)
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

        public void Respawn()
        {
            for(int i = 0; i < map.Creatures.Count; i++)
            {
                if(map.Creatures[i].Health < 1)
                {
                    map.Creatures[i].Spawn();
                }
            }
        }

        private void EquipItem(UI targetEquipment, UI SourceEquipment = null)
        {
            if (SourceEquipment == null)
            {
                map.EquipItem(dragItem, targetEquipment);
            }
            else
            {
                map.EquipItem(dragItem, targetEquipment, SourceEquipment);
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
                if(map.Players[h].Mana < map.Players[h].MaxMana)
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
                else if (map.IsAdjacent(map.Creatures[i], map.Players[0]))
                {
                    if (gameTime.TotalGameTime.TotalMilliseconds - map.Creatures[i].TimeOfLastAttack > 1000)
                    {
                        int targetID = map.Creatures[i].TargetID;
                        int dmgDealt = map.CreatureAttack(map.Creatures[i], map.Players[0]);
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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            Vector2 cursorPos = Mouse.GetState().Position.ToVector2();
           
            GraphicsDevice.Clear(Color.ForestGreen);



            spriteBatch.Begin();

            DrawUI();

            DrawEquipment();
            

            for (int i = 0; i < map.Tiles.Count; i++)
            {
                spriteBatch.Draw(GetSpriteByID(map.Tiles[i].SpriteID), new Vector2((float)map.Tiles[i].Position.X, (float)map.Tiles[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            for(int i = 0; i < map.WorldItems.Count; i++)
            {
                //if(GetListUIByMousePosition(map.MapItems[i].Position.X, map.MapItems[i].Position.Y).ID == -1)
                if(map.WorldItems[i].Slot == null)
                spriteBatch.Draw(GetSpriteByID(map.WorldItems[i].SpriteID), new Vector2((float)map.WorldItems[i].Position.X, (float)map.WorldItems[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            // Draw creatures names:
            DrawCreatureNames();

            // Draw player name:
            DrawOutlinedString(font, map.Players[0].Name, new Vector2((float)map.Players[0].Position.X, (float)map.Players[0].Position.Y + Coordinates.Step), Color.White);

            // Draw Creatures:
            DrawCreatures();

            // Draw player sprite:
            spriteBatch.Draw(GetSpriteByID(map.Players[0].SpriteID), new Vector2((float)map.Players[0].Position.X, (float)map.Players[0].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            DrawSpells(gameTime);

            // Draw damage dealt
            DrawDamageDone(gameTime);

            int startStep = (spellUI[spellUI.Count - 1].Position.Y / 32) + 1;
            spriteBatch.DrawString(font, "Health: " + map.Players[0].Health + " / " + map.Players[0].MaxHealth, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Mana: " + map.Players[0].Mana + " / " + map.Players[0].MaxMana, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Level: " + map.Players[0].Level, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Experience: " + map.Players[0].Experience, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Magic Strength: " + map.Players[0].MagicStrength, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Strength: " + map.Players[0].TotalStrength(), new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Defense: " + map.Players[0].TotalDefense(), new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);

            spriteBatch.Draw(MouseCursors[currentMouse].Sprite, cursorPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
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
        }

        private void DrawCreatures()
        {
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                if (map.Creatures[i].Health > 0)
                {
                    spriteBatch.Draw(GetSpriteByID(map.Creatures[i].SpriteID), new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                    if (map.Creatures[i].ID == map.Players[0].TargetID)
                    {
                        // Draw attackbox
                        spriteBatch.Draw(GetSpriteByID(3), new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
        private void DrawCreatureNames()
        {
            for (int i = 0; i < map.Creatures.Count; i++)
            {
                if (map.Creatures[i].Health > 0)
                {
                    DrawOutlinedString(font, map.Creatures[i].Name, new Vector2((float)map.Creatures[i].Position.X, (float)map.Creatures[i].Position.Y + Coordinates.Step), Color.White);
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
                        DrawOutlinedString(font, dmgDone[f].damageDealt.ToString(), new Vector2((float)dmgDone[f].creature.Position.X + 2, (float)dmgDone[f].creature.Position.Y + (float)dmgDone[f].OffsetY(currentTime)), Color.LimeGreen);
                    }
                    else
                    {
                        DrawOutlinedString(font, dmgDone[f].damageDealt.ToString(), new Vector2((float)dmgDone[f].creature.Position.X + 2, (float)dmgDone[f].creature.Position.Y + (float)dmgDone[f].OffsetY(currentTime)), Color.Red);
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
            for(int i = 0; i < SpellDamage.Count; i++)
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
                                spriteBatch.Draw(spell.Sprite, new Vector2((float)SpellDamage[index].Position.X - Coordinates.Step + (i * Coordinates.Step), (float)SpellDamage[index].Position.Y - Coordinates.Step + (j * Coordinates.Step)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }
                else
                {
                    spriteBatch.Draw(spell.Sprite, new Vector2((float)SpellDamage[index].creature.Position.X, (float)SpellDamage[index].creature.Position.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
            for (int i = 0; i < map.WorldItems.Count; i++)
            {
                tmpRect.X = map.WorldItems[i].Position.X;
                tmpRect.Y = map.WorldItems[i].Position.Y;
                if (tmpRect.Contains(x, y))
                {
                    return map.WorldItems[i];
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
            for(int i = 0; i < Spells.Count; i++)
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

        private Item CreateWorldItemFromListItem(int ID, Coordinates pos = null)
        {
            Item newItem;
            for (int i = 0; i < map.ItemList.Count; i++)
            {
                if (map.ItemList[i].ID == ID)
                {
                    newItem = new Item(map.ItemList[i].Name, map.ItemList[i].WearSlot, pos, map.ItemList[i].SpriteID, map.ItemList[i].ID, map.ItemList[i].Strength, map.ItemList[i].Defense, map.ItemList[i].Visible);
                    return newItem;
                }
            }

            return new Item();
        }


        private UI GetListUIByItemSlot(string _ItemSlot)
        {
            for(int i = 0; i < listUI.Count;i++)
            {
                if(listUI[i].Name == _ItemSlot)
                {
                    return listUI[i];
                }

            }

            return new UI();
        }

        private void SaveWorld()
        {
            string saveString = "";
            StreamWriter writer = new StreamWriter("Content\\World.save");

            for (int i = 0; i < map.Players.Count; i++)
            {
                saveString = map.Players[i].ID + "|" + map.Players[i].Name + "|" + map.Players[i].EquippedItems.ToString() + "|" + map.Players[i].Level + "|" + map.Players[i].Experience + "|" + map.Players[i].MaxHealth + "|" + map.Players[i].MaxMana + "|" + map.Players[i].MagicStrength + "|" + map.Players[i].ManaSpent + "|" + map.Players[i].Position.X + "|" + map.Players[i].Position.Y + "|" + map.Players[i].SpriteID;
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

                string[] currentPlayer = read.ReadLine().Split("|".ToCharArray());
                string[] EquippedItems = currentPlayer[2].Split(",".ToCharArray());

                // TODO: Add the possibility of loading more than one player
                map.Players.Add(new Player(currentPlayer[1], new Coordinates(int.Parse(currentPlayer[9]), int.Parse(currentPlayer[10])), int.Parse(currentPlayer[5]), int.Parse(currentPlayer[6]), int.Parse(currentPlayer[3]), int.Parse(currentPlayer[0])));
                int currentID = map.Players.Count - 1;
                map.Players[currentID].Experience = int.Parse(currentPlayer[4]);

                if (int.Parse(EquippedItems[0]) > 0)
                {
                    map.WorldItems.Add(CreateWorldItemFromListItem(int.Parse(EquippedItems[0])));
                    map.WorldItems[map.WorldItems.Count - 1].Slot = ItemSlot.LeftHand;
                    map.WorldItems[map.WorldItems.Count - 1].Position = GetListUIByItemSlot(ItemSlot.LeftHand).Position;
                    map.Players[currentID].EquippedItems.LeftHand = map.WorldItems[map.WorldItems.Count - 1];
                    GetListUIByItemSlot(ItemSlot.LeftHand).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[1]) > 0)
                {
                    map.WorldItems.Add(CreateWorldItemFromListItem(int.Parse(EquippedItems[1])));
                    map.WorldItems[map.WorldItems.Count - 1].Slot = ItemSlot.RightHand;
                    map.WorldItems[map.WorldItems.Count - 1].Position = GetListUIByItemSlot(ItemSlot.RightHand).Position;
                    map.Players[currentID].EquippedItems.RightHand = map.WorldItems[map.WorldItems.Count - 1];
                    GetListUIByItemSlot(ItemSlot.RightHand).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[2]) > 0)
                {
                    map.WorldItems.Add(CreateWorldItemFromListItem(int.Parse(EquippedItems[2])));
                    map.WorldItems[map.WorldItems.Count - 1].Slot = ItemSlot.Helmet;
                    map.WorldItems[map.WorldItems.Count - 1].Position = GetListUIByItemSlot(ItemSlot.Helmet).Position;
                    map.Players[currentID].EquippedItems.Helmet = map.WorldItems[map.WorldItems.Count - 1];
                    GetListUIByItemSlot(ItemSlot.Helmet).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[3]) > 0)
                {
                    map.WorldItems.Add(CreateWorldItemFromListItem(int.Parse(EquippedItems[3])));
                    map.WorldItems[map.WorldItems.Count - 1].Slot = ItemSlot.Armor;
                    map.WorldItems[map.WorldItems.Count - 1].Position = GetListUIByItemSlot(ItemSlot.Armor).Position;
                    map.Players[currentID].EquippedItems.Armor = map.WorldItems[map.WorldItems.Count - 1];
                    GetListUIByItemSlot(ItemSlot.Armor).Sprite = GetSpriteByName("UI_background");
                }
                if (int.Parse(EquippedItems[4]) > 0)
                {
                    map.WorldItems.Add(CreateWorldItemFromListItem(int.Parse(EquippedItems[4])));
                    map.WorldItems[map.WorldItems.Count - 1].Slot = ItemSlot.Legs;
                    map.WorldItems[map.WorldItems.Count - 1].Position = GetListUIByItemSlot(ItemSlot.Legs).Position;
                    map.Players[currentID].EquippedItems.Legs = map.WorldItems[map.WorldItems.Count - 1];
                    GetListUIByItemSlot(ItemSlot.Legs).Sprite = GetSpriteByName("UI_background");
                }
                map.Players[currentID].ManaSpent = int.Parse(currentPlayer[8]);
                map.Players[currentID].MagicStrength = int.Parse(currentPlayer[7]);
                //saveString = Players[i].ID + "|" + Players[i].Name + "|" + Players[i].EquippedItems.ToString() + "|" + Players[i].Level + "|" + Players[i].Experience + "|" + Players[i].MaxHealth + "|" + Players[i].MaxMana + "|" + Players[i].MagicStrength + "|" + Players[i].ManaSpent + "|" + Players[i].Position.X + "|" + Players[i].Position.Y + "|" + Players[i].SpriteID;

                read.Close();
            }
        }
    }
}