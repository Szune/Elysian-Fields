using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicLua;

namespace Elysian_Fields.Modules.AI
{
    class NPC : Creature
    {
        dynamic lua = new DynamicLua.DynamicLua();
        string PathToLua;
        bool Greeted = false;
        bool Talkative = false;
        Map map = Map.Instance;

        public NPC(string path)
        {
            LoadScript(path);
        }

        public void LoadScript(string path)
        {
            PathToLua = path;
            lua.DoFile(path);
            Name = (string) lua.name;
            Position = new Coordinates((int)lua.pos.x, (int)lua.pos.y, (int)lua.pos.z);
            Talkative = lua.talkative;
            if (Talkative)
            {
                Greeted = lua.auto_greeted;
            }
            lua.CreateItem = new Action<int, int, int, int>(map.NPC_CreateItem);
            lua.SpawnMonster = new Action<int, int, int, int>(map.NPC_CreateCreature);
        }

        public string Chat(string word, Player player)
        {
            if (word.Trim() != "")
            {

                if (lua.talkative)
                {
                    if (word == lua.greeting && !Greeted)
                    {
                        string answer = lua.greeting_answer;
                        Greeted = true;
                        return answer.Replace("%p", player.Name);
                    }
                    else if (Greeted && word != lua.greeting)
                    {
                        string msg = lua.Chat(word, player.ID);
                        if (msg.Length > 0)
                        {
                            return msg;
                        }
                        return "";

                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
    }
}
