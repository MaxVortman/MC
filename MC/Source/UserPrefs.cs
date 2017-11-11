using System;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;
using MC.Source.Graphics.Themes;

namespace MC.Source
{
    [Serializable]
    public class UserPrefs
    {
        [NonSerialized]
        private FontFamily _font;
        public FontFamily FontFamily
        {
            get => _font;
            set => _font = value;
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
            _f = _font.ToString();
        }

        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            //decoding
            Password = Decode(Password, _keyP);
            //
            _font = new FontFamily(_f);
        }

        private static string Encode(string pText, string pKey)
        {
            var strEncode = Encoding.UTF8;
            var txt = strEncode.GetBytes(pText);
            var key = strEncode.GetBytes(pKey);
            var res = new byte[pText.Length];
            for (int i = 0; i < txt.Length; i++)
            {
                res[i] = (byte)(txt[i] ^ key[i % key.Length]);
            }
            return strEncode.GetString(res);
        }

        private static string Decode(string pText, string pKey)
        {
            var strEncode = Encoding.UTF8;
            var txt = strEncode.GetBytes(pText);
            var res = new byte[txt.Length];
            var key = strEncode.GetBytes(pKey);
            for (int i = 0; i < txt.Length; i++)
            {
                res[i] = (byte)(txt[i] ^ key[i % key.Length]);
            }
            return strEncode.GetString(res);
        }
    }
}
