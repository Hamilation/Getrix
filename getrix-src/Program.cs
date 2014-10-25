using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Getrix
{
    class Program
    {
        public static List<vClass> classes;
        public static Buffer memory;
        public static List<Var> vars;

        static void Main(string[] args)
        {
            classes = new List<vClass>();
            memory = new Buffer();
            vars = new List<Var>();

            StreamReader sr = new StreamReader(args[0]);
            string code = sr.ReadToEnd();
            Scanner scanner = new Scanner(code);
            VirtualMachine vm = new VirtualMachine();
        }
    }
}
