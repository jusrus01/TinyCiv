using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class GameOverViewModel : ObservableObject
    {
        public ObservableValue<String> GameOverText { get; set; } = new ObservableValue<string>();

        public GameOverViewModel(string gameOverText)
        {
            GameOverText.Value = gameOverText;
        }
    }
}
