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

                        /*try
                        {*/
                        if (lua.words != null)
                        {
                            string msg = lua.Chat(word, player.ID);
                            if (msg.Length > 0)
                            {
                                return msg;
                            }
                        }
                        //}
                        /*catch(Exception ex)
                        {
                            return ex.Message;
                        }*/
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
