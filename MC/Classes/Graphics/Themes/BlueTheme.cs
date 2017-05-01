using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MC
{
    [Serializable]
    class BlueTheme : Theme
    {
        public BlueTheme()
        {
            BackColor = Brushes.LightGray;
            LVColor = new Brush[] { Brushes.AliceBlue, Brushes.White};
            FolderIconPath = @"/Images/Icons/Folder1.png";
            DriveIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\hard-drive-icon.png";
            USBIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\USB.svg";
            CDRomIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\Cd-ROM-icon.png";
        }
    }
}
