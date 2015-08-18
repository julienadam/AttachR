using System.Linq;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public static class KeyCombinationValidator
    {
        public static bool IsValidCombination(params Key[] keys)
        {
            return keys.Any(k => IsModifier(k)) && keys.Count(k => !IsModifier(k)) == 1;
        }

        public static bool IsModifier(Key key)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LWin:
                case Key.RWin:
                    return true;
                default:
                    return false;
            }
        }
    }
}