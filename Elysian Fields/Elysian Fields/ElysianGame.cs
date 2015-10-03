using Microsoft.Xna.Framework;
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
            map.Players.Add(new Player("Aephirus", new Coordinates(0, 0), 150, 100, 1));
            //player1 = new Player("Aephirus", new Coordinates(0, 0));

            for(int i = 0; i < 5; i++)
            {
                map.Creatures.Add(new Creature("Ghost" + i.ToString(), new Coordinates(Coordinates.Step * 2 + i * Coordinates.Step, 0), map.Players[0].ID, 5, 100, i + 1));
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
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_ui"), spriteList.Count + 1, Entity.SpellEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\fistspell_animation"), spriteList.Count + 1, Entity.SpellEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_ui"), spriteList.Count + 1, Entity.SpellEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\healspell_animation"), spriteList.Count + 1, Entity.SpellEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\goldenarmor"), spriteList.Count + 1, Entity.ItemEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\torso"), spriteList.Count + 1, Entity.UnknownEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\helmet"), spriteList.Count + 1, Entity.UnknownEntity));
            spriteList.Add(new SpriteObject(Content.Load<Texture2D>("Graphics\\hornedhelmet"), spriteList.Count + 1, Entity.ItemEntity));

            map.Items.Add(new Item("Sword of magicnezz", new Coordinates(3 * Coordinates.Step, 3 * Coordinates.Step), 1, 0, 60, 1));
            map.Items.Add(new Item("Sword of magicnezz", new Coordinates(3 * Coordinates.Step, 5 * Coordinates.Step), 1, 0, 60, 1));
            map.Items.Add(new Item("Golden armor", new Coordinates(2 * Coordinates.Step, 2 * Coordinates.Step), 9, 0, 0, 25));
            map.Items.Add(new Item("Horned helmet", new Coordinates(3 * Coordinates.Step, 2 * Coordinates.Step), 12, 0, 0, 20));

            Spells.Add(new Spell(new bool[]
            {true, true, true,
            true, true, true,
            true, true, true}
            , 50, GetSpriteByID(6), 20, false, false, 1));

            Spells.Add(new Spell(new bool[] { true }, 50, GetSpriteByID(8), 5, true, true, 2));

            listUI.Add(new UI(Content.Load<Texture2D>("Graphics\\fist"), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 27, Coordinates.Step * 0), ItemSlot.LeftHand));
            listUI.Add(new UI(Content.Load<Texture2D>("Graphics\\fist"), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 28, Coordinates.Step * 0), ItemSlot.RightHand));
            listUI.Add(new UI(GetSpriteByID(11), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 29, Coordinates.Step * 0), ItemSlot.Helmet));
            listUI.Add(new UI(GetSpriteByID(10), listUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 30, Coordinates.Step * 0), ItemSlot.Armor));

            spellUI.Add(new UI(GetSpriteByID(5), spellUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 27, Coordinates.Step * 1),"Fist", 1));
            spellUI.Add(new UI(GetSpriteByID(7), spellUI.Count, Entity.UnknownEntity, new Coordinates(Coordinates.Step * 28, Coordinates.Step * 1), "Heal", 2));

            map.EquipItem(map.Items[0], GetListUIByItemSlot(ItemSlot.LeftHand), null, false);
            map.EquipItem(map.Items[1], GetListUIByItemSlot(ItemSlot.RightHand), null, false);
            map.EquipItem(map.Items[2], GetListUIByItemSlot(ItemSlot.Armor), null, false);
            map.EquipItem(map.Items[3], GetListUIByItemSlot(ItemSlot.Helmet), null, false);

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
                                    map.UnequipItem(dragItem, sourceEquipment, target);
                                }
                            }
                            else
                            {
                                if (sourceEquipment.ID == -1)
                                {
                                    map.EquipItem(dragItem, targetEquipment);
                                }
                                else
                                {
                                    map.EquipItem(dragItem, targetEquipment, sourceEquipment);
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
                map.GeneratePaths();// <- Remove this to make monsters move
            }

            if (gameTime.TotalGameTime.Seconds % 2 == 0 && !regenerated)
            {
                //Window.Title = "Hej" + gameTime.TotalGameTime.Seconds.ToString();
                regenerated = true;
                BaseRegeneration();
            }
            else if(gameTime.TotalGameTime.Seconds % 2 == 1 && regenerated)
            {
                regenerated = false;
            }

            //Window.Title = (gameTime.TotalGameTime.Seconds % 2).ToString();

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

        public void BaseRegeneration()
        {
            for (int h = 0; h < map.Players.Count; h++)
            {
                if (map.Players[h].Health < map.Players[h].MaxHealth)
                {
                    map.Players[h].Health += 1;
                }
                if(map.Players[h].Mana < map.Players[h].MaxMana)
                {
                    map.Players[h].Mana += 2;
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
            spriteBatch.DrawString(font, "Experience: " + map.Players[0].Experience, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Health: " + map.Players[0].Health + " / " + map.Players[0].MaxHealth, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Mana: " + map.Players[0].Mana + " / " + map.Players[0].MaxMana, new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Strength: " + map.Players[0].TotalStrength(), new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);
            spriteBatch.DrawString(font, "Defense: " + map.Players[0].TotalDefense(), new Vector2((float)Coordinates.Step * 27, (float)Coordinates.Step * startStep++), Color.Black);

            spriteBatch.Draw(MouseCursors[currentMouse].Sprite, cursorPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
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
    }
}