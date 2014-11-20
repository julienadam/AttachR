namespace AttachR.Commands
{
    public static class LocalCommands
    {
        private static PreferencesCommand preferencesCommand = new PreferencesCommand();
        private static RunAllCommand runRunAllCommand = new RunAllCommand();
        private static StopAllCommand stopAllCommand = new StopAllCommand();

        public static PreferencesCommand PreferencesCommand
        {
            get { return preferencesCommand; }
            set { preferencesCommand = value; }
        }

        public static RunAllCommand RunAllCommand
        {
            get { return runRunAllCommand; }
            set { runRunAllCommand = value; }
        }

        public static StopAllCommand StopAllCommand
        {
            get { return stopAllCommand; }
            set { stopAllCommand = value; }
        }
    }
}