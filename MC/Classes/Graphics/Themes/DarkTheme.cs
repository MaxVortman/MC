using System;
using System.Windows.Media;

namespace MC.Classes.Graphics.Themes
{
    [Serializable]
    internal class DarkTheme : Theme
    {
        public DarkTheme()
        {
            Name = "Dark";
            BackColor = Brushes.DarkKhaki;
            LvColor = new Brush[] { Brushes.Cornsilk, Brushes.SteelBlue };
            FolderIconPath = @"/Images/Icons/Folder2.png";
            DriveIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\Drive2.png";
            UsbIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\USB2.png";
            CdRomIconPath = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\CDROM2.png";
        }
    }
}
