using System.IO;
using MC.Windows;

namespace MC.Source.Entries.Drives
{
    internal class Fixed : Drive
    {
        public Fixed(DriveInfo driveInfo) : base(driveInfo)
        {
            Image = MainWindow.UserPrefs?.Theme.DriveIconPath;
        }
    }
}
