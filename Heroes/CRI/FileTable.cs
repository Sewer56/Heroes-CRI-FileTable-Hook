using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Process.Functions.X86Functions;

namespace Reloaded_Mod_Template.Heroes.CRI
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FileTable
    {
        public bool IsEnabled;
        public int SixIfInvalidFileHandleOrBeyondMaxListSize;
        public int OneIfInvalidFileHandleOrBeyondMaxListSize;
        public int field_C;
        public int field_10;
        public int field_14;
        public char* AlsoDvdrootFullPath;
        public int MaxFileEntryArraySizeInBytes;
        public int FileEntryArraySizeInBytes;
        public char* DvdrootFullPath;
        public int DvdrootFullPathStringLength;
        public int field_2C;
        public int FileEntryCount;
        public int FirstFileEntryPtr;
        public int CurrentFileEntryIteratorPtr;
        public int field_3C;
        public int field_40;

        /// <summary>
        /// 006CFF40
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int AddFileToFileTable(NewFileInfoTuple* filePath, FileTable* fileTable);

        /// <summary>
        /// 006D63B0
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int BuildFileTable(string folderPath, int decrementsOnNewDirectory, int* a3);

        /// <summary>
        /// 006D6330
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int CallsBuildFileTable(char* a1, int a2, int* a3);

        /// <summary>
        /// 006B4040
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate char* GetDvdrootFullPath(FileTable* fileTable);

        /// <summary>
        /// 006CEFE0
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int GetFileEntryCount(FileTable* fileTable);

        /// <summary>
        /// 006D00F0
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileEntry* GetFileEntryFromFilePath(string fullFilePath);

        /// <summary>
        /// 006D01D0
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileEntry* GetNextFileEntry(FileTable* fileTable);

        /// <summary>
        /// 006D00D0
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileEntry* LoadADXFromFileTable(string fullFilePath);

        /// <summary>
        /// 006CFBA0
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate FileTable* MaybeConstructor(string directoryPath, int a2NormallyMinusOne, char* a3NormallyZero, int a4NormallyZero);

        /// <summary>
        /// 006CF9D0
        /// </summary>
        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int ProbablyResetFileTable();
    }
}
