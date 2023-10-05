namespace TinyCiv.Client.Code.MVVM
{
    public class ObservableValue<T> : ObservableObject
    {
        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public ObservableValue() { }
        public ObservableValue(T defaultValue)
        {
            _value = defaultValue;
        }
    }
}
