using System.Linq;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public static class KeyCombinationValidator
    {
        public static bool IsValidCombination(params Key[] keys)
        {
            return keys.Any(k => KeyCombination.IsModifier(k)) && keys.Count(k => !KeyCombination.IsModifier(k)) == 1;
        }

       
    }
}