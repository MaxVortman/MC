using MC.Abstract_and_Parent_Classes;
using MC.Classes.Drives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
{
    class DriveFiller
    {
        public static ObservableCollection<ListSElement> FillTheListBoxWithDrives(DriveInfo[] drives)
        {
            var drivesElem = new ObservableCollection<ListSElement>();
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
