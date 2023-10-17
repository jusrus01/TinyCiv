using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderAreaDecorator : BorderDecorator
    {
        private double nearbyDistance;
        private List<GameObject> gameObjects;
        private List<GameObject> highlightedGameObjects;

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
                double squareLeft = decoratedPosition.column - nearbyDistance;
                double squareTop = decoratedPosition.row - nearbyDistance;
                double squareRight = decoratedPosition.column + nearbyDistance;
                double squareBottom = decoratedPosition.row + nearbyDistance;

                foreach (var gameObject in gameObjects)
                {
                    if (gameObject.Position.column >= squareLeft &&
                        gameObject.Position.column <= squareRight &&
                        gameObject.Position.row >= squareTop &&
                        gameObject.Position.row <= squareBottom)
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
