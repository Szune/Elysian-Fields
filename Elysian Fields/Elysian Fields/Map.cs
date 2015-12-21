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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.Xna;
using System.IO;
using Microsoft.Xna.Framework;
using Elysian_Fields.Modules.AI;
using System.Threading.Tasks;

namespace Elysian_Fields
{
    public sealed class Map
    {
        private static readonly Map instance = new Map();
        internal List<Tile> Tiles = new List<Tile>();
        internal List<Creature> Creatures = new List<Creature>();
        internal List<Player> Players = new List<Player>();
        internal List<Entity> Food = new List<Entity>();
        internal List<Item> WorldItems = new List<Item>();
        internal List<Item> ItemList = new List<Item>();
        internal List<Creature> CreatureList = new List<Creature>();
        internal List<NPC> NPCs = new List<NPC>();
        internal List<Tile> DebugTiles_Pathfinding = new List<Tile>();
        internal List<Spell> Spells = new List<Spell>();
        private Random Generator = new Random();
        internal Task PathfindingTask;
        //private  PathfindingTask;

        internal Coordinates windowSize;

        private Map() { }

        public static Map Instance
        {
            get
            {
                return instance;
            }
        }

        // Map(Coordinates WindowSize) { windowSize = WindowSize; }

        internal void GeneratePathFromCreature(Creature FromCreature, Coordinates Target, int DistanceTo = 20)
        {
            FromCreature.ResetPath();
            PathfindingTask = Task.Factory.StartNew(() =>
            {
                AI ai = new AI();
                FromCreature.Path = ai.Monster_PathTo(Target, FromCreature.Position, DistanceTo);
                if (FromCreature.Path.Count > 0)
                {
                    FromCreature.Destination = Coordinates.Parse(FromCreature.Path[0]);
                }
            }
            );

            //FromCreature.Path = ai.PathTo(Target, FromCreature.Position, DistanceTo);
        }

        internal void GeneratePathFromPlayer(Creature FromCreature, Coordinates Target, int DistanceTo = 20)
        {
            FromCreature.Destination = Target;
            FromCreature.ResetPath();
            AI ai = new AI();
            PathfindingTask = Task.Factory.StartNew(() =>
            {
                FromCreature.Path = ai.PathTo(Target, FromCreature.Position, DistanceTo);
            }
            );

            //FromCreature.Path = ai.PathTo(Target, FromCreature.Position, DistanceTo);
        }

        internal void LoadNPCs()
        {
            DirectoryInfo GetDir = new DirectoryInfo("Content\\Scripts\\NPCs");
            FileInfo[] FilesInDir = GetDir.GetFiles();
            for (int i = 0; i < FilesInDir.Length; i++)
            {
                if (FilesInDir[i].Name != "funcs.lua")
                {
                    NPCs.Add(new NPC(FilesInDir[i].FullName));
                }
            }
        }

        internal void LoadSpells()
        {
            DirectoryInfo GetDir = new DirectoryInfo("Content\\Scripts\\Spells");
            FileInfo[] FilesInDir = GetDir.GetFiles();
            for (int i = 0; i < FilesInDir.Length; i++)
            {
                if (FilesInDir[i].Name != "funcs.lua")
                {
                    Spells.Add(new Spell(FilesInDir[i].FullName));
                }
            }
        }

        internal void GeneratePaths()
        {
            for (int i = 0; i < Creatures.Count; i++)
            {
                Creatures[i].TargetID = Monster_FindTarget(Creatures[i]);
                if (Creatures[i].TargetID != -1)
                {
                    Player player = GetPlayerByID(Creatures[i].TargetID);
                    if (player.Health > 0 && DistanceToDiagonal(player.Position, Creatures[i].Position) > 1 && DistanceToDiagonal(player.Position, Creatures[i].Position) <= 8)
                    {
                        if (Creatures[i].Health > 0) // && !IsAdjacent(Creatures[i], GetCreatureByID(Creatures[i].TargetID)))
                        {
                            //if (!Creatures[i].hasPath() || (Creatures[i].hasPath() && !SamePosition(player.Position, Creatures[i].Destination)))
                            //{
                            GeneratePathFromCreature(Creatures[i], player.Position, 8);
                            /*Coordinates next = Creatures[i].NextStep();
                            if (next != null)
                            {
                                Creatures[i].Position = new Coordinates(next.X, next.Y, Creatures[i].Position.Z);
                            }*/
                            //}
                        }
                    }
                }
            }
        }

        internal void MovePlayer()
        {
            /* TODO: Add multiplayer functionality and change Players[0] to a variable */

            if (Players[0].Health > 0 && Players[0].hasPath())
            {
                MoveCreature(Players[0], Players[0].NextStep());
            }
        }

        internal bool Roll(int chancePercent)
        {
            if (chancePercent >= Generator.Next(1, 101))
            {
                return true;
            }
            return false;
        }

        internal int Monster_FindTarget(Creature monster)
        {
            int targetID = -1;
            //StreamWriter debug = new StreamWriter("debug.txt", true);
            //rectangle.x = monsterx-13
            //rectangle.width = rectangle.x + 25
            //rectangle.y = monstery - 9
            //rectangle.height = rectangle.y + 17
            Rectangle TargetArea;
            TargetArea.X = monster.Position.X - 7;
            TargetArea.Y = monster.Position.Y - 5;
            TargetArea.Width = TargetArea.X + 15;
            TargetArea.Height = TargetArea.Y + 11;
            if (TargetArea.X < 0) { TargetArea.X = 0; TargetArea.Width += 1; }
            if (TargetArea.Y < 0) { TargetArea.Y = 0; TargetArea.Height += 1; }

            for (int i = 0; i < Players.Count; i++)
            {
                //debug.WriteLine("TargetAreaX = " + TargetArea.X + " TargetAreaY = " + TargetArea.Y + "TargetAreaWidth = " + TargetArea.Width + " TargetAreaHeight = " + TargetArea.Height + " | PlayerX = " + Players[i].Position.X + " PlayerY = " + Players[i].Position.Y + " PlayerZ = " + Players[i].Position.Z + " | MonsterZ = " + monster.Position.Z);
                if (DistanceToDiagonal(monster.Position, Players[i].Position) <= 8)
                {
                    if (TargetArea.Contains(Players[i].Position.X, Players[i].Position.Y) && monster.Position.Z == Players[i].Position.Z)
                    {
                        targetID = Players[i].ID;
                    }
                }
            }

            //debug.Close();

            return targetID;
        }

        internal void MoveCreature(Entity creature, Coordinates step)
        {
            if (IsTileWalkable(step) && DistanceTo(creature.Position, step) == 1)
            {
                Tile walkTile = GetTopTileFromTile(step);
                if (!walkTile.MovePlayer)
                {
                    creature.Position = new Coordinates(step.X, step.Y, step.Z);
                }
                else
                {
                    creature.Position = new Coordinates(step.X + walkTile.RelativeMovePosition.X, step.Y + walkTile.RelativeMovePosition.Y, step.Z + walkTile.RelativeMovePosition.Z);
                }
            }
            //else
            //{
            //    int EntityType = GetEntityTypeFromTile(step);
            //    if (EntityType == Entity.CreatureEntity)
            //    {
            //        /* TODO: Change Players[0] to a variable ID and add method to attack other players to allow for multiplayer */
            //        if (creature.EntityType == Entity.PlayerEntity)
            //        {
            //            Creature monster = GetCreatureByID(GetCreatureIDFromTile(step));
            //            PlayerAttack(Players[0], monster);
            //        }
            //    }
            //    else if (EntityType == Entity.PlayerEntity)
            //    {
            //        if (creature.EntityType == Entity.CreatureEntity)
            //        {
            //            CreatureAttack(GetCreatureByID(creature.ID), Players[0]);
            //        }
            //    }
            //    else if (EntityType == Entity.ItemEntity)
            //    {
            //        /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
            //        Eat(Players[0], step);
            //    }
            //}
        }

        internal bool AllDead()
        {
            bool dead = true;
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (Creatures[i].Health > 0)
                {
                    dead = false;
                    break;
                }
            }
            return dead;
        }

        internal int CreatureAttack(Creature creature, Player target)
        {
            if (CanAttack(creature, target))
            {
                int dmgDealt = target.ReceiveDamage(creature.Strength, target.TotalDefense());
                if (target.Health < 1)
                {
                    //creature.Experience += 1 + target.Experience;
                    target.Die();
                }
                return dmgDealt;
            }
            return -1;
        }

        internal List<DamageObject> PlayerAttack(Player player, GameTime gameTime)
        {
            List<DamageObject> DamagedMonsters = new List<DamageObject>();
            int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            if (player.TargetID != -1)
            {
                Creature target = GetCreatureByID(player.TargetID);
                if (target.Name != "null")
                {
                    // TODO: Fix below so it works with the new coordinate system (which is screen coordinates / 32)
                    if (CanAttack(player, target) && DistanceToDiagonal(player.Position, target.Position) < 2) // < 46 because when standing diagonal, the distance is 45, when standing directly in front, it is 32
                    {
                        int dmgDealt = target.ReceiveDamage(player.TotalStrength(), target.Defense);
                        DamagedMonsters.Add(new DamageObject(target, dmgDealt, DamageObject.Text_Damage, currentTime, currentTime + DamageObject.DamageDuration));
                        if (target.Health < 1)
                        {
                            int experience = CreatureDie(target, player);
                            DamagedMonsters.Add(new DamageObject(player, experience, DamageObject.Text_Experience, currentTime, currentTime + DamageObject.DamageDuration));
                        }
                    }
                }
            }
            return DamagedMonsters;
        }

        internal int CreatureDie(Creature creature, Player killer)
        {
            killer.ReceiveExperience(creature.Experience);
            if (creature.LootList.Count > 0)
            {
                for (int i = 0; i < creature.LootList.Count; i++)
                {
                    if (Roll(creature.LootList[i].LootChance))
                    {
                        CreateWorldItemFromListItem(creature.LootList[i].RealID, creature.Position);
                    }
                }
            }
            if (creature.ID == killer.TargetID)
            {
                killer.TargetID = -1;
            }
            return creature.Experience;
        }

        internal void PlayerDie(Creature killer, Player creature)
        {
            //killer.ReceiveExperience(creature.Experience);
            //if (creature.LootList.Count > 0)
            //{
            //Random generator = new Random();
            //generator.
            /*for (int i = 0; i < 1; i++)
            {
                //Decimal chance = (generator.Next(creature.LootList[i].LootChance * 100, 1 * 100));
                //chance = (Math.Round(chance / 10.0)) * 10;
                //if (chance >= 90)
                CreateWorldItemFromListItem(creature.LootList[i].SpriteID, creature.Position);
                //else
                //killer.Name = chance.ToString();
            }
        }*/
            if (creature.ID == killer.TargetID)
            {
                killer.TargetID = -1;
            }
        }

        internal List<DamageObject> PlayerCastSpell(Player player, Spell spell, Creature target, GameTime gameTime)
        {
            List<DamageObject> DamagedMonsters = new List<DamageObject>();
            int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            if (spell.AreaSpell)
            {
                Coordinates TopLeftOfScreen = new Coordinates(player.Position.X - 7, player.Position.Y - 5);
                /* TODO: Make area spell healing possible! */
                int n = 1;
                int y = 0;
                int x = 0;
                int experience = 0;
                for (int i = 0; i < spell.Area.Length; i++)
                {
                    if (n % 15 == 0)
                    {
                        y++;
                        x = -1;
                    }
                    if (spell.Area[i] == 1)
                    {
                        //spriteBatch.Draw(spell.Sprite, new Vector2((float)(SpellDamage[index].Position.X - map.Players[0].Position.X + (Utility.ScreenX) - 7 + x) * Coordinates.Step, (float)(SpellDamage[index].Position.Y - map.Players[0].Position.Y + (Utility.ScreenY) - 5 + y) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        List<Creature> CastOnCreatures = GetCreaturesFromTile(new Coordinates(TopLeftOfScreen.X + x, TopLeftOfScreen.Y + y, player.Position.Z));
                        for (int c = 0; c < CastOnCreatures.Count; c++)
                        {

                            Creature creature = CastOnCreatures[c];
                            if (creature.Health > 0)
                            {
                                int DamageDealt = creature.ReceiveDamage(spell.Damage + (player.MagicStrength * 2), 0);
                                int text_type = 0;
                                if (spell.HealSpell)
                                {
                                    text_type = DamageObject.Text_Healing;
                                }
                                else
                                {
                                    text_type = DamageObject.Text_Damage;
                                }
                                DamagedMonsters.Add(new DamageObject(creature, DamageDealt, text_type, currentTime, currentTime + DamageObject.DamageDuration));
                                if (creature.Health < 1)
                                {
                                    experience += CreatureDie(creature, player);
                                }
                            }
                        }
                    }
                    n++;
                    x++;
                }
                if (experience > 0)
                {
                    DamagedMonsters.Add(new DamageObject(player, experience, DamageObject.Text_Experience, currentTime, currentTime + DamageObject.DamageDuration));
                }
            }
            else
            {
                if (spell.HealSpell)
                {
                    int damage = spell.Damage + (player.MagicStrength * 2);
                    if (player.Health + damage > player.MaxHealth)
                    {
                        damage = player.MaxHealth - player.Health;
                        player.Health = player.MaxHealth;
                    }
                    else
                    {
                        player.Health += damage;
                    }
                    int text_type = 0;
                    if (spell.HealSpell)
                    {
                        text_type = DamageObject.Text_Healing;
                    }
                    else
                    {
                        text_type = DamageObject.Text_Damage;
                    }
                    DamagedMonsters.Add(new DamageObject(player, damage, text_type, currentTime, currentTime + DamageObject.DamageDuration));
                }
                else
                {
                    int DamageDealt = target.ReceiveDamage(spell.Damage + (player.MagicStrength * 2), 0);
                    int text_type = 0;
                    if (spell.HealSpell)
                    {
                        text_type = DamageObject.Text_Healing;
                    }
                    else
                    {
                        text_type = DamageObject.Text_Damage;
                    }
                    DamagedMonsters.Add(new DamageObject(target, DamageDealt, text_type, currentTime, currentTime + DamageObject.DamageDuration));
                    if (target.Health < 1)
                    {
                        int experience = CreatureDie(target, player);
                        DamagedMonsters.Add(new DamageObject(player, experience, DamageObject.Text_Experience, currentTime, currentTime + DamageObject.DamageDuration));
                    }
                }
            }

            return DamagedMonsters;
        }

        internal List<DamageObject> CreatureCastSpell(Player target, Spell spell, Creature creature, GameTime gameTime)
        {
            List<DamageObject> DamagedPlayers = new List<DamageObject>();
            int currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            if (spell.AreaSpell)
            {
                Coordinates TopLeftOfScreen = new Coordinates(creature.Position.X - 7, creature.Position.Y - 5);
                /* TODO: Make area spell healing possible! */
                int n = 1;
                int y = 0;
                int x = 0;
                for (int i = 0; i < spell.Area.Length; i++)
                {
                    if (n % 15 == 0)
                    {
                        y++;
                        x = -1;
                    }
                    if (spell.Area[i] == 1)
                    {
                        //spriteBatch.Draw(spell.Sprite, new Vector2((float)(SpellDamage[index].Position.X - map.Players[0].Position.X + (Utility.ScreenX) - 7 + x) * Coordinates.Step, (float)(SpellDamage[index].Position.Y - map.Players[0].Position.Y + (Utility.ScreenY) - 5 + y) * Coordinates.Step), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        List<Player> CastOnPlayers = GetPlayersFromTile(new Coordinates(TopLeftOfScreen.X + x, TopLeftOfScreen.Y + y, creature.Position.Z));
                        for (int c = 0; c < CastOnPlayers.Count; c++)
                        {

                            Player player = CastOnPlayers[c];
                            if (player.Health > 0)
                            {
                                int DamageDealt = player.ReceiveDamage(spell.Damage + (creature.MagicStrength * 2), 0);
                                int text_type = 0;
                                if (spell.HealSpell)
                                {
                                    text_type = DamageObject.Text_Healing;
                                }
                                else
                                {
                                    text_type = DamageObject.Text_Damage;
                                }
                                DamagedPlayers.Add(new DamageObject(player, DamageDealt, text_type, currentTime, currentTime + DamageObject.DamageDuration));
                                if (player.Health < 1)
                                {
                                    PlayerDie(creature, player);
                                }
                            }
                        }
                    }
                    n++;
                    x++;
                }
            }
            else
            {
                if (spell.HealSpell)
                {
                    int damage = spell.Damage + (creature.MagicStrength * 2);
                    if (creature.Health + damage > creature.MaxHealth)
                    {
                        damage = creature.MaxHealth - creature.Health;
                        creature.Health = creature.MaxHealth;
                    }
                    else
                    {
                        creature.Health += damage;
                    }
                    int text_type = 0;
                    if (spell.HealSpell)
                    {
                        text_type = DamageObject.Text_Healing;
                    }
                    else
                    {
                        text_type = DamageObject.Text_Damage;
                    }
                    DamagedPlayers.Add(new DamageObject(creature, damage, text_type, currentTime, currentTime + DamageObject.DamageDuration));
                }
                else
                {
                    int DamageDealt = target.ReceiveDamage(spell.Damage + (creature.MagicStrength * 2), 0);
                    int text_type = 0;
                    if (spell.HealSpell)
                    {
                        text_type = DamageObject.Text_Healing;
                    }
                    else
                    {
                        text_type = DamageObject.Text_Damage;
                    }
                    DamagedPlayers.Add(new DamageObject(target, DamageDealt, text_type, currentTime, currentTime + DamageObject.DamageDuration));
                    if (target.Health < 1)
                    {
                        PlayerDie(creature, target);
                    }
                }
            }

            return DamagedPlayers;
        }


        internal bool CanAttack(Creature creature, Creature target)
        {
            if (target.Health > 0 && creature.Health > 0)
            {
                return true;
            }
            return false;
        }

        internal int GetEntityTypeFromTile(Coordinates Tile)
        {
            if (IsTileCreature(Tile)) return Entity.CreatureEntity;
            if (IsTilePlayer(Tile)) return Entity.PlayerEntity;
            if (IsTileFood(Tile)) return Entity.ItemEntity;
            return Entity.UnknownEntity;
        }

        internal bool IsAdjacent(Entity Object1, Entity Object2)
        {
            return (DistanceToDiagonal(Object2.Position, Object1.Position) < 2);
        }

        internal int DistanceTo(Coordinates Source, Coordinates Destination)
        {
            return (Math.Abs((Source.X - Destination.X) + Math.Abs(Source.Y - Destination.Y)));
        }

        internal int DistanceToDiagonal(Coordinates Source, Coordinates Destination)
        {
            return (int)Math.Sqrt(Math.Pow((Source.X - Destination.X), 2) + Math.Pow(Source.Y - Destination.Y, 2));
        }

        internal int GetTileIDFromTile(Coordinates Tile)
        {
            int TileID = -1;
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (SamePosition(Tile, Tiles[i].Position))
                {
                    TileID = Tiles[i].ID;
                    break;
                }
            }
            return TileID;
        }

        internal bool GetTileWalkable(Coordinates Tile)
        {
            bool walk = true;
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (SamePosition(Tile, Tiles[i].Position) && !Tiles[i].Walkable)
                {
                    walk = false;
                    break;
                }
            }
            return walk;
        }

        internal Tile GetTileByID(int ID)
        {
            int TileID = -1;
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i].ID == ID)
                {
                    TileID = i;
                    break;
                }
            }

            if (TileID == -1)
            {
                return new Tile("null");
            }

            return Tiles[TileID];
        }

        internal void DragItem(Item item, Coordinates Target)
        {
            if (IsTileThrowable(Target) && AdjacentToItem(Players[0], item))
            {
                item.Z_order = GetTopItemFromTile(Target).Z_order + 1;
                item.Position = new Coordinates(Target.X, Target.Y, Target.Z);
            }
        }

        internal bool IsTileThrowable(Coordinates Target)
        {
            bool tileThrowable = true;

            int TileID = GetTileIDFromTile(Target);
            if (TileID != -1) /* TODO: When map is finished, IsTileWalkable() should return false; if TileID == -1 (it should not be possible to walk where there is no sprite)*/
            {
                tileThrowable = GetTileWalkable(Target);
            }
            else
            {
                tileThrowable = false;
            }
            return tileThrowable;
        }

        internal bool AdjacentToItem(Player player, Item item)
        {
            if (DistanceToDiagonal(Players[0].Position, item.Position) < 2)
            {
                if (item.Position.Z == Players[0].Position.Z)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        internal Item GetTopItemFromTile(Coordinates Tile)
        {
            List<Item> TileItems = new List<Item>();
            for (int i = 0; i < WorldItems.Count; i++)
            {
                if (SamePosition(WorldItems[i].Position, Tile))
                {
                    TileItems.Add(WorldItems[i]);
                }
            }
            if (TileItems.Count > 0)
            {
                TileItems.Sort((a, b) => a.Z_order.CompareTo(b.Z_order));
                return TileItems[TileItems.Count - 1];
            }
            return new Item();
        }

        internal Tile GetTopTileFromTile(Coordinates Tile)
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
            return new Tile("");
        }

        internal void EquipItem(Item item, UI equipment, UI sourceEquipment = null, bool haveToBeAdjacent = true)
        {
            // TODO: Change when adding multiplayer
            if (AdjacentToItem(Players[0], item) || haveToBeAdjacent == false)
            {
                if (Players[0].EquippedItems.IsItemSlotEmpty(equipment.Name))
                {
                    if (item.WearSlot == equipment.Name || (item.WearSlot == ItemSlot.LeftHand && equipment.Name == ItemSlot.RightHand))
                    {
                        item.Position = new Coordinates(equipment.Position.X, equipment.Position.Y);
                        item.Slot = equipment.Name;
                        item.WearingPlayerID = Players[0].ID;
                        Players[0].EquipItem(item, equipment.Name);

                        if (sourceEquipment != null)
                        {
                            Players[0].UnequipItem(sourceEquipment.Name);
                        }
                    }
                }
            }
        }

        internal Spell GetSpellByName(string name)
        {
            for (int i = 0; i < Spells.Count; i++)
            {
                if (Spells[i].Name == name)
                {
                    return Spells[i];
                }
            }
            return new Spell();
        }

        internal Spell GetSpellByID(int id)
        {
            for (int i = 0; i < Spells.Count; i++)
            {
                if (Spells[i].ID == id)
                {
                    return Spells[i];
                }
            }
            return new Spell();
        }

        internal void UnequipItem(Item item, UI equipment, Coordinates target)
        {
            if (IsTileThrowable(target))
            {
                item.Slot = null;
                item.WearingPlayerID = -1;
                item.Z_order = GetTopItemFromTile(target).Z_order + 1;
                item.Position = new Coordinates(target.X, target.Y, target.Z);
                Players[0].UnequipItem(equipment.Name);
            }
        }

        internal void ThrowItemFromBag(Item item, Coordinates target)
        {
            if (IsTileThrowable(target))
            {
                item.Slot = null;
                item.Z_order = GetTopItemFromTile(target).Z_order + 1;
                item.Position = new Coordinates(target.X, target.Y, target.Z);
                item.Parent.RemoveItem(item);
            }
        }

        internal void ThrowItemToBag(Item item, Backpack Bag, Coordinates target, UI SourceEquipment = null, Backpack Parent = null)
        {
            if (Parent != null)
            {
                Parent.RemoveItem(item);
                item.Slot = null;
                item.WearingPlayerID = Players[0].ID;
                item.Position = new Coordinates(target.X, target.Y);
                Bag.AddItem(item);
                if (SourceEquipment != null)
                {
                    Players[0].UnequipItem(SourceEquipment.Name);
                }
            }
            else
            {
                if (SourceEquipment != null)
                {
                    item.Slot = null;
                    item.WearingPlayerID = Players[0].ID;
                    item.Position = new Coordinates(target.X, target.Y);
                    Bag.AddItem(item);
                    Players[0].UnequipItem(SourceEquipment.Name);
                }
                else
                {
                    if (AdjacentToItem(Players[0], item))
                    {
                        item.Slot = null;
                        item.WearingPlayerID = Players[0].ID;
                        item.Position = new Coordinates(target.X, target.Y);
                        Bag.AddItem(item);
                    }
                }
            }
        }

        internal void EquipItemFromBag(Item item, UI equipment)
        {
            if (Players[0].EquippedItems.IsItemSlotEmpty(equipment.Name))
            {
                if (item.WearSlot == equipment.Name || (item.WearSlot == ItemSlot.LeftHand && equipment.Name == ItemSlot.RightHand))
                {
                    item.Position = new Coordinates(equipment.Position.X, equipment.Position.Y);
                    item.Slot = equipment.Name;
                    item.WearingPlayerID = Players[0].ID;
                    item.Parent.RemoveItem(item);
                    Players[0].EquipItem(item, equipment.Name);
                }
            }
        }

        internal Item CreateWorldItemFromListItem(int ID, Coordinates pos = null, int zorder = 0)
        {
            Item newItem;
            if (pos == null) { pos = new Coordinates(0, 0); }
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].ID == ID)
                {
                    newItem = new Item(ItemList[i].Name, ItemList[i].RealID, ItemList[i].WearSlot, pos, ItemList[i].SpriteID, WorldItems.Count + 1, ItemList[i].Strength, ItemList[i].Defense, ItemList[i].Visible, 0, zorder);
                    WorldItems.Add(newItem);
                    return WorldItems[WorldItems.Count - 1];
                }
            }

            return new Item();
        }

        internal void CreateCreatureFromCreatureList(int ID, Coordinates pos = null)
        {
            Creature newCreature;
            if (pos == null) { pos = new Coordinates(0, 0, 0); }
            for (int i = 0; i < CreatureList.Count; i++)
            {
                if (CreatureList[i].ID == ID)
                {
                    newCreature = new Creature(CreatureList[i].Name, pos, Players[0].ID, CreatureList[i].MagicStrength, CreatureList[i].Strength, CreatureList[i].Health, Creatures.Count, CreatureList[i].Defense, CreatureList[i].Experience, CreatureList[i].SpriteID, CreatureList[i].LootList, CreatureList[i].Spells);
                    Creatures.Add(newCreature);
                }
            }
        }

        internal void NPC_CreateCreature(int ID, int x, int y, int z)
        {
            CreateCreatureFromCreatureList(ID, new Coordinates(x, y, z));
        }

        internal void NPC_CreateItem(int ID, int x, int y, int z)
        {
            CreateWorldItemFromListItem(ID, new Coordinates(x, y, z));
        }

        internal bool IsTileWalkable(Coordinates Tile)
        {
            bool tileWalkable = true;

            int TileID = GetTileIDFromTile(Tile);
            if (TileID != -1)
            {
                tileWalkable = GetTileWalkable(Tile);
            }
            else
            {
                return false;
            }

            if (tileWalkable) { tileWalkable = !IsTilePlayer(Tile); }
            if (tileWalkable) { tileWalkable = !IsTileCreature(Tile); }
            if (tileWalkable) { tileWalkable = !IsTileNPC(Tile); }
            if (tileWalkable) { tileWalkable = !OutOfBoundaries(Tile); }

            return tileWalkable;
        }

        internal HashSet<Node> GetBlockedTiles(Node TargetPosition, int MaxDistanceFromPosition = 7)
        {
            HashSet<Node> nodes = new HashSet<Node>();
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Position.Z == TargetPosition.Z && Players[i].Health > 0)
                {
                    if (DistanceToDiagonal(Coordinates.Parse(TargetPosition), Players[i].Position) <= MaxDistanceFromPosition)
                    {
                        nodes.Add(new Node(Players[i].Position.X, Players[i].Position.Y));
                    }
                }
            }
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (Creatures[i].Position.Z == TargetPosition.Z && Creatures[i].Health > 0)
                {
                    if (DistanceToDiagonal(Coordinates.Parse(TargetPosition), Creatures[i].Position) <= MaxDistanceFromPosition)
                    {
                        nodes.Add(new Node(Creatures[i].Position.X, Creatures[i].Position.Y));
                    }
                }
            }
            for (int i = 0; i < NPCs.Count; i++)
            {
                if (NPCs[i].Position.Z == TargetPosition.Z)
                {
                    if (DistanceToDiagonal(Coordinates.Parse(TargetPosition), NPCs[i].Position) <= MaxDistanceFromPosition)
                    {
                        nodes.Add(new Node(NPCs[i].Position.X, NPCs[i].Position.Y));
                    }
                }
            }
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i].Position.Z == TargetPosition.Z)
                {
                    if (DistanceToDiagonal(Coordinates.Parse(TargetPosition), Tiles[i].Position) <= MaxDistanceFromPosition && !Tiles[i].Walkable)
                    {
                        nodes.Add(new Node(Tiles[i].Position.X, Tiles[i].Position.Y));
                    }
                }
            }
            return nodes;
        }

        internal bool AI_IsTileWalkable(Coordinates Tile)
        {
            bool tileWalkable = true;

            int TileID = GetTileIDFromTile(Tile);
            if (TileID != -1)
            {
                tileWalkable = GetTileWalkable(Tile);
            }
            else
            {
                return false;
            }

            if (tileWalkable) { tileWalkable = !OutOfBoundaries(Tile); }

            return tileWalkable;
        }

        internal bool IsTilePlayer(Coordinates Tile)
        {
            /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
            if (SamePosition(Tile, Players[0].Position) && Players[0].Health > 0)
            {
                return true;
            }
            return false;
        }

        internal bool IsTileCreature(Coordinates Tile)
        {
            bool tileCreature = false;
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (SamePosition(Tile, Creatures[i].Position) && Creatures[i].Health > 0)
                {
                    tileCreature = true;
                    break;
                }
            }
            return tileCreature;
        }

        internal bool IsTileNPC(Coordinates Tile)
        {
            bool tileNPC = false;
            for (int i = 0; i < NPCs.Count; i++)
            {
                if (SamePosition(Tile, NPCs[i].Position))
                {
                    tileNPC = true;
                    break;
                }
            }
            return tileNPC;
        }

        internal bool IsTileAnimate(Coordinates Tile)
        {
            return IsTileCreature(Tile) || IsTilePlayer(Tile);
        }

        internal bool IsTileFood(Coordinates Tile)
        {
            bool tileFood = false;
            for (int i = 0; i < Food.Count; i++)
            {
                if (SamePosition(Tile, Food[i].Position) && Food[i].Visible)
                {
                    tileFood = true;
                    break;
                }
            }
            return tileFood;
        }

        internal int GetCreatureIDFromTile(Coordinates Tile)
        {
            int CreatureID = -1;
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (SamePosition(Tile, Creatures[i].Position) && Creatures[i].Health > 0)
                {
                    CreatureID = Creatures[i].ID;
                    break;
                }
            }
            return CreatureID;
        }

        internal List<Creature> GetCreaturesFromTile(Coordinates Tile)
        {
            List<Creature> tile_creatures = new List<Creature>();
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (SamePosition(Tile, Creatures[i].Position) && Creatures[i].Health > 0)
                {
                    tile_creatures.Add(Creatures[i]);
                }
            }
            return tile_creatures;
        }

        internal List<Player> GetPlayersFromTile(Coordinates Tile)
        {
            List<Player> tile_players = new List<Player>();
            for (int i = 0; i < Players.Count; i++)
            {
                if (SamePosition(Tile, Players[i].Position) && Players[i].Health > 0)
                {
                    tile_players.Add(Players[i]);
                }
            }
            return tile_players;
        }


        internal Creature GetCreatureByID(int ID)
        {
            int CreatureID = -1;
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (Creatures[i].ID == ID)
                {
                    CreatureID = i;
                    break;
                }
            }

            if (CreatureID == -1)
            {
                return new Creature("null");
            }
            return Creatures[CreatureID];
        }

        internal Player GetPlayerByID(int ID)
        {
            /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].ID == ID)
                {
                    return Players[i];
                }
            }
            return new Player();

            //return Players[0];
        }

        internal Entity GetCreatureByName(string Name)
        {
            int CreatureID = -1;
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (Creatures[i].Name == Name)
                {
                    CreatureID = i;
                    break;
                }
            }
            if (CreatureID == -1)
            {
                return new Entity("null");
            }
            return Creatures[CreatureID];
        }

        internal bool SamePosition(Coordinates Source, Coordinates Destination, bool CheckZ = true)
        {
            if (CheckZ)
                return (Source.X == Destination.X && Source.Y == Destination.Y && Source.Z == Destination.Z);
            else
                return (Source.X == Destination.X && Source.Y == Destination.Y);
        }

        internal bool TileAbove(Coordinates Position)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (IsAbove(Tiles[i].Position, Position))
                {
                    return true;
                }
            }
            return false;
        }

        internal int TopTileZ(Coordinates Tile)
        {
            int currentTopZ = Utility.MinZ;
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (SamePosition(Tiles[i].Position, Tile, false) && Tiles[i].Position.Z > currentTopZ)
                {
                    currentTopZ = Tiles[i].Position.Z;
                }
            }
            return currentTopZ;
        }

        internal bool IsAbove(Coordinates Destination, Coordinates Source)
        {
            return (Source.X == Destination.X && Source.Y == Destination.Y && Source.Z < Destination.Z);
        }

        internal bool OutOfBoundaries(Coordinates Coordinates)
        {
            return !(Coordinates.X >= 0 && Coordinates.Y >= 0 && Coordinates.X < Utility.MaxX && Coordinates.Y < Utility.MaxY);
        }
    }
}
