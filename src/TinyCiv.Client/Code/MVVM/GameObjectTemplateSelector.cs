using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code.MVVM
{
    public class GameObjectTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UnitTemplate { get; set; }
        public DataTemplate GameObjectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Unit)
            {
                return UnitTemplate;
            }
            return GameObjectTemplate;
        }
    }
}
