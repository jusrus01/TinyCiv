﻿using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.units
{
    public class Tarran : Unit
    {
        public override int Health => 60;
        public override int MaxHealth => 60;
        public override int Damage => 10;
        public override int Speed => 1;
        public override int ProductionPrice => 50;
        public override int ExpReward => 50;
        public override string Description => "5x more damage against the cities and 5x reduced damage from them";

        public Tarran(ServerGameObject serverGameObject) : base(serverGameObject)
        {
        }
    }
}
