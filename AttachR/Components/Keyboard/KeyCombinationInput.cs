using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public class KeyCombinationInput : TextBox
    {
        public KeyCombinationInput()
        {
            IsReadOnly = true;
        }

        public static readonly DependencyProperty KeyCombinationProperty = DependencyProperty.Register("KeyCombination", typeof(KeyCombination), typeof(KeyCombinationInput));

        public KeyCombination KeyCombination
        {
            get { return (KeyCombination)GetValue(KeyCombinationProperty); }
            set
            {
                SetValue(KeyCombinationProperty, value);
                Text = KeyCombinationConverter.CombinationToString(value);
            }
        }

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

            KeyCombination= new KeyCombination { Keys = pressedKeys };
        }
    }
}
