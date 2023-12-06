using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameStateProtectionProxy : GameState
    {
        public GameStateProtectionProxy(GameState gameState) : base(gameState.Rows, gameState.Columns)
        {
        }

        protected override void SelectCity(GameObject gameObject)
        {
            if (CurrentPlayer.IsOwner(gameObject))
            {
                base.SelectCity(gameObject);
            }
        }

        protected override void SelectUnit(GameObject gameObject)
        {
            if (CurrentPlayer.IsOwner(gameObject))
            {
                base.SelectUnit(gameObject);
            }
        }
    }
}
