# HOLLOW KNIGHT CONSOLE COMMANDS <3

Adds several console commands to Hollow Knight. Why? Because we can, of course! Ever wanted to just play around in a really hard game?
Infinite health! Infinite money! Godmode! Instakill! all ya could want, unless it isn't, in which case:
I am open to suggestions for commands, and have plenty of free time. Just drop a suggestion at Daltonyx#4105 on discord and I'll get working on it!

this will be on nexusmods soon with an easy installer for windows, but I will update the .dll here in releases as I think about it for any early birds

## Installation

To install this project, follow these steps:

1. Download bepinex 6.0.0pre-1 (I use bepinex 6 from the github here: https://github.com/BepInEx/BepInEx/releases) DOES NOT WORK WITH BEPINEX 5.4
	
	1.a. Extract all files from the downloaded zip to your Hollow Knight installation. (Normally 'Program Files x86/steam/steamapps/common/Hollow Knight')

2. Start the game at least once, to populate bepinex folders

3. Put this ConsoleCommands.DLL into "steamapps/common/Hollow Knight/BepInEx/Plugins" and start game!

## To Compile Yourself

###WINDOWS
To compile this yourself (i get it man, who knows what other folks are doing), follow these steps:

1. clone this repository wherever you feel like
2. cd into the directory where you cloned it (powershell, quake window, cmd, etc)
3. run dotnet build (in the terminal from step 2)
NOTE: I think it's .net6 it uses

## Usage

To use this project, follow these steps:

1. When actually in game (not at the title screen!) press / to open the console
2. Type your command
4. Press return (enter) to send the command.

## Current Commands
/addhealth (int) - adds the amount entered to health and maxHealth both
	
/addmp (int) - adds the amount entered to MP and maxMP both
	
/addmoney (int) - adds the amount entered to the Banker
	
/achget - unlocks _ALL_ 63 achievements (requires the game on steam i think /shrug)
	
/godmode - enables godmode (as far as I can tell)
	
/xdamage - enables instakill (damage of nail and beam set to 999)
