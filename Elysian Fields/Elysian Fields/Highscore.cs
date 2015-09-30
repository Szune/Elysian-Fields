using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Highscore : IComparable
    {
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Difficulty { get; set; }
        public string DifficultyString { get; set; }
        public int SuperPower { get; set; }
        public int Score { get; set; }

        public Highscore(string name, int score, string difficulty)
        {
            Name = name;
            Score = score;
            DifficultyString = difficulty;
        }

        public Highscore(string name, int experience, int difficulty, int superPower, string difficultyString)
        {
            Name = name;
            Experience = experience;
            Difficulty = difficulty;
            SuperPower = superPower;
            Score = (difficulty * experience * 50) + (difficulty * superPower);
            DifficultyString = difficultyString;
        }

        public int CompareTo(object obj)
        {
            var compareScore = (Highscore)obj;
            if (Score == compareScore.Score)
            {
                return 0;
            }
            if (Score < compareScore.Score)
            {
                return 1;
            }
            return -1;
        }
    }
}
