using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.Xna;
using System.IO;
using Microsoft.Xna.Framework;

namespace Elysian_Fields
{
    class Map
    {

        public List<Tile> Tiles = new List<Tile>();
        public List<Creature> Creatures = new List<Creature>();
        public List<Player> Players = new List<Player>();
        public List<Entity> Food = new List<Entity>();
        public List<Item> Items = new List<Item>();

        private DrawEngine draw;

        public int CreatureCount;

        public int AddSuperPowerSteps;

        public bool FriendlyFire;

        private Random Generator = new Random();

        private Coordinates windowSize;

        public Map() { }

        public Map(Coordinates WindowSize) { windowSize = WindowSize; }

        public Map(DrawEngine drawEngine, int SuperPowerStepsPerFood, bool friendlyFire)
        {
            draw = drawEngine;
            //Creatures.Add(new Creature("P", new Coordinates(0, 0), 1, ConsoleColor.Green, 1)); <- old way of doing it
            //Players.Add(new Player("P", new Coordinates(0, 0), ConsoleColor.Green, 1, 1));

            /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
            if (bool.Parse(ConfigurationManager.AppSettings["PlayerDebug"])) { Players[0].SuperPowerSteps = 15000; }

            AddSuperPowerSteps = SuperPowerStepsPerFood;
            FriendlyFire = friendlyFire;
        }

        public void GeneratePathFromCreature(Creature FromCreature, Coordinates Target)
        {
            FromCreature.Destination = Target;
            FromCreature.ResetPath();
            AI ai = new AI(this);
            FromCreature.Path = ai.PathTo(Target, FromCreature.Position);
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
                Players[0].Name = "Couldn't load map";
            }
        }

        public void GeneratePaths()
        {
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (GetPlayerByID(Creatures[i].TargetID).Health > 0)
                {
                    if (Creatures[i].Health > 0) // && !IsAdjacent(Creatures[i], GetCreatureByID(Creatures[i].TargetID)))
                    {
                        if (!Creatures[i].hasPath() || (Creatures[i].hasPath() && !SamePosition(Players[0].Position, Creatures[i].Destination)))
                        {
                            /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
                            GeneratePathFromCreature(Creatures[i], Players[0].Position);
                            Coordinates next = Creatures[i].NextStep();
                            if (next != null)
                            {
                                Creatures[i].Position = new Coordinates(next.X, next.Y);
                            }
                            //Creatures[i].Position = new Coordinates(next.X * Coordinates.Step, next.Y * Coordinates.Step);
                            //draw.MoveObject(Creatures[i], Creatures[i].NextStep());
                        }
                    }
                }
            }
        }

        public void MovePlayer()
        {
/* TODO: Add multiplayer functionality and change Players[0] to a variable */

                if (Players[0].Health > 0 && Players[0].hasPath())
                {
                    MoveCreature(Players[0], Players[0].NextStep());
                }
        }

        public void MoveCreature(Entity creature, Coordinates step)
        {
            if (IsTileWalkable(step) && DistanceTo(creature.Position, step) == Coordinates.Step)
            {
                creature.Position = new Coordinates(step.X, step.Y);
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

        public bool AllDead()
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

        public void ResetExperience()
        {
            for (int i = 0; i < Creatures.Count; i++)
            {
                Creatures[i].Experience = 0;
            }
        }

        public int CreatureAttack(Creature creature, Player target)
        {
            if (CanAttack(creature, target))
            {
                int dmgDealt = target.ReceiveDamage(creature.Strength, target.TotalDefense());
                if (target.Health < 1)
                {
                    creature.Experience += 1 + target.Experience;
                }
                return dmgDealt;
            }
            return -1;
        }

        public int PlayerAttack(Player player)
        {

            if (player.TargetID != -1)
            {
                Creature target = GetCreatureByID(player.TargetID);
                if (target.Name != "null")
                {
                    if (CanAttack(player, target) && DistanceToDiagonal(player.Position, target.Position) < 46) // < 46 because when standing diagonal, the distance is 45, when standing directly in front, it is 32
                    {
                        int dmgDealt = target.ReceiveDamage(player.TotalStrength(), target.Defense);
                        if (target.Health < 1)
                        {
                            player.Experience += 1 + target.Experience;
                            player.TargetID = -1;
                        }
                        return dmgDealt;
                    }
                }
            }
            return -1;
        }

        public List<DamageObject> PlayerCastSpell(Player player, Spell spell, GameTime gameTime)
        {
            List<DamageObject> DamagedMonsters = new List<DamageObject>();
            for (int i = 0; i < spell.Area.Length / 3; i++)
            {
                for (int j = 0; j < spell.Area.Length / 3; j++)
                {
                    if (spell.Area[i + j])
                    {
                        int creatureID = GetCreatureIDFromTile(new Coordinates(player.Position.X - Coordinates.Step + (i * Coordinates.Step), player.Position.Y - Coordinates.Step + (j * Coordinates.Step)));
                        if(creatureID != -1)
                        {
                            Creature creature = GetCreatureByID(creatureID);
                            if(creature.Health > 0)
                            {
                                int DamageDealt = creature.ReceiveDamage(spell.Damage, 0);
                                int currentTime = (int) gameTime.TotalGameTime.TotalMilliseconds;
                                DamagedMonsters.Add(new DamageObject(creature, DamageDealt, currentTime, currentTime + DamageObject.DamageDuration));
                            }
                        }
                    }
                }
            }

            return DamagedMonsters;
        }


        public bool CanAttack(Creature creature, Creature target)
        {
            if (target.Health > 0 && creature.Health > 0)
            {
                return true;
            }
            return false;
        }

        public bool Eat(Player creature, Coordinates step)
        {
            /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
            if (IsTileFood(step))
            {
                SetFoodEaten(step);
                draw.MoveObject(creature, step);
                creature.SuperPowerSteps += AddSuperPowerSteps;
            }
            return true;
        }

        public void SetFoodEaten(Coordinates step)
        {
            for (int i = 0; i < Food.Count; i++)
            {
                if (SamePosition(step, Food[i].Position) && Food[i].Visible)
                {
                    Food[i].Visible = false;
                }
            }
        }

        public int GetEntityTypeFromTile(Coordinates Tile)
        {
            if (IsTileCreature(Tile)) return Entity.CreatureEntity;
            if (IsTilePlayer(Tile)) return Entity.PlayerEntity;
            if (IsTileFood(Tile)) return Entity.ItemEntity;
            return Entity.UnknownEntity;
        }

        public bool IsAdjacent(Entity Object1, Entity Object2)
        {
            return (DistanceToDiagonal(Object2.Position, Object1.Position) < 46);
        }

        public int DistanceTo(Coordinates Source, Coordinates Destination)
        {
            return (Math.Abs((Source.X - Destination.X) + Math.Abs(Source.Y - Destination.Y)));
        }

        public int DistanceToDiagonal(Coordinates Source, Coordinates Destination)
        {
            return (int)Math.Sqrt(Math.Pow((Source.X - Destination.X), 2) + Math.Pow(Source.Y - Destination.Y, 2));
        }

        public int GetTileIDFromTile(Coordinates Tile)
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

        public Tile GetTileByID(int ID)
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

            if(TileID == -1)
            {
                return new Tile("null");
            }

            return Tiles[TileID];
        }
        
        public void DragItem(Item item, Coordinates Target)
        {
            bool tileThrowable = true;

            int TileID = GetTileIDFromTile(Target);
            if (TileID != -1) /* TODO: When map is finished, IsTileWalkable() should return false; if TileID == -1 (it should not be possible to walk where there is no sprite)*/
            {
                tileThrowable = GetTileByID(TileID).Walkable;
            }
            if (tileThrowable)
            {
                tileThrowable = AdjacentToItem(Players[0], item);
            }
            if (tileThrowable)
            {
                item.Position = new Coordinates(Target.X, Target.Y);
            }
        }

        public bool AdjacentToItem(Player player, Item item)
        {
            if (DistanceToDiagonal(Players[0].Position, item.Position) < 46)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void EquipItem(Item item, UI equipment, UI sourceEquipment = null, bool haveToBeAdjacent = true)
        {
            if (AdjacentToItem(Players[0], item) || haveToBeAdjacent == false)
            {
                item.Position = new Coordinates(equipment.Position.X, equipment.Position.Y);
                switch (equipment.Name)
                {
                    case ItemSlot.LeftHand:
                        Players[0].EquipItem(item, ItemSlot.LeftHand);
                        break;
                    case ItemSlot.RightHand:
                        Players[0].EquipItem(item, ItemSlot.RightHand);
                        break;
                    default:
                        break;

                }

                if (sourceEquipment != null)
                {
                    switch (sourceEquipment.Name)
                    {
                        case ItemSlot.LeftHand:
                            Players[0].UnequipItem(ItemSlot.LeftHand);
                            break;
                        case ItemSlot.RightHand:
                            Players[0].UnequipItem(ItemSlot.RightHand);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void UnequipItem(Item item, UI equipment, Coordinates target)
        {
            item.Position = new Coordinates(target.X, target.Y);
            if (equipment.Name == ItemSlot.LeftHand)
            {
                Players[0].UnequipItem(ItemSlot.LeftHand);
            }
            if (equipment.Name == ItemSlot.RightHand)
            {
                Players[0].UnequipItem(ItemSlot.RightHand);
            }
        }

        public bool IsTileWalkable(Coordinates Tile)
        {
            bool tileWalkable = true;

            int TileID = GetTileIDFromTile(Tile);
            if (TileID != -1) /* TODO: When map is finished, IsTileWalkable() should return false; if TileID == -1 (it should not be possible to walk where there is no sprite)*/
            {
                tileWalkable = GetTileByID(TileID).Walkable;
            }

            if (tileWalkable) { tileWalkable = !IsTilePlayer(Tile); }
            if (tileWalkable) { tileWalkable = !IsTileCreature(Tile); }
            if (tileWalkable) { tileWalkable = !IsTileFood(Tile); }
            if (tileWalkable) { tileWalkable = !OutOfBoundaries(Tile); }

            return tileWalkable;
        }

        public bool IsTilePlayer(Coordinates Tile)
        {
            /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
            if (SamePosition(Tile, Players[0].Position) && Players[0].Health > 0)
            {
                return true;
            }
            return false;
        }

        public bool IsTileCreature(Coordinates Tile)
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

        public bool IsTileAnimate(Coordinates Tile)
        {
            return IsTileCreature(Tile) || IsTilePlayer(Tile);
        }

        public bool IsTileFood(Coordinates Tile)
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

        public int GetCreatureIDFromTile(Coordinates Tile)
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

        public Creature GetCreatureByID(int ID)
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

        public Player GetPlayerByID(int ID)
        {
            /* TODO: Change Players[0] to a variable ID to allow for multiplayer */
            for (int i = 0; i < Players.Count;i++)
            {
                if(Players[i].ID == ID)
                {
                    return Players[i];
                }
            }
            return new Player();

            //return Players[0];
        }

        public Entity GetCreatureByName(string Name)
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

        public bool SamePosition(Coordinates Source, Coordinates Destination)
        {
            return (Source.X == Destination.X && Source.Y == Destination.Y);
        }

        public bool OutOfBoundaries(Coordinates Coordinates)
        {
            return !(Coordinates.X >= 0 && Coordinates.Y >= 0 && Coordinates.X < windowSize.X && Coordinates.Y < windowSize.Y);
        }


        public void DrawTiles()
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i].Visible)
                {
                    draw.DrawObject(Tiles[i]);
                }
            }
        }

        public void DrawCreatures()
        {
            for (int i = 0; i < Creatures.Count; i++)
            {
                if (Creatures[i].Health > 0)
                {
                    draw.DrawObject(Creatures[i]);
                }
            }
        }

        public void DrawPlayers()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Health > 0)
                {
                    draw.DrawObject(Players[i]);
                }
            }
        }

        public void DrawFood()
        {
            for (int i = 0; i < Food.Count; i++)
            {
                if (Food[i].Visible)
                {
                    draw.DrawObject(Food[i]);
                }
            }
        }

        public void DrawMap()
        {
            DrawTiles();
            DrawCreatures();
            DrawFood();
            DrawPlayers();
        }
    }
}
