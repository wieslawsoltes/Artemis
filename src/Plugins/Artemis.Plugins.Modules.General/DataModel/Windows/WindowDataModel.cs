﻿using System.Diagnostics;
using Artemis.Core.Plugins.Abstract.DataModels.Attributes;
using Artemis.Plugins.Modules.General.Utilities;

namespace Artemis.Plugins.Modules.General.DataModel.Windows
{
    public class WindowDataModel
    {
        [DataModelIgnore]
        public Process Process { get; }

        public WindowDataModel(Process process)
        {
            Process = process;
            WindowTitle = process.MainWindowTitle;
            ProcessName = process.ProcessName;

            // Accessing MainModule requires admin privileges, this way does not
            ProgramLocation = WindowMonitor.GetProcessFilename(process);
        }

        public string WindowTitle { get; set; }
        public string ProcessName { get; set; }
        public string ProgramLocation { get; set; }
    }
}