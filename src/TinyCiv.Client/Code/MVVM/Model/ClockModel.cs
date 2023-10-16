using System;

namespace TinyCiv.Client.Code.MVVM.Model
{
    public class ClockModel : ObservableObject
    {
        public string Name { get; set; }
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

        public ClockModel(string name, string imagePath, TimeSpan remainingTime)
        {
            Name = name;
            ImagePath = imagePath;
            RemainingTime = remainingTime;
        }
    }
}
