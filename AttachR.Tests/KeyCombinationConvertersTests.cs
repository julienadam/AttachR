using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using AttachR.Components.Keyboard;
using NFluent;
using Xunit;

namespace AttachR.Tests
{
    public class KeyCombinationConvertersTests
    {
        [Fact]
        public void Can_parse_single_key()
        {
            Check.That(ParseShortcut("A")).ContainsExactly(Key.A);
        }

        [Fact]
        public void Can_parse_single_key_symbol()
        {
            Check.That(ParseShortcut("Control")).ContainsExactly(Key.LeftCtrl);
        }

        [Fact]
        public void Can_parse_multikey()
        {
            Check.That(ParseShortcut("Control + I")).ContainsExactly(Key.LeftCtrl, Key.I);
        }

        [Fact]
        public void Can_parse_multikey_control_alt()
        {
            Check.That(ParseShortcut("Control + Alt + I")).ContainsExactly(Key.LeftCtrl, Key.LeftAlt, Key.I);
        }

        [Fact]
        public void Can_parse_multikey_windows()
        {
            Check.That(ParseShortcut("Windows + I")).ContainsExactly(Key.LWin, Key.I);
        }

        [Fact]
        public void Can_parse_multikey_windows_play()
        {
            Check.That(ParseShortcut("Windows + MediaPlayPause")).ContainsExactly(Key.LWin, Key.MediaPlayPause);
        }

        [Fact]
        public void Can_display_windows_play()
        {
            Check.That(KeyCombinationConverter.CombinationToString(new[] {Key.LWin, Key.MediaPlayPause})).IsEqualTo("Windows + MediaPlayPause");
        }
        
        private IEnumerable<Key> ParseShortcut(string input)
        {
            return input
                .Split('+')
                .Select(x => ParseSingleKey(x.Trim()))
                .Where(x => x.HasValue)
                .Select(x => x.Value);
        }


        private Key? ParseSingleKey(string key)
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
