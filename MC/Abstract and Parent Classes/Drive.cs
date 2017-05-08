using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC
{
    abstract class Drive : List_sElement
    {
        public bool isReady { private set; get; }
        public string TotalSize { private set; get; }
        DriveInfo driveInfo;
        public Drive(DriveInfo driveInfo)
        {
            isReady = driveInfo.IsReady;
            this.driveInfo = driveInfo;
            GetAndSetInfo();
        }

        public override void Open()
        {
        }

        protected override void GetAndSetInfo()
        {
            Path = driveInfo.RootDirectory.ToString();
            Name = driveInfo.Name;

            if (isReady)
            {
                Size = "Free Size: " + FormatSize(driveInfo.TotalFreeSpace * 8);
                TotalSize = "Total Size: " + FormatSize(driveInfo.TotalSize * 8);
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

