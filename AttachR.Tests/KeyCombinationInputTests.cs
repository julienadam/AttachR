using System.Windows.Input;
using AttachR.Components.Keyboard;
using NFluent;
using Xunit;

namespace AttachR.Tests
{
    public class KeyCombinationValidatorTests
    {
        [Fact]
        public void Control_b_is_valid_combination()
        {
            Check.That(KeyCombinationValidator.IsValidCombination(Key.LeftCtrl, Key.B)).IsTrue();
        }

        [Fact]
        public void Control_a_b_is_not_a_valid_combination()
        {
            Check.That(KeyCombinationValidator.IsValidCombination(Key.LeftCtrl, Key.A, Key.B)).IsFalse();
        }

        [Fact]
        public void Control_shift_alt_win_B_is_valid_combination()
        {
            Check.That(KeyCombinationValidator.IsValidCombination(Key.LeftCtrl, Key.LeftShift, Key.LeftAlt, Key.LWin, Key.B)).IsTrue();
        }

        [Fact]
        public void A_b_is_not_a_valid_combination()
        {
            Check.That(KeyCombinationValidator.IsValidCombination(Key.A, Key.B)).IsFalse();
        }

        [Fact]
        public void A_alone_is_not_a_valid_combination()
        {
            Check.That(KeyCombinationValidator.IsValidCombination(Key.A)).IsFalse();
        }

        [Fact]
        public void Control_alone_is_not_a_valid_combination()
        {
            Check.That(KeyCombinationValidator.IsValidCombination(Key.LeftCtrl)).IsFalse();
        }

        [Fact]
        public void Control_shift_is_not_a_valid_combination()
        {
            Check.That(KeyCombinationValidator.IsValidCombination(Key.LeftCtrl, Key.LeftShift)).IsFalse();
        }
        
    }
}