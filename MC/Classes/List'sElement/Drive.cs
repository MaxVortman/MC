using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC
{
    class Drive : List_sElement
    {
        public string TotalSize { private set; get;}
        DriveInfo driveInfo;
        public Drive(DriveInfo driveInfo)
        {
            this.driveInfo = driveInfo;
            GetAndSetInfo();
        }

        public override bool Open()
        {
            return true;
        }

        protected override void GetAndSetInfo()
        {
            Path = driveInfo.RootDirectory.ToString();
            Name = driveInfo.Name;

            if (driveInfo.IsReady)
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
    }
}
