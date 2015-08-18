using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace AttachR.Components.Keyboard
{
    public static class KeyboardState
    {
        private static readonly byte[] distinctVirtualKeys = Enumerable
            .Range(0, 256)
            .Select(KeyInterop.KeyFromVirtualKey)
            .Where(item => item != Key.None)
            .Distinct()
            .Select(item => (byte) KeyInterop.VirtualKeyFromKey(item))
            .ToArray();

        /// <summary>
        /// Gets all keys that are currently in the down state.
        /// </summary>
        /// <returns>
        /// A collection of all keys that are currently in the down state.
        /// </returns>
        public static Key[] GetCurrentlyPressedKeys()
        {
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            return distinctVirtualKeys
                .Where(virtualKey => (keyboardState[virtualKey] & 0x80) != 0)
                .Select(virtualKey => KeyInterop.KeyFromVirtualKey(virtualKey))
                .ToArray();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] keyState);
    }
}