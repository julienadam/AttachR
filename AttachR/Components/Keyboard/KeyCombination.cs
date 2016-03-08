using System;
using System.Linq;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public class KeyCombination
    {
        public KeyCombination(params Key[] keys)
        {
            Keys = keys;
        }

        public Key[] Keys { get; set; }

        public override string ToString()
        {
            return KeyCombinationConverter.CombinationToString(this);
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
                case Key.Clear:
                    return true;
                default:
                    return false;
            }
        }
        public static ModifierKeys ToModifier(Key key)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return ModifierKeys.Control;
                case Key.LeftShift:
                case Key.RightShift:
                    return ModifierKeys.Shift;
                case Key.LeftAlt:
                case Key.RightAlt:
                    return ModifierKeys.Alt;
                case Key.LWin:
                case Key.RWin:
                    return ModifierKeys.Windows;
            }

            return ModifierKeys.None;
        }

        public void CombinationToShortcut(out Key key, out ModifierKeys modifiers)
        {
            key = Keys.First(k => !IsModifier(k));
            modifiers =
                Keys.Where(IsModifier)
                    .Select(ToModifier)
                    .Aggregate((keys1, keys2) => keys1 | keys2);
        }
    }
}