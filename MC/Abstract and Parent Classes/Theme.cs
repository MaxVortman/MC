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
    public class Theme
    {
        public string Name { get; set; }

        [NonSerialized]
        private BrushConverter colorConverter = new BrushConverter();

        [NonSerialized]
        private Brush BackgroundColor;
        public string BackColorString
        {            
            set
            {
                BackgroundColor = (Brush)colorConverter.ConvertFrom(value);
            }
        }
        public Brush BackColor
        {
            get { return BackgroundColor; }
            protected set { BackgroundColor = value; }
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

        public string[] LVColorString
        {            
            set
            {
                colorConverter = new BrushConverter();
                ListViewItemsColor = new Brush[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    ListViewItemsColor[i] = (Brush)colorConverter.ConvertFrom(value[i]); 
                }
            }
        }

        public string FolderIconPath { get;  set; }
        public string DriveIconPath { get;  set; }
        public string USBIconPath { get;  set; }
        public string CDRomIconPath { get;  set; }


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

        public override string ToString()
        {
            return Name;
        }
    }
}
