# Introduction 
The project builds a BBC Micro Model B platform game which demonstrates many aspects of game development.  There is an accompanying development.docx file which provides a journal of the development which may be of interest.  The project also contains a level editor that can be used to author new, or edit existing levels.

# Getting Started
The solution contains four projects: Game, LevelEditor, BMPConverter, MIDIConverter

Game
----
This project contains the game sources, and builds a program.SSD file which can be run in a BBC Model B emulator.
The main program is called program.6502. It in turn includes other source files.

The build uses a custom build step to invoke beebasm.exe which is used to assemble the code and create the SSD. The build step will likely need updating to point to the location of beebasm.exe on your machine.

The project invokes BeebEm.exe to run/debug the game. The location of the emulator will likely need updating to point to the location of the emulator you are using.

For 6502 syntax highlighting in Visual Studio, copy the Extensions folder to: C:\Users\<user>\.vs\ and restart Visual Studio.

LevelEditor
-----------------
This project contains a Windows Forms based level editor, that allows levels to be created and edited. Instructions on how to use the editor can be found bug clicking on the question mark icon in the toolbar.  The levels files are located in \game folder.

BMPConverter
------------
This project builds a console app that converts 32-bit BMP level assets (created in paint.net) into level atlas files, which are then packaged with other intermediate level files to create levels within the game image.

The app should be run after any BMP asset file changes are made, followed by a rebuild of the Game project; so the new level atlas files are repackaged with the game.

Each level's BMP assets are held in a folder named game\assets\levelN\, where N is the level number

Note that the colors used within the BMP asset files must be a close match to the colors defined in the associated level's palette.

MIDIConverter
-----------------
This project builds a console app then converts a type-0 MIDI file into a more compact format that is binary included in music.6502. The app also generates a pitch table which is embedded in music.6502.


Other files
-----------
Development.docx provides details on the development of the game.  This may be of interest
memory.xlsx was used to help determine the memory locations of sprite assets and other program data.
music.sib is a Sibelius music score containing the Doctor Who tune and arrangement by Ron Grainer & Delia Derbyshire.
music.midi is a type-0 midi file created by Sibelius.

# Build and Test
Per above, ensure the Daleks project Debugging and Custom Build Step are set up correctly. Then invoke the 'Build All' command.

# Contribute
TBD
