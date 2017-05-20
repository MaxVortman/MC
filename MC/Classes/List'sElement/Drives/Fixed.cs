using System.IO;
using MC.Abstract_and_Parent_Classes;
using MC.Windows;

namespace MC.Classes.Drives
{
    internal class Fixed : Drive
    {
        public Fixed(DriveInfo driveInfo) : base(driveInfo)
        {
            Image = MainWindow.UserPrefs?.Theme.DriveIconPath;
        }
    }
}
