using System;

namespace TinyCiv.Client.Code.MVVM.Model
{
    public class ClockModel : ObservableObject
    {
        public string ImagePath { get; set; }

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

        public ClockModel(string imagePath, TimeSpan remainingTime)
        {
            ImagePath = imagePath;
            RemainingTime = remainingTime;
        }
    }
}
