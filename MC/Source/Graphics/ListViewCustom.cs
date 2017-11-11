using System.Windows;
using System.Windows.Controls;
using MC.Windows;

namespace MC.Source.Graphics
{
    public class ListViewCustom : ListView
    {

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element == null) return;
            var index = ItemContainerGenerator.IndexFromContainer(element);
            var lbItem = element as ListBoxItem;
            if (lbItem != null)
                lbItem.Background = index % 2 == 0
                    ? MainWindow.UserPrefs?.Theme.LvColor[0]
                    : MainWindow.UserPrefs?.Theme.LvColor[1];
        }
    }
}
