namespace AttachR.ViewModels
{
    public class DebuggingTargetViewModelWithSampleData : DebuggingTargetViewModel
    {
        public DebuggingTargetViewModelWithSampleData()
        {
            CommandLineArguments = "sample arguments that are quite long so that it will probably overflow and show that the textbox is indeed multiline even with impossibly long words like Pneumonoultramicroscopicsilicovolcanoconiosis";
            Executable = @"c:\path\to\some\executable.exe";
            DebuggingEngines[0].Selected = true;
            DebuggingEngines[1].Selected = true;
        }
    }
}