using System;
using System.IO;

namespace TLib.IO
{
    public class UsbDiskEnterEventArgs : EventArgs
    {
        public UsbDiskEnterEventArgs(DriveInfo drive) { Drive = drive; }
        public DriveInfo Drive { get; }
    }
}
