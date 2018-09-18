using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Reloaded;
using Reloaded.Assembler;
using Reloaded.Process;
using Reloaded.Process.Buffers;
using Reloaded.Process.Functions.X86Hooking;
using Reloaded.Process.Memory;
using Reloaded_Mod_Template.Heroes.CRI;
using Vanara.PInvoke;

namespace Reloaded_Mod_Template
{
    public static unsafe class Program
    {
        #region Mod Loader Template Description & Explanation | Your first time? Read this.
        /*
         *  Reloaded Mod Loader DLL Modification Template
         *  Sewer56, 2018 ©
         *
         *  -------------------------------------------------------------------------------
         *
         *  Here starts your own mod loader DLL code.
         *
         *  The Init function below is ran at the initialization stage of the game.
         *
         *  The game at this point suspended and frozen in memory. There is no execution
         *  of game code currently ongoing.
         *
         *  This is where you do your hot-patches such as graphics stuff, patching the
         *  window style of the game to borderless, setting up your initial variables, etc.
         *
         *  -------------------------------------------------------------------------------
         *
         *  Important Note:
         *
         *  This function is executed once during startup and SHOULD return as the
         *  mod loader awaits successful completion of the main function.
         *
         *  If you want your mod/code to sit running in the background, please initialize
         *  another thread and run your code in the background on that thread, otherwise
         *  please remember to return from the function.
         *
         *  There is also some extra code, including DLL stubs for Reloaded, classes
         *  to interact with the Mod Loader Server as well as other various loader related
         *  utilities available.
         *
         *  -------------------------------------------------------------------------------
         *  Extra Tip:
         *
         *  For Reloaded mod development, there are also additional libraries and packages
         *  available on NuGet which provide you with extra functionality.
         *
         *  Examples include:
         *  [Input] Reading controller information using Reloaded's input stack.
         *  [IO] Accessing the individual Reloaded config files.
         *  [Overlays] Easy to use D3D and external overlay code.
         *
         *  Simply search libReloaded on NuGet to find those extras and refer to
         *  Reloaded-Mod-Samples subdirectory on Github for examples of using them (and
         *  sample mods showing how Reloaded can be used).
         *
         *  -------------------------------------------------------------------------------
         *
         *  [Template] Brief Walkthrough:
         *
         *  > ReloadedTemplate/Initializer.cs
         *      Stores Reloaded Mod Loader DLL Template/Initialization Code.
         *      You are not required/should not (need) to modify any of the code.
         *
         *  > ReloadedTemplate/Client.cs
         *      Contains various pieces of code to interact with the mod loader server.
         *
         *      For convenience it's recommended you import Client static(ally) into your
         *      classes by doing it as such `Reloaded_Mod_Template.Reloaded_Code.Client`.
         *
         *      This will avoid you typing the full class name and let you simply type
         *      e.g. Print("SomeTextToConsole").
         *
         *  -------------------------------------------------------------------------------
         *
         *  If you like Reloaded, please consider giving a helping hand. This has been
         *  my sole project taking up most of my free time for months. Being the programmer,
         *  artist, tester, quality assurance, alongside various other roles is pretty hard
         *  and time consuming, not to mention that I am doing all of this for free.
         *
         *  Well, alas, see you when Reloaded releases.
         *
         *  Please keep this notice here for future contributors or interested parties.
         *  If it bothers you, consider wrapping it in a #region.
        */
        #endregion Mod Loader Template Description

        #region Template Variables
        /*
            Default Variables:
            These variables are automatically assigned by the mod template, you do not
            need to assign those manually.
        */

        /// <summary>
        /// Holds the game process for us to manipulate.
        /// Allows you to read/write memory, perform pattern scans, etc.
        /// See libReloaded/GameProcess (folder)
        /// </summary>
        public static ReloadedProcess GameProcess;

        /// <summary>
        /// Stores the absolute executable location of the currently executing game or process.
        /// </summary>
        public static string ExecutingGameLocation;

        /// <summary>
        /// Specifies the full directory location that the current mod 
        /// is contained in.
        /// </summary>
        public static string ModDirectory;
        #endregion Template Variables

        private static FunctionHook<FileTable.BuildFileTable> _buildFileTableHook;
        private static FunctionHook<FileTable.GetFileEntryFromFilePath> _getFileEntryFromPathHook;
        private static MemoryBuffer _memoryBuffer;
        private static Dictionary<string, IntPtr> _structMapper;

        /// <summary>
        /// Entry point.
        /// </summary>
        public static unsafe void Init()
        {
            #if DEBUG
            Debugger.Launch();
            #endif

            // 255 bytes estimate per file
            // 10000 files capacity
            IntPtr bufferLocation       = GameProcess.AllocateMemory(2550000);
            _memoryBuffer               = new MemoryBuffer(bufferLocation, 2550000, true);
            _structMapper               = new Dictionary<string, IntPtr>();

            _getFileEntryFromPathHook   = FunctionHook<FileTable.GetFileEntryFromFilePath>.Create(0x006D00F0, GetFileEntryFromPathImpl).Activate();
            _buildFileTableHook         = FunctionHook<FileTable.BuildFileTable>.Create(0x006D63B0, BuildFileTableHookImpl).Activate();
        }

        /// <summary>
        /// Returns 0; we don't want long load times.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="decrementsOnNewDirectory"></param>
        /// <param name="a3"></param>
        /// <returns></returns>
        private static int BuildFileTableHookImpl(string folderPath, int decrementsOnNewDirectory, int* a3)
        {
            return 0;
        }

        /// <summary>
        /// Spoofs the functionality of the original function by opening a handle to a specific file on the spot 
        /// </summary>
        /// <param name="fullFilePath">Full path to the file entry to be obtained.</param>
        /// <returns></returns>
        private static FileEntry* GetFileEntryFromPathImpl(string fullFilePath)
        {
            // If file already exists, return it.
            if (_structMapper.TryGetValue(fullFilePath, out IntPtr result))
            {
                return (FileEntry*) result;
            }

            // Create new file entry if not already exist.
            FileEntry newFileEntry  = new FileEntry();
            newFileEntry.FileName   = (char*)_memoryBuffer.Add(Encoding.ASCII.GetBytes(fullFilePath));
            newFileEntry.FileHandle = (int)Kernel32.CreateFile(fullFilePath, Kernel32.FileAccess.FILE_ALL_ACCESS,
                                                               FileShare.ReadWrite, new SECURITY_ATTRIBUTES(), FileMode.Open,
                                                               FileFlagsAndAttributes.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero).DangerousGetHandle();
            newFileEntry.FileSize   = Kernel32.GetFileSize(new SafeFileHandle((IntPtr)newFileEntry.FileHandle, true), out uint lpFileSizeHigh);
            
            // Set next entry to last mapped value if possible.
            if (_structMapper.Values.Count > 0)
            {
                newFileEntry.NextEntry = (FileEntry*)_structMapper.Values.Last();
            }
            else
            {
                newFileEntry.NextEntry = (FileEntry*)0;
            }
            
            // Append to buffer.
            IntPtr newFileEntryPtr = _memoryBuffer.Add(newFileEntry);
            _structMapper[fullFilePath] = newFileEntryPtr;

            // Return pointer to file entry.
            return (FileEntry*)newFileEntryPtr;
        }
    }
}
