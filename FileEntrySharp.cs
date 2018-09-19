using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Reloaded.Process.Buffers;
using Reloaded_Mod_Template.Heroes.CRI;

namespace Reloaded_Mod_Template
{
    /// <summary>
    /// The C# version of the FileEntry class, which converts into the native FileEntry class before writing to memory.
    /// </summary>
    public unsafe class FileEntrySharp
    {
        public SafeFileHandle FileHandle;
        public uint           FileSize;
        public int            Gap8;
        public FileEntry*     NextEntry;
        public string         FileName;

        public FileEntry GetFileEntry(MemoryBuffer memoryBuffer)
        {
            return new FileEntry
            {
                FileHandle = (int)  FileHandle.DangerousGetHandle(),
                FileName   = (char*)memoryBuffer.Add(Encoding.ASCII.GetBytes(FileName)),
                FileSize   = FileSize,
                Gap8       = Gap8,
                NextEntry  = NextEntry
            };
        }
    }
}
