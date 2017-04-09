using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MC
{
    class Folder : List_sElement
    {
        public Folder(string Path) : base(Path)
        {
        }

        protected override void GetAndSetInfo()
        {
            Image = "/Images/Icons/Folder.png";
            DirectoryInfo dir = new DirectoryInfo(Path);
            Name = dir.Name;
            Size = "";
            Date = Convert.ToString(dir.CreationTime);
        }
    }
}
