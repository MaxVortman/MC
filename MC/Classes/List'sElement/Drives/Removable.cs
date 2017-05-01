using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC
{
    class Removable : Drive
    {
        public Removable(DriveInfo driveInfo) : base(driveInfo)
        {
            Image = MainWindow.userPrefs?.Theme.USBIconPath;
        }
    }
}
