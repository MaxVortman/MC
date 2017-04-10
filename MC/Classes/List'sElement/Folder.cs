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
            Image = "/Images/Icons/Folder1.png";
            DirectoryInfo dir = new DirectoryInfo(Path);
            Name = dir.Name;
            try
            {
                int count = dir.GetFileSystemInfos().Count();
                Size = count.ToString() + " item";
                if (count != 1)
                    Size += "s";
            }
            catch (UnauthorizedAccessException)
            {
                Size = "?? items";    
            }
            Date = Convert.ToString(dir.CreationTime);
        }

        public override bool Open()
        {
            return true;
        }

    }
}
