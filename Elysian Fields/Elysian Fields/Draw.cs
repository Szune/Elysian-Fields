using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Draw
    {
        public Entity emptyTile = new Entity(" ");

        private int OffsetX = 16;
        private int OffsetY = 2;

        public Draw() { }

        public void MoveObject(Entity Object, Coordinates Destination)
        {
            if (Destination != null)
            {
                ClearObject(Object);
                Object.Position = new Coordinates(Destination.X, Destination.Y);
                DrawObject(Object);
            }
        }

        public void DrawObject(Entity Object)
        {
            if (!OutOfBoundaries(Object.Position) && Object.Visible)
            {
                Console.SetCursorPosition(Object.Position.X, Object.Position.Y);
                Console.ForegroundColor = Object.Color;
                Console.Write(Object.Name);
                Console.SetCursorPosition(Object.Position.X, Object.Position.Y);
            }
        }

        public void ClearObject(Entity Object)
        {
            if (!OutOfBoundaries(Object.Position))
            {
                int sourceX = Console.CursorLeft, sourceY = Console.CursorTop;
                emptyTile.Position = new Coordinates(Object.Position.X, Object.Position.Y);
                DrawObject(emptyTile);
                Console.SetCursorPosition(sourceX, sourceY);
            }
        }

        public bool OutOfBoundaries(Coordinates Coordinates)
        {
            return !(Coordinates.X >= 0 && Coordinates.Y >= 0 && Coordinates.X < Console.WindowWidth && Coordinates.Y < Console.WindowHeight - 1);
        }

        public void StatusMessage(string Message, ConsoleColor color = ConsoleColor.White)
        {
            ClearStatusMessage();
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.ForegroundColor = color;
            Console.Write(Message);
        }

        public void ClearStatusMessage()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(new String(' ', Console.WindowWidth - 1)); // Skriv över sista textraden
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        public void GameOver()
        {
            WinLoseScreen(false);
        }

        public void Win(bool highscore)
        {
            WinLoseScreen(true, highscore);
        }

        public void WinLoseScreen(bool win, bool highscore = false)
        {
            ClearScreen();
            if (win)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            List<Coordinates> rangeList = new List<Coordinates>() { new Coordinates(0, 3), new Coordinates(11, 14) };
            DecorateLines(rangeList, "-");

            if (win)
            {
                TypeWriterWrite(" Congratulations, you slaughtered all of the ghosts!", 4);
                if (highscore)
                {
                    TypeWriterWrite(" You made it into the highscores, write your name here:", 5);
                }
                Console.SetCursorPosition(OffsetX - 4, OffsetY + 8);
                Console.WriteLine(" To play another round, press Enter.");
                Console.SetCursorPosition(OffsetX - 4, OffsetY + 9);
                Console.WriteLine(" To view highscores, press Space.");
                Console.SetCursorPosition(OffsetX - 3, OffsetY + 6);
            }
            else
            {
                TypeWriterWrite("   You were slaughtered. Better luck next time!", 4);
                Console.SetCursorPosition(OffsetX - 4, OffsetY + 8);
                Console.WriteLine("   To play another round, press Enter.");
                Console.SetCursorPosition(OffsetX - 4, OffsetY + 9);
                Console.WriteLine("   To view highscores, press Space.");
            }

        }

        public void DecorateLines(List<Coordinates> ranges, string decoration)
        {
            foreach(Coordinates Range in ranges)
            {
            for (int i = Range.X; i < Range.Y; i++)
            {
                Console.SetCursorPosition(OffsetX - 4, OffsetY + i);
                Console.Write(decoration);
                for (int j = 0; j < 27; j++)
                {
                    Console.Write(" " + decoration);
                }
            }
            }
        }

        public void TypeWriterWrite(string write, int line)
        {
            Console.SetCursorPosition(OffsetX - 4, OffsetY + line);
            for (int i = 0; i < write.Length; i++)
            {
                Console.Write(write.Substring(i, 1));
                System.Threading.Thread.Sleep(50);
            }
        }

        public void Highscores(List<Highscore> highscores)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (highscores.Count < 10)
            {
                for (int i = 0; i < highscores.Count; i++)
                {
                    Highscore(highscores[i], i + 1);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    Highscore(highscores[i], i + 1);
                }
            }

            Console.SetCursorPosition(OffsetX, OffsetY + 20);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press Enter to play.");
        }

        public void Highscore(Highscore highscore, int position)
        {
            Console.SetCursorPosition(OffsetX, OffsetY + (position * 2) - 2);
            Console.WriteLine(position + ". " + highscore.Name + " - " + highscore.DifficultyString + " - " + highscore.Score.ToString());
        }

        public void ChooseDifficulty(bool askAgain)
        {

            if (!askAgain)
            {
                ClearScreen();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(OffsetX, OffsetY);
                Console.WriteLine("Difficulty:");
                Console.SetCursorPosition(OffsetX, OffsetY + 1);
                Console.WriteLine("1. Easy");
                Console.SetCursorPosition(OffsetX, OffsetY + 2);
                Console.WriteLine("2. Normal");
                Console.SetCursorPosition(OffsetX, OffsetY + 3);
                Console.WriteLine("3. Hard");
                Console.SetCursorPosition(OffsetX, OffsetY + 4);
            }
            else
            {
                Console.SetCursorPosition(OffsetX, 7);
                Console.WriteLine("Choose either 1, 2 or 3.");
                Console.SetCursorPosition(OffsetX, 6);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write(" ");
                }
                Console.SetCursorPosition(OffsetX, 6);
            }
        }
    }
}
