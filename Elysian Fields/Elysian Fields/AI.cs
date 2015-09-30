using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Elysian_Fields
{
    class AI
    {
        private Map map { get; set; }

        private int G = int.Parse(ConfigurationManager.AppSettings["G"]);

        private DrawEngine draw = new DrawEngine();

        public AI() { }

        public AI(Map mapEngine)
        {
            map = mapEngine;
        }

        public List<Coordinates> PathTo(Coordinates Target, Coordinates FromCreature)
        {
            List<Coordinates> OpenList = new List<Coordinates>(); // Den öppna listan med rutor som vi fortfarande kollar
            List<Coordinates> ClosedList = new List<Coordinates>(); // Den stängda listan med rutor som vi inte behöver kolla längre
            Coordinates Parent = FromCreature; // Parent-rutan
            Coordinates Destination = Target; // Destinationen
            int tries = 0;
            bool havePath = false;

            OpenList.Add(Parent); // Lägg till startpunkten
            OpenList.AddRange(CheckAdjacentSquares(Parent, Parent, Destination, ClosedList, OpenList)); // Lägg till de rutor runtom som går att gå på i den öppna listan
            OpenList.Remove(Parent); // Ta bort startpunkten
            ClosedList.Add(Parent); // Lägg till startpunkten i listan med rutor vi inte behöver kolla



            while (!havePath)
            {
                if (tries == 4 * 200) { break; }
                Parent = LowestFScore(OpenList);
                if (bool.Parse(ConfigurationManager.AppSettings["DebugMode"]))
                {
                    //draw.DrawObject(new Entity("F", new Coordinates(Parent.X, Parent.Y)));   // Endast för debugging
                }
                OpenList.Remove(Parent);
                ClosedList.Add(Parent);
                OpenList.AddRange(CheckAdjacentSquares(Parent, Parent, Destination, ClosedList, OpenList));

                if (IsTileInList(Target, ClosedList)) { havePath = true; }
                if (OpenList.Count == 0 && havePath == false) { break; }
                tries += 4;
            }

            List<Coordinates> path = new List<Coordinates>();


            if (havePath)
            {
                Destination = GetTileInList(Target, ClosedList);
                path.Add(Destination);
                while (Destination.hasParent())
                {
                    if (bool.Parse(ConfigurationManager.AppSettings["DebugMode"]))
                    {
                        draw.DrawObject(new Entity("F", new Coordinates(Destination.X, Destination.Y)));      // Endast för debugging
                    }
                    Destination = Destination.Parent;
                    path.Add(Destination);
                }
            }

            return path;
        }

        public List<Coordinates> CheckAdjacentSquares(Coordinates Square, Coordinates Parent, Coordinates Destination, List<Coordinates> ClosedList, List<Coordinates> OpenList)
        {
            List<Coordinates> WalkableSquares = new List<Coordinates>();
            
            Coordinates CheckingSquare = new Coordinates(Square.X + Coordinates.Step, Square.Y); // East
            if (IsTileWalkable(CheckingSquare, ClosedList, OpenList) || SamePosition(CheckingSquare, Destination))
            {
                WalkableSquares.Add(new Coordinates(CheckingSquare.X, CheckingSquare.Y, CalculateHeuristic(CheckingSquare, Destination), (G + Parent.G), Parent));
            }
            
            CheckingSquare = new Coordinates(Square.X - Coordinates.Step, Square.Y); // West
            if (IsTileWalkable(CheckingSquare, ClosedList, OpenList) || SamePosition(CheckingSquare, Destination)) {
                WalkableSquares.Add(new Coordinates(CheckingSquare.X, CheckingSquare.Y, CalculateHeuristic(CheckingSquare, Destination), (G + Parent.G), Parent));
            }

            CheckingSquare = new Coordinates(Square.X, Square.Y - Coordinates.Step); // North
            if (IsTileWalkable(CheckingSquare, ClosedList, OpenList) || SamePosition(CheckingSquare, Destination)) {
                WalkableSquares.Add(new Coordinates(CheckingSquare.X, CheckingSquare.Y, CalculateHeuristic(CheckingSquare, Destination), (G + Parent.G), Parent));
            }

            CheckingSquare = new Coordinates(Square.X, Square.Y + Coordinates.Step); // South
            if (IsTileWalkable(CheckingSquare, ClosedList, OpenList) || SamePosition(CheckingSquare, Destination)) {
                WalkableSquares.Add(new Coordinates(CheckingSquare.X, CheckingSquare.Y, CalculateHeuristic(CheckingSquare, Destination), (G + Parent.G), Parent));
            }

            return WalkableSquares;
        }

        public bool IsTileWalkable(Coordinates Tile, List<Coordinates> ClosedList, List<Coordinates> OpenList)
        {
            return (map.IsTileWalkable(Tile) && !IsTileInList(Tile, ClosedList) && !IsTileInList(Tile, OpenList));
        }


        public int CalculateHeuristic(Coordinates Source, Coordinates Destination)
        {
            return (G + (G * (Math.Abs(Source.X - Destination.X) + Math.Abs(Source.Y - Destination.Y)))); // Manhattan-metoden
        }

        public bool IsTileInList(Coordinates Tile, List<Coordinates> List)
        {
            bool isClosed = false;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].X == Tile.X && List[i].Y == Tile.Y) { isClosed = true; }
            }

            return isClosed;
        }

        public Coordinates GetTileInList(Coordinates Tile, List<Coordinates> List)
        {
            Coordinates TileCoordinates = new Coordinates();
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].X == Tile.X && List[i].Y == Tile.Y) { TileCoordinates = List[i]; break; }
            }

            return TileCoordinates;
        }


        public Coordinates LowestFScore(List<Coordinates> OpenList)
        {
            Coordinates lowestF = new Coordinates();
            for (int i = 0; i < OpenList.Count; i++)
            {
                if (i == 0)
                {
                    lowestF = OpenList[i];
                }
                else
                {
                    if (OpenList[i].F < lowestF.F)
                    {
                        lowestF = OpenList[i];
                    }
                }
            }
            return lowestF;
        }

        public bool SamePosition(Coordinates Source, Coordinates Destination)
        {
            return (Source.X == Destination.X && Source.Y == Destination.Y);
        }
    }
}
