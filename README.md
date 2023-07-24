# HOLLOW KNIGHT CONSOLE COMMANDS <3

Adds several console commands to Hollow Knight. Why? Because we can, of course! Ever wanted to just play around in a really hard game?
Infinite health! Infinite money! Godmode! Instakill! all ya could want, unless it isn't, in which case:
I am open to suggestions for commands, and have plenty of free time. Just drop a suggestion at Daltonyx#4105 on discord and I'll get working on it!

## Usage
1. When actually in game (not at the title screen!) press / to open the console
2. Type your command
4. Press return (enter) to send the command.

## Current Commands
/addhealth (int) - adds the amount entered to health and maxHealth both

/sethealth (int) - sets health to whatever choice is

/setbdamage (int 1-5) - sets beam damage to corresponding upgrade value (goes above actual game values past 3, be careful!)

/setndamage (int 1-5) - sets nail damage to corresponding upgrade value (goes above actual game values, be careful!)
 
/addmp (int) - adds the amount entered to MP (referred to as soul in-game, MP in code)

/addmpreserve (int) - adds the amount entered to MPReserve and MP Cap
	
/addmoney (int) - adds the amount entered to the Banker

/addore (int) - adds the amount entered to Ore

/addvessels (int) - adds the amount entered to Vessel Fragments

/addgeo (int) - adds the amount entered to Geo count

/addegg (int) - adds the amount entered to rancid Eggs

/addkey (int) - adds the amount entered to Simple Keys

/addorb (int) - adds the amount entered to Dream Orbs

/addzote (int) - adds the amount entered to Zote count

/addcharm (int) - equips the selected charm (0-39)

/allcharms - unlocks all powerups

/allkeys - gives all keys

/jump (bool) - enables/disables infinite jump
	
/achget - unlocks _ALL_ 63 achievements (requires the game on steam i think /shrug)
	
/godmode (bool) - enables godmode (as far as I can tell)
	
/xdamage (bool) - enables instakill (damage of nail and beam set to 999)

/reset -resets character to base stats, and fixes any borked character (if you ever got the old damage bug from xdamage or godmode. it's patched now)

/stags - enables all stag stations


## Installation

To install this project, follow these steps:

1. Download bepinex 6.0.0pre-1. DOES NOT WORK WITH BEPINEX 5.4
	
	1.a. I made a windows easy installer you can get here: https://www.nexusmods.com/hollowknight/mods/44

	1.b. if not windows, or just want to do it yourself, download the zip for bepinex 6.0.0pre-1 from here: https://github.com/BepInEx/BepInEx/releases

	1.c. ONLY if using step 1.b, Extract all files from the downloaded zip to your Hollow Knight installation. (Normally 'Program Files x86/steam/steamapps/common/Hollow Knight')

2. Start the game at least once, to populate bepinex folders

3. Put this ConsoleCommands.DLL into "steamapps/common/Hollow Knight/BepInEx/Plugins" and start game!

## To Compile Yourself

###WINDOWS
To compile this yourself (i get it man, who knows what other folks are doing), follow these steps:

1. clone the repository wherever you feel like
2. cd into the directory where you cloned it (powershell, quake window, cmd, etc)
3. run dotnet build (in the terminal from step 2)
NOTE: I think it's .net6 it uses
