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
        public string Password { get; set; }
        public string Login { get; set; }

        private string _f;
        private string _keyP;
        [OnSerializing]
        private void SetValuesOnSerializing(StreamingContext context)
        {            
            //encoding
            _keyP = Password.Remove(Password.Length / 2);
            Password = Encode(Password, _keyP);
            //
            _f = Font.ToString();
        }

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            //decoding
            Password = Decode(Password, _keyP);
            //
            Font = new FontFamily(_f);
        }

        private static string Encode(string pText, string pKey)
        {
            Encoding strEncode = Encoding.UTF8;
            byte[] txt = strEncode.GetBytes(pText);
            byte[] key = strEncode.GetBytes(pKey);
            byte[] res = new byte[pText.Length];
            for (int i = 0; i < txt.Length; i++)
            {
                res[i] = (byte)(txt[i] ^ key[i % key.Length]);
            }
            return strEncode.GetString(res);
        }

        private static string Decode(string pText, string pKey)
        {
            Encoding strEncode = Encoding.UTF8;
            byte[] txt = strEncode.GetBytes(pText);
            byte[] res = new byte[txt.Length];
            byte[] key = strEncode.GetBytes(pKey);
            for (int i = 0; i < txt.Length; i++)
            {
                res[i] = (byte)(txt[i] ^ key[i % key.Length]);
            }
            return strEncode.GetString(res);
        }
    }
}
