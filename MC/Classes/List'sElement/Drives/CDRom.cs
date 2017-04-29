using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC
{
    class CDRom : Drive
    {
        public CDRom(DriveInfo driveInfo) : base(driveInfo)
        {
            Image = @"C:\Users\Максим Борисович\Documents\Visual Studio 2015\Projects\MC\MC\Images\Icons\Cd-ROM-icon.png";
        }
    }
}
