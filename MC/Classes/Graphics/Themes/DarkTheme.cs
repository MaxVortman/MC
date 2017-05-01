using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MC
{
    [Serializable]
    class DarkTheme : Theme
    {
        public DarkTheme()
        {
            BackColor = Brushes.DarkKhaki;
            LVColor = new Brush[] { Brushes.Cornsilk, Brushes.SteelBlue };
            FolderIconPath = @"/Images/Icons/Folder2.png";
            DriveIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\Drive2.png";
            USBIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\USB2.png";
            CDRomIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\CDROM2.png";
        }
    }
}
