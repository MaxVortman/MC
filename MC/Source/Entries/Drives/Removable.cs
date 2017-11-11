using System.IO;
using MC.Windows;

namespace MC.Source.Entries.Drives
{
    internal class Removable : Drive
    {
        public Removable(DriveInfo driveInfo) : base(driveInfo)
        {
            Image = MainWindow.UserPrefs?.Theme.UsbIconPath;
        }
    }
}
