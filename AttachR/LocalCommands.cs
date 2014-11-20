using System;

namespace AttachR
{
    public class LocalCommands
    {
        private static readonly PreferencesCommand preferencesCommand = new PreferencesCommand();

        public static PreferencesCommand Preferences
        {
            get { return preferencesCommand;  }
        }
    }
}