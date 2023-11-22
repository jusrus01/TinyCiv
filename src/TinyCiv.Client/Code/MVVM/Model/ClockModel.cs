using System;
using System.Windows.Controls;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.Model
{
    public class ClockModel : ObservableObject
    {
        public IBuyable BuyableObject { get; set; }
        public Image ImagePath { get; set; }

        private TimeSpan remainingTime;
        public TimeSpan RemainingTime
        {
            get => remainingTime;
            set
            {
                remainingTime = value;
                OnPropertyChanged(nameof(RemainingTime)); // Notify property change
            }
        }

        public ClockModel(IBuyable buyableObject, Image imagePath, TimeSpan remainingTime)
        {
            BuyableObject = buyableObject;
            ImagePath = imagePath;
            RemainingTime = remainingTime;
        }
    }
}
