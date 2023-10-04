﻿namespace TinyCiv.Shared.Game;

public enum GameObjectType
{
    Empty,

    // UNITS
    Warrior,
    Cavalry,
    Colonist,
    Tarran,

    // STRUCTURES
    City,
    Bank,
    Blacksmith,
    Farm,
    Mine,
    Port,
    Market,

    // STATIC STRUCTURES
    StaticMountain,
    StaticWater
}