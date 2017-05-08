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
            Name = "Blue";
            BackColor = Brushes.LightGray;
            LVColor = new Brush[] { Brushes.AliceBlue, Brushes.White};
            FolderIconPath = @"/Images/Icons/Folder1.png";
            DriveIconPath = @"/Images/Icons/hard-drive-icon.png";
            USBIconPath = @"/Images/Icons/USB.svg";
            CDRomIconPath = @"/Images/Icons/Cd-ROM-icon.png";
        }
    }
}
