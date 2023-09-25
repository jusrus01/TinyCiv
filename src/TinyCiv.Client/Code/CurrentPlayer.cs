﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class CurrentPlayer
    {
        private static readonly CurrentPlayer instance = new CurrentPlayer();

        public static CurrentPlayer Instance { get { return instance; } }

        public static Guid Id { get { return instance.player.Id; } }

        public static TeamColor Color { get { return instance.player.Color; } }

        //-------------------------

        public Player player;

        private CurrentPlayer() {}
    }
}