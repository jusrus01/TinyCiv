﻿using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderAreaDecorator : BorderDecorator
    {
        private double nearbyDistance;
        private List<GameObject> gameObjects;
        public List<GameObject> highlightedGameObjects;

        public BorderAreaDecorator(BorderObject decoratedObject, double nearbyDistance, List<GameObject> gameObjects) : base(decoratedObject)
        {
            this.nearbyDistance = nearbyDistance;
            this.gameObjects = gameObjects;
            highlightedGameObjects = new List<GameObject>();
        }

        public override BorderProperties ApplyEffects()
        {
            BorderProperties borderProperties = base.ApplyEffects();
            Position decoratedPosition = _decoratedObject.Position;

            if (decoratedPosition != null)
            {
                foreach (var gameObject in gameObjects)
                {
                    var difference = gameObject.Position - decoratedPosition;
                    if (Math.Abs(difference.row) < nearbyDistance &&
                        Math.Abs(difference.column) < nearbyDistance)
                    {
                        if (gameObject.Border != null)
                        {
                            highlightedGameObjects.Add(gameObject);
                            gameObject.Border.BackgroundBrush = borderProperties.BackgroundBrush;
                        }
                    }
                }
            }
            return borderProperties;
        }

        public override BorderProperties RemoveEffects()
        {
            foreach (var gameObject in highlightedGameObjects)
            {
                if (gameObject.Border != null)
                {
                    gameObject.Border.BackgroundBrush = Brushes.Transparent;
                }
            }

            return base.RemoveEffects(); ;
        }
    }
}
