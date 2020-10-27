using System;
using System.Net;

namespace Letter.Tcp
{
    public class FileHandleEndPoint : EndPoint
    {
        public FileHandleEndPoint(ulong fileHandle, FileHandleType fileHandleType)
        {
            FileHandle = fileHandle;
            FileHandleType = fileHandleType;

            switch (fileHandleType)
            {
                case FileHandleType.Auto:
                case FileHandleType.Tcp:
                case FileHandleType.Pipe:
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public ulong FileHandle { get; }
        public FileHandleType FileHandleType { get; }
    }
}