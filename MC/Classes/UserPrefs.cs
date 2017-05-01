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
    public class UserPrefs
    {
        [NonSerialized]
        private FontFamily Font;
        public FontFamily FontFamily
        {
            get { return Font; }
            set
            {
                Font = value;
            }
        }

        public Theme Theme { get; set; }

        private string _f;
        [OnSerializing]
        private void SetValuesOnSerializing(StreamingContext context)
        {
            _f = Font.ToString();
        }

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            Font = new FontFamily(_f);
        }
    }
}
