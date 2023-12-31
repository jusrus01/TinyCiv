﻿using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services
{
    public interface IResourceService
    {
        public Resources InitializeResources(Guid playerId);
        public void AddBuilding(Guid playerId, IBuilding building, Action<Resources> callback);
        public void AddResources(Guid playerId, ResourceType resourceType, int amount);
        public Resources GetResources(Guid playerId);
        Resources? BuyInteractable(Guid playerId, IInteractableInfo info);
        void CancelInteractablePayment(Guid playerId, IInteractableInfo? info);
    }
}
