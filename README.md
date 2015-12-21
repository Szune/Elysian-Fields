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
- [x] Scripted spells
- [x] Loading and saving monsters in map editor and game
- [x] Loading and saving items in map editor and game
- [x] Monsters being able to cast spells
- [ ] Improved pathfinding algorithm
- [ ] Random loot system

#NPC and spell scripts

##Spell Scripts
Spells can now be created by simply placing a lua file in the folder "<PathToElysian Fields.exe>\Content\Scripts\Spells"
These spells can be used by monsters as well as players, depending on how you set up the player UI.

###Example spells
Heal:
```
ID = 2
Name = 'Heal'
Damage = 50
SpriteName = 'Spell_HealSpell'
Cooldown = 500
Mana_Cost = 5
Spell_Heal = true
Spell_RequireTarget = false
Spell_Area = false
```

Area damage spell:
```
ID = 1
Damage = 50
Name = 'Fist'
SpriteName = 'Spell_FistSpell'
Mana_Cost = 20
Cooldown = 1000
Spell_Heal = false
Spell_RequireTarget = false
Spell_Area = true
Area = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,
	0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,
	0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,
	0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,
	0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
```

Area damage spell used by a monster:
```
ID = 500
Damage = 150
Name = 'DinosaurFire'
SpriteName = 'Spell_DinosaurFire'
Mana_Cost = 0
Cooldown = 4000
Spell_Heal = false
Spell_RequireTarget = false
Spell_Area = true
Area = {0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,
	0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,
	0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,
	1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
	0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,
	0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,
	0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,
	0,0,0,0,0,0,0,1,0,0,0,0,0,0,0}
```

##NPC Scripts
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
