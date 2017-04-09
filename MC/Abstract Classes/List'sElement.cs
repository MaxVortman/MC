﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MC
{
    abstract class List_sElement
    {
        public object Image { get; protected set; }
        public string Name { get; protected set; }
        public string Size { get; protected set; }
        public string Date { get; protected set; }
        public string Path { get; protected set; }

        public List_sElement(string Path)
        {
            this.Path = Path;
            GetAndSetInfo();
        }

        protected abstract void GetAndSetInfo();
    }
}