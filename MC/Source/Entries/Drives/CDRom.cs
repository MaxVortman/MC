using System.IO;
using MC.Windows;

namespace MC.Source.Entries.Drives
{
    internal class CdRom : Drive
    {
        public CdRom(DriveInfo driveInfo) : base(driveInfo)
        {
            Image = MainWindow.UserPrefs?.Theme.CdRomIconPath;
        }
    }
}
