# Project Bueno
Project Bueno the - Currently in alpha
#### [Latest Compile](https://www.dropbox.com/sh/tylckc25tfdv41r/AAB4olM2lmImcnB-SQHdBNc7a?dl=1)
#### [Todo List](ToDo.txt)
#### [Design](https://www.dropbox.com/s/ci4adh4dlwas6gk/Design.txt?raw=1)

## How to play
####Controls: 
* WASD - Move
* 12345 - Cast spells
* Backspace - Enter/Exit spell creation menu
* P - Pause/Unpause
* **In spell creation menu:**
	* Left Mouse Button - Buy skill / Pick up skill / Place skill in spell / Clear picked up skill (when clicking on empty space) / Remove skill from spell (when clicking with no picked up spell)
* **Debug Controls:**
	* M - Show Minimap
	* K - Clear all enemies
	* K (in spell menu) - Set KP to 10000
	* C - Toggle collision box drawing
	* Left Shift - Increase speed to 60 blocks/s (60 pixels/s on minimap) and ignore collision with sea
	* Up Arrow - Double the zoom level
	* Down Arrow - Halve the zoom level

## How to build
1. Clone this repo (git clone https://github.com/FeldrinH/ProjectBueno.git).
2. Open **ProjectBueno.sln** in Visual Studio 2015.
3. Build with **Release (Copy Deps)** configuration to copy dependencies into output folder (Allow NuGet to restore dependencies).
4. Build with **Release** configuration unless you need to update dependencies.

Note that all configurations build into the same folder (ProjectBueno-Release)
