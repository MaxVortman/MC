using System;
using System.IO;
using Buffer = MC.Classes.Buffer;

namespace MC.Abstract_and_Parent_Classes
{
    internal abstract class Drive : ListSElement
    {
        public bool IsReady { private set; get; }
        public string TotalSize { set; get; }
        private readonly DriveInfo _driveInfo;

        protected Drive(DriveInfo driveInfo)
        {
            IsReady = driveInfo.IsReady;
            _driveInfo = driveInfo;
            GetAndSetInfo();
        }

        public override void Open()
        {
        }

        protected sealed override void GetAndSetInfo()
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

        public override Buffer Copy()
        {
            throw new NotImplementedException();
        }

        public override void Paste(string path, Buffer buffer)
        {
            throw new NotImplementedException();
        }

        public new void Unarchive(string pathZip)
        {
            throw new NotImplementedException();
        }

        public override void UpdateSize()
        {
            throw new NotImplementedException();
        }

        public override void UpdateName(string newPath)
        {
            throw new NotImplementedException();
        }

        public override void Archive(string pathZip)
        {
            throw new NotImplementedException();
        }
    }
}

