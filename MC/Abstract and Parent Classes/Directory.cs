using MC.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Abstract_and_Parent_Classes
{
    abstract class Directory : Entity
    {
        
        public List<Entity> GetEntry()
        {
            //must be faster
            var dataList = CreateDataList();
            return GetData(dataList);
        }

        public List<Entity> CreateDataList()
        {
            var dataList = new List<Entity>(500);
            // ... folder
            if (Path.Length > 3)
            {
                var parentPath = System.IO.Directory.GetParent(Path).FullName;
                dataList.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            }
            return dataList;
        }

        protected List<Entity> GetData(List<Entity> dataList)
        {
            //enumerate folder's path
            foreach (var item in System.IO.Directory.EnumerateDirectories(Path))
            {
                dataList.Add(new Folder(item));
            }
            //enumerate file's path
            foreach (var item in System.IO.Directory.EnumerateFiles(Path))
            {
                dataList.Add(new File(item));
            }

            return dataList;
        }

        public override Classes.Buffer Copy()
        {

            var dataList = GetData(new List<Entity>(500));
            int count = dataList.Count;
            Classes.Buffer[] buffer = new Classes.Buffer[count];
            int i = 0;
            foreach (Entity elem in dataList)
            {
                if (i < count)
                {
                    buffer[i] = elem.Copy();
                    i++;
                }
            }

            return new FolderBuffer(Name, buffer);
        }

        public override void Paste(string path, Classes.Buffer buffer)
        {

            System.IO.Directory.CreateDirectory(path);

            Classes.Buffer[] filesBuffer = (buffer as FolderBuffer).FoldersBuffer;
            var dataList = GetData(new List<Entity>(500));
            int count = dataList.Count;
            int i = 0;
            foreach (Entity elem in dataList)
            {
                if (i < count)
                {
                    elem.Paste(System.IO.Path.Combine(path, elem.Name), filesBuffer[i]);
                    i++;
                }
            }
        }

        public static bool Exists(string path)
        {
            return System.IO.Directory.Exists(path);
        }
    }
}
