using System;
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
        public string Name { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }
        public string Path { get; protected set; }

        public List_sElement(string Path)
        {
            this.Path = Path;
            GetAndSetInfo();
        }

        protected abstract void GetAndSetInfo();

        public abstract bool Open();
    }
}
