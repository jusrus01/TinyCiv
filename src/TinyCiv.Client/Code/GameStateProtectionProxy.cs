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
        private readonly Guid ownerId;

        public GameStateProtectionProxy(GameState gameState, Guid ownerId) : base(gameState.Rows, gameState.Columns)
        {
            this.ownerId = ownerId;
        }

        protected override void SelectCity(GameObject gameObject)
        {
            if (ownerId == gameObject.OwnerId)
            {
                SelectCity(gameObject);
            }
        }

        protected override void SelectUnit(GameObject gameObject)
        {
            if (ownerId == gameObject.OwnerId)
            {
                SelectUnit(gameObject);
            }
        }
    }
}
