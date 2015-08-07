using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace IcodeNet
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false)]
    [ComVisible(true)]
    public class ConnectionStringsOptionPage : DialogPage
    {
        [Category("ConnctionStringsOptionPage")]
        [DisplayName("Placeholder")]
        [Description(@"Placeholder to be used to override connection strings- config settings  on the developer machine without touching the web.config")]
        public string[] Directories { get; set; }

    }
}
