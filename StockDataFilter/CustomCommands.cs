using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockDataFilter
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Generate = new RoutedUICommand
            (
                "Generate",
                "Generate",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                                new KeyGesture(Key.G, ModifierKeys.Control)
                }
            );
    }
}
