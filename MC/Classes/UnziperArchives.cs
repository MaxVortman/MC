using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
{
    class UnziperArchives
    {
        public static void UnarchiveElemInThread(Entity item)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.Windows.Forms.DialogResult.OK == folderDialog.ShowDialog())
            {
                item.Unarchive(folderDialog.SelectedPath);
            }
        }
    }
}
