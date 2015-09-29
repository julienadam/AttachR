using System.Linq;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public static class KeyCombinationConverter
    {
        public static string CombinationToString(KeyCombination combination)
        {
            if (combination.Keys == null || combination.Keys.Length == 0)
            {
                return "";
            }
            else
            {
                return string.Join(" + ", combination.Keys.Select(TranslateKey));
            }
        }

        private static string TranslateKey(Key input)
        {
            switch (input)
            {
                case Key.LWin:
                case Key.RWin:
                    return "WIN";
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return "CTRL";
                case Key.LeftAlt:
                case Key.RightAlt:
                    return "ALT";
                case Key.LeftShift:
                case Key.RightShift:
                    return "SHIFT";
                default:
                    return input.ToString();
            }
        }

        public static KeyCombination ParseShortcut(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new KeyCombination();
            }

            return new KeyCombination(
                input
                    .Split('+')
                    .Select(x => ParseSingleKey(x.Trim()))
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToArray());
        }


        private static Key? ParseSingleKey(string key)
        {
            var converter = new KeyConverter();
            try
            {
                return (Key?)converter.ConvertFrom(key);
            }
            catch
            {
                return null;
            }
        }
    }
}
