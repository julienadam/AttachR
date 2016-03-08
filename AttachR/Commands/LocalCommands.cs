namespace AttachR.Commands
{
    public static class LocalCommands
    {
        public static PreferencesCommand PreferencesCommand { get; set; } = new PreferencesCommand();

        public static RunAllCommand RunAllCommand { get; set; } = new RunAllCommand();

        public static DebugAllCommand DebugAllCommand { get; set; } = new DebugAllCommand();

        public static StopAllCommand StopAllCommand { get; set; } = new StopAllCommand();
    }
}