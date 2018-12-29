# DungeonsGenerator
Dungeons generator created as a fork of the one used for [Once upon a Dungeon](https://store.steampowered.com/app/772090/Once_upon_a_Dungeon/) game.

Features:
- generates a dungeon with a desired number of rooms (nodes)
- generates doors allowing to traverse the dungeon 
- written in C# 3.5 (to allow integration with Unity 3D)

Usage:
```
var generator = new Generator();
var level = generator.Run();
level.Print();
```
Above code will display something like:
[Sample Dungeon](DungeonsConsoleRunner/samples/SampleDungeon.png)
