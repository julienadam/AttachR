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
    }
}