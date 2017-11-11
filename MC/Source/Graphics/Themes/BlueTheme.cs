using System;
using System.Windows.Media;

namespace MC.Source.Graphics.Themes
{
    [Serializable]
    internal class BlueTheme : Theme
    {
        public BlueTheme()
        {
            Name = "Blue";
            BackColor = Brushes.LightGray;
            LvColor = new Brush[] { Brushes.AliceBlue, Brushes.White};
            FolderIconPath = @"/Images/Icons/Folder1.png";
            DriveIconPath = @"/Images/Icons/hard-drive-icon.png";
            UsbIconPath = @"/Images/Icons/USB.svg";
            CdRomIconPath = @"/Images/Icons/Cd-ROM-icon.png";
        }
    }
}
