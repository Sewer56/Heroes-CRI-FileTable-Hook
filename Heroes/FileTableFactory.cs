using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Process.Functions.X86Functions;
using Reloaded_Mod_Template.Heroes.CRI;

namespace Reloaded_Mod_Template.Heroes
{
    public unsafe class FileTableFactory
    {
        /// <summary>
        /// 0x006C8000
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileTable* CreateFromDirectory(char* directoryPath, int a2NormallyMinusOne, int a3NormallyZero, int a4NormallyZero);

        /// <summary>
        /// 0x006D6310
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int SetFileTablePointer(IntPtr newFunctionPointer, FileTable* fileTablePointer);

        /// <summary>
        /// 0x006CFE70
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileTable* SetTablePointersAndBuild(FileTable* fileTablePointer);
    }
}
