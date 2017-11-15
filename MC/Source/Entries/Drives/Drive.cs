using System.IO;

namespace MC.Source.Entries.Drives
{
    internal abstract class Drive : Directory
    {
        public bool IsReady { private set; get; }
        public string TotalSize { set; get; }
        private DriveInfo _driveInfo;

        protected Drive(DriveInfo driveInfo)
        {
            IsReady = driveInfo.IsReady;
            _driveInfo = driveInfo;
            GetAndSetInfo();
        }

        private void GetAndSetInfo()
        {
            Path = _driveInfo.RootDirectory.ToString();
            Name = _driveInfo.Name;

            if (IsReady)
            {
                Size = "Free Size: " + FormatSize(_driveInfo.TotalFreeSpace * 8);
                TotalSize = "Total Size: " + FormatSize(_driveInfo.TotalSize * 8);
            }
            Image = "/Images/Icons/Drive.png";
        }

        public override void UpdateSize()
        {
            IsReady = _driveInfo.IsReady;
            if (IsReady)
            {
                Size = "Free Size: " + FormatSize(_driveInfo.TotalFreeSpace * 8);
                TotalSize = "Total Size: " + FormatSize(_driveInfo.TotalSize * 8);
            }
        }

        public override void UpdateName(string newPath)
        {
            Path = newPath;
            Name = _driveInfo.Name;
        }
    }
}

