using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reloaded_Mod_Template.Heroes.CRI
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NewFileInfoTuple
    {
        public char* FileName;
        public int NFileSizeLow;
        public int NFileSizeHigh;
    }
}
