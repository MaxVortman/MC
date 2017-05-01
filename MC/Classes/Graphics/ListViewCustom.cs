﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC
{
    public class ListViewCustom : ListView
    {

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            int index = ItemContainerGenerator.IndexFromContainer(element);
            ListBoxItem lbItem = element as ListBoxItem;
            List_sElement p = (List_sElement)item;
            if (index % 2 == 0)
            {
                lbItem.Background = MainWindow.userPrefs?.Theme.LVColor[0];
            }
            else
            {
                lbItem.Background = MainWindow.userPrefs?.Theme.LVColor[1];
            }
        }
    }
}
