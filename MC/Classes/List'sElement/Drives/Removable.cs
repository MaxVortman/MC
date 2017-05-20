using System.IO;
using MC.Abstract_and_Parent_Classes;
using MC.Windows;

namespace MC.Classes.Drives
{
    internal class Removable : Drive
    {
        public Removable(DriveInfo driveInfo) : base(driveInfo)
        {
            Image = MainWindow.UserPrefs?.Theme.UsbIconPath;
        }
    }
}
