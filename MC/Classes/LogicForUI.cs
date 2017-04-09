using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MC
{
    class LogicForUI
    {
        private GraphicalApp graphics;


        public LogicForUI(GraphicalApp graphics)
        {
            this.graphics = graphics;
        }

        public void FillInList(string path)
        {
            //enumerate folder's path
            foreach (var item in Directory.EnumerateDirectories(path))
            {

                graphics.AddLine(new Folder(item));
            }
            //enumerate file's path
            foreach (var item in Directory.EnumerateFiles(path))
            {
                graphics.AddLine(new File(item));
            }

        }
    }
}

