using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;

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
