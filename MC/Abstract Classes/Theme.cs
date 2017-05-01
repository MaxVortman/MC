using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MC
{
    [Serializable]
    public abstract class Theme
    {

        [NonSerialized]
        private BrushConverter colorConverter = new BrushConverter();

        [NonSerialized]
        private Brush BackgroundColor;
        public Brush BackColor
        {
            get { return BackgroundColor; }
            protected set
            {
                BackgroundColor = value;
            }
        }
        [NonSerialized]
        private Brush[] ListViewItemsColor;
        public Brush[] LVColor
        {
            get { return ListViewItemsColor; }
            protected set
            {
                ListViewItemsColor = value;
            }
        }        

        public string FolderIconPath { get; protected set; }
        public string DriveIconPath { get; protected set; }
        public string USBIconPath { get; protected set; }
        public string CDRomIconPath { get; protected set; }


        private string _bc;
        private string[] _lvic;

        [OnSerializing]
        private void SetValuesOnSerializing(StreamingContext context)
        {
            _bc = colorConverter.ConvertToString(BackgroundColor);
            _lvic = new string[ListViewItemsColor.Length];
            for (int i = 0; i < ListViewItemsColor.Length; i++)
            {
                _lvic[i] = colorConverter.ConvertToString(ListViewItemsColor[i]);
            }            
        }

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            colorConverter = new BrushConverter();
            BackgroundColor = (Brush)colorConverter.ConvertFrom(_bc);
            ListViewItemsColor = new Brush[_lvic.Length];
            for (int i = 0; i < _lvic.Length; i++)
            {
                ListViewItemsColor[i] = (Brush)colorConverter.ConvertFrom(_lvic[i]);
            }
        }

        public static Theme ThemeSelection(string themeName)
        {
            switch (themeName)
            {
                case "Blue":
                    return new BlueTheme();
                case "Dark":
                    return new DarkTheme();
                default:
                    throw new NotImplementedException("There is no such topic.");
            }
        }
    }
}
