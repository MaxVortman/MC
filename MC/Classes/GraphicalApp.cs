﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC
{
    class GraphicalApp
    {

        private MainWindow window;

        public GraphicalApp(MainWindow window)
        {
            this.window = window;
        }

        public void AddLine(List_sElement elem)
        {
            window.ListView1.Items.Add(elem);
        }
    }
}