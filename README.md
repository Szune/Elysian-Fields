# Elysian-Fields
A 2D game developed using MonoGame

##TODOs
- [x] Multilevel system
- [x] Scripted NPCs
- [x] Monsters
- [x] Spells
- [x] Equipment system
- [x] Saving
- [x] Loading
- [ ] Improved pathfinding algorithm
- [ ] Scripted spells
- [ ] Loading and saving monsters in map editor and game
- [ ] Loading and saving items in map editor and game
- [ ] Monsters being able to cast spells
- [ ] Random loot system

##Scripted NPCs
NPCs can now be created by simply placing a lua file in the folder "<PathToElysian Fields.exe>\Content\Scripts\NPCs"

Minimum required fields for a non-talking NPC:
```lua
name = 'NameHere' -- any name you'd like, this will be the NPCs name in-game
pos = {x = 3, y = 3, z = 0} -- set to any position you'd like
talkative = 'false' -- if set to 'true', then all the fields required for a talking NPC also have to be included
```

Minimum required fields for a talking NPC:
```lua
name = 'NameHere' -- any name you'd like
pos = {x = 3, y = 3, z = 0} -- set to any position you'd like
talkative = 'true'
auto_greeted = 'false' -- set to true if the player isn't required to greet the NPC before starting a conversation
greeting = 'hi'
greeting_answer = 'hiho, %p' -- %p is replaced by playername (only true for this specific variable)
```

Example of a talking NPC who performs actions, "Protester.lua" (also requires "funcs.lua" below):
```lua
name = 'Protester'
pos = {x = 15, y = 3, z = 0}
auto_greeted = false
talkative = true
greeting = 'hi'
greeting_answer = 'Hello, %p'
words = {'item', 'spawn'}
spawnmonsterid = 2
replies = {{isFunc = true, func = 'CreateItem', msg = 'Here is your item'}, {isFunc = true, func = 'SpawnMonster', msg = 'A cat has spawned'}}

require "Content/Scripts/NPCs/funcs"

function Chat(msg, playerid)
	for i, v in ipairs(words) do
		if (v == msg) then
			if (replies[i].isFunc) then
				if (replies[i].func == 'CreateItem') then
					doFunc(replies[i].func, playerid, pos.x, pos.y + 1, pos.z)
				elseif (replies[i].func == 'SpawnMonster') then
					doFunc(replies[i].func, spawnmonsterid, pos.x, pos.y + 1, pos.z)
				end
			end
			reply = replies[i].msg
			return reply
		end
	end
	return ""
end
```

funcs.lua:
```lua
function doFunc(func, ...)

args = {...}

	if (func == "CreateItem") then
		CreateItem(21, args[2], args[3], args[4])
	elseif (func == "SpawnMonster") then
		SpawnMonster(args[1], args[2], args[3], args[4])
	end


end
```
