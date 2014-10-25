using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Getrix
{
    class Opcodes
    {
        public static readonly byte pushint = 0;
        public static readonly byte pushstring = 1;
        public static readonly byte pushvar = 2;
        public static readonly byte pushfield = 3;
        public static readonly byte pop = 4;
        public static readonly byte popa = 5;
        public static readonly byte print = 6;
        public static readonly byte printl = 7;
        public static readonly byte read = 8;
        public static readonly byte readl = 9;
        public static readonly byte get = 10;
        public static readonly byte getl = 11;
        public static readonly byte defvar = 12;
        public static readonly byte deffield = 13;
        public static readonly byte setvar = 14;
        public static readonly byte setfield = 15;
        public static readonly byte add = 16;
        public static readonly byte sub = 17;
        public static readonly byte div = 18;
        public static readonly byte mul = 19;
        public static readonly byte cmp = 20;
        public static readonly byte call = 21;
        public static readonly byte calls = 22;
        public static readonly byte ce = 23;
        public static readonly byte ces = 24;
        public static readonly byte cn = 25;
        public static readonly byte cns = 26;
        public static readonly byte jmp = 27;
        public static readonly byte je = 28;
        public static readonly byte jn = 29;
        public static readonly byte exit = 30;
    }
}
