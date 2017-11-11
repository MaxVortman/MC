using MC.Source.Entries;

namespace MC.Source
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
