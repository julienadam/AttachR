using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public class KeyCombinationInput : TextBox
    {
        public BindingList<Key> Keys { get; } = new BindingList<Key>();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            UpdateKeys();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            UpdateKeys();
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void UpdateKeys()
        {
            var pressedKeys = KeyboardState.GetCurrentlyPressedKeys();
            if (!KeyCombinationValidator.IsValidCombination(pressedKeys))
            {
                return;
            }

            Keys.Clear();
            foreach (var pressedKey in pressedKeys)
            {
                Keys.Add(pressedKey);
            }

            Text = KeyCombinationConverter.CombinationToString(pressedKeys);
        }
    }
}
