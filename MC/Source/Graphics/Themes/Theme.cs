using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace MC.Source.Graphics.Themes
{
    [Serializable]
    public class Theme
    {
        public string Name { get; set; }

        [NonSerialized]
        private BrushConverter _colorConverter = new BrushConverter();

        [NonSerialized]
        private Brush _backgroundColor;
        public string BackColorString
        {            
            set => _backgroundColor = (Brush)_colorConverter.ConvertFrom(value);
        }
        public Brush BackColor
        {
            get => _backgroundColor;
            protected set => _backgroundColor = value;
        }
        [NonSerialized]
        private Brush[] _listViewItemsColor;
        public Brush[] LvColor
        {
            get => _listViewItemsColor;
            protected set => _listViewItemsColor = value;
        }

        public string[] LvColorString
        {            
            set
            {
                _colorConverter = new BrushConverter();
                _listViewItemsColor = new Brush[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    _listViewItemsColor[i] = (Brush)_colorConverter.ConvertFrom(value[i]); 
                }
            }
        }

        public string FolderIconPath { get;  set; }
        public string DriveIconPath { get;  set; }
        public string UsbIconPath { get;  set; }
        public string CdRomIconPath { get;  set; }


        private string _bc;
        private string[] _lvic;

        [OnSerializing]
        private void SetValuesOnSerializing(StreamingContext context)
        {
            _bc = _colorConverter.ConvertToString(_backgroundColor);
            _lvic = new string[_listViewItemsColor.Length];
            for (int i = 0; i < _listViewItemsColor.Length; i++)
            {
                _lvic[i] = _colorConverter.ConvertToString(_listViewItemsColor[i]);
            }            
        }

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            _colorConverter = new BrushConverter();
            _backgroundColor = (Brush)_colorConverter.ConvertFrom(_bc);
            _listViewItemsColor = new Brush[_lvic.Length];
            for (int i = 0; i < _lvic.Length; i++)
            {
                _listViewItemsColor[i] = (Brush)_colorConverter.ConvertFrom(_lvic[i]);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
