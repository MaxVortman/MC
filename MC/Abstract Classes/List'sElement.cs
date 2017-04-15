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

        protected abstract void GetAndSetInfo();

        protected string FormatSize(long size)
        {
            string stringSize = "";
            double size_i = Convert.ToDouble(size);
            double d23 = Math.Pow(2, 33);
            if (size_i >= d23)
            {
                size_i /= d23;
                stringSize = String.Format("{0:f} GB", size_i);
            }
            else if (size_i >= 1024 * 8 * 1024)
            {
                size_i /= (1024 * 8 * 1024);
                stringSize = String.Format("{0:f} MB", size_i);
            }
            else if (size_i >= 1024 * 8)
            {
                size_i /= (1024 * 8);
                stringSize = String.Format("{0:f} KiB", size_i);
            }
            else
                stringSize += " B";

            return stringSize;
        }

        public abstract bool Open();
    }
}
