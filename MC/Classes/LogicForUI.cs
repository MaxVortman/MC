using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace MC
{
    class LogicForUI
    {
        private GraphicalApp graphics;


        public LogicForUI(GraphicalApp graphics)
        {
            this.graphics = graphics;
        }

        private void FillInList(string path)
        {
            graphics.ClearList();   
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

        public void OpenElem(object elem)
        {
            //test for folder
            if (((List_sElement)elem).Open())
            {
                try
                {
                    graphics.SetCaptionOfPath(((List_sElement)elem).Path);
                    FillInList(((List_sElement)elem).Path);

                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

