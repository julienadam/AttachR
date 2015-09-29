using System.Windows.Input;
using AttachR.Components.Keyboard;
using NFluent;
using Xunit;

namespace AttachR.Tests
{
    public class KeyCombinationConvertersTests
    {
        [Fact]
        public void Can_parse_null()
        {
            Check.That(KeyCombinationConverter.ParseShortcut(null).Keys).IsEmpty();
        }

        [Fact]
        public void Can_parse_empty()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("").Keys).IsEmpty();
        }

        [Fact]
        public void Can_parse_spaces()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("  ").Keys).IsEmpty();
        }

        [Fact]
        public void Can_parse_single_key()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("A").Keys).ContainsExactly(Key.A);
        }

        [Fact]
        public void Can_parse_single_key_symbol()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("Control").Keys).ContainsExactly(Key.LeftCtrl);
        }

        [Fact]
        public void Can_parse_multikey()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("Control + I").Keys).ContainsExactly(Key.LeftCtrl, Key.I);
        }

        [Fact]
        public void Can_parse_multikey_control_alt()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("Control + Alt + I").Keys).ContainsExactly(Key.LeftCtrl, Key.LeftAlt, Key.I);
        }

        [Fact]
        public void Can_parse_multikey_windows()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("Windows + I").Keys).ContainsExactly(Key.LWin, Key.I);
        }

        [Fact]
        public void Can_parse_multikey_windows_play()
        {
            Check.That(KeyCombinationConverter.ParseShortcut("Windows + MediaPlayPause").Keys).ContainsExactly(Key.LWin, Key.MediaPlayPause);
        }

        [Fact]
        public void Can_display_windows_play()
        {
            Check.That(new KeyCombination(Key.LWin, Key.MediaPlayPause).ToString()).IsEqualTo("WIN + MediaPlayPause");
        }
    }
}
