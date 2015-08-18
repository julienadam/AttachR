using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public class KeyCombinationConverter
    {
        public static string CombinationToString(IEnumerable<Key> keys)
        {
            return string.Join(" + ", keys.Select(TranslateKey));
        }

        private static string TranslateKey(Key input)
        {
            switch (input)
            {
                case Key.LWin:
                case Key.RWin:
                    return "Windows";
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return "Control";
                case Key.LeftAlt:
                case Key.RightAlt:
                    return "Alt";
                case Key.LeftShift:
                case Key.RightShift:
                    return "Shift";
                default:
                    return input.ToString();
            }
        }
    }
}
