using System;
using System.Collections.Generic;

namespace AttachR.ViewModels
{
    public static class DebuggingEngines
    {
        public static List<DebuggingEngine> AvailableModes
        {
            get { return availableModes; }
        }

        // Hard coded for now. It can be retrieved from Visual Studio but it needs a running instance.
        private static readonly List<DebuggingEngine> availableModes = new List<DebuggingEngine>
        {
            new DebuggingEngine(new Guid("FB0D4648-F776-4980-95F8-BB7F36EBC1EE"), "Managed (v4.5, v4.0)"),
            new DebuggingEngine(new Guid("5FFF7536-0C87-462D-8FD2-7971D948E6DC"), "Managed (v3.5, v3.0, v2.0)"),
            new DebuggingEngine(new Guid("449EC4CC-30D2-4032-9256-EE18EB41B62B"), "Managed"),
            new DebuggingEngine(new Guid("92EF0900-2251-11D2-B72E-0000F87572EF"), "Managed/Native"),
            new DebuggingEngine(new Guid("3B476D35-A401-11D2-AAD4-00C04F990171"), "Native"),
            new DebuggingEngine(new Guid("032F4B8C-7045-4B24-ACCF-D08C9DA108FE"), "Silverlight"),
            new DebuggingEngine(new Guid("5AF6F83C-B555-11D1-8418-00C04FA302A1"), "T-SQL 2000"),
            new DebuggingEngine(new Guid("1202F5B4-3522-4149-BAD8-58B2079D704F"), "T-SQl 2005"),
            new DebuggingEngine(new Guid("6589AE11-3B51-494A-AC77-91DA1B53F35A"), "Workflow"),
            new DebuggingEngine(new Guid("F200A7E7-DEA5-11D0-B854-00A0244A1DE2"), "Script"),
            new DebuggingEngine(new Guid("F4453496-1DB8-47F8-A7D5-31EBDDC2EC96"), "GPU - Software Emulator"),
        };
    }
}