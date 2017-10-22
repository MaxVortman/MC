using System;
using System.Collections.ObjectModel;
using System.IO;
using MC.Abstract_and_Parent_Classes;
using MC.Classes.Drives;

namespace MC.Classes.Fillers
{
    static class DriveFiller
    {
        public static ObservableCollection<Drive> FillTheListBoxWithDrives(DriveInfo[] drives)
        {
            var drivesElem = new ObservableCollection<Drive>();
            foreach (var info in drives)
            {
                switch (info.DriveType.ToString())
                {
                    case "Fixed":
                        drivesElem.Add(new Fixed(info));
                        break;
                    case "CDRom":
                        drivesElem.Add(new CdRom(info));
                        break;
                    case "Network":
                        throw new NotImplementedException();
                    case "NoRootDirectory":
                        throw new NotImplementedException();
                    case "Ram":
                        throw new NotImplementedException();
                    case "Removable":
                        drivesElem.Add(new Removable(info));
                        break;
                    case "Unknown":
                        throw new NotImplementedException();
                }
            }
            return drivesElem;
        }
    }
}
