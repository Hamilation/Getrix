using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Getrix
{
    class vClass
    {
        public string Name;
        public List<Func> funcs;
        public List<string> fields;
        public List<Instance> instanceinfo;
        public vClass(string name)
        {
            Name = name;
            funcs = new List<Func>();
            fields = new List<string>();
            instanceinfo = new List<Instance>();
        }
    }

    class Func
    {
        public string Name;
        public int Position;
        public List<Label> labels;
        public Func(string name)
        {
            Name = name;
            Position = 0;
            labels = new List<Label>();
        }
    }

    class Label
    {
        public string Name;
        public int Position;
        public Label(string name)
        {
            Name = name;
            Position = 0;
        }
    }

    class Var
    {
        public string Name;
        public string baseclass;
        public object Value;
        public Var()
        {
            Name = "";
            baseclass = "";
            Value = "";
        }
    }
}
