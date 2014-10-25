using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Getrix
{
    class call
    {
        public string Name;
        public string inst;
        public int Position;
        public vClass lastclass;
        public Func lastfunc;
        public call()
        {
            Name = "";
            inst = "";
            Position = 0;
            lastclass = null;
            lastfunc = null;
        }
    }

    class field
    {
        public string Name;
        public object Value;
        public field(string name)
        {
            Name = name;
            Value = "";
        }
    }

    class Instance
    {
        public string Name;
        public List<Func> funcs;
        public List<field> fields;
        public Instance(string name)
        {
            Name = name;
            funcs = new List<Func>();
            fields = new List<field>();
        }
    }

    class VirtualMachine
    {
        static Stack<object> stack;
        static vClass currentclass;
        static Func currentfunc;
        static bool CompareFlag;
        static Stack<call> callstack;

        public VirtualMachine()
        {
            stack = new Stack<object>();
            currentclass = null;
            currentfunc = null;
            CompareFlag = false;
            callstack = new Stack<call>();

            loadClass("Program", "Main");
        }

        static void loadClass(string classname, string funcname)
        {
            foreach (vClass c in Program.classes)
            {
                if (c.Name == classname)
                {
                    foreach (Func f in c.funcs)
                    {
                        if (f.Name == funcname)
                        {
                            Program.memory.pos = f.Position;
                            currentclass = c;
                            currentfunc = f;
                            execute();
                        }
                    }
                }
            }
        }

        static void call(string inst, string name)
        {
            //Console.WriteLine("calling");
            foreach (Instance i in currentclass.instanceinfo)
            {
                if (i.Name == inst)
                {
                    //Console.WriteLine("found instance");
                    foreach (Func f in i.funcs)
                    {
                        if (f.Name == name)
                        {
                            //Console.WriteLine("found func");
                            call c = new call();
                            c.Name = currentclass.Name;
                            c.Position = Program.memory.pos;
                            c.inst = inst;
                            c.lastfunc = currentfunc;
                            c.lastclass = currentclass;
                            callstack.Push(c);
                            currentfunc = f;
                            Program.memory.pos = f.Position;
                        }
                    }
                }
            }
        }

        static void calls(string name)
        {
            if (callstack.Count > 0)
            {
                call call = callstack.Pop();
                callstack.Push(call);
                foreach (Instance i in currentclass.instanceinfo)
                {
                    if (i.Name == call.inst)
                    {
                        foreach (Func f in i.funcs)
                        {
                            if (f.Name == name)
                            {
                                call c = new call();
                                c.Name = call.inst;
                                c.Position = Program.memory.pos;
                                c.inst = call.inst;
                                c.lastfunc = currentfunc;
                                c.lastclass = currentclass;
                                callstack.Push(c);
                                Program.memory.pos = f.Position;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Func f in currentclass.funcs)
                {
                    if (f.Name == name)
                    {
                        call c = new call();
                        c.Name = currentclass.Name;
                        c.Position = Program.memory.pos;
                        c.inst = currentclass.Name;
                        c.lastfunc = currentfunc;
                        callstack.Push(c);
                        currentfunc = f;
                        Program.memory.pos = f.Position;
                    }
                }
            }
        }

        static void jmp(string labelname)
        {
            foreach (Label l in currentfunc.labels)
            {
                if (l.Name == labelname)
                {
                    Program.memory.pos = l.Position;
                }
            }
        }

        static object GetVar(string name)
        {
            object val = "";
            foreach (Var v in Program.vars)
            {
                if (v.baseclass == currentclass.Name)
                {
                    if (v.Name == name)
                    {
                        val = v.Value;
                    }
                }
            }
            return val;
        }

        static object GetField(string inst, string name)
        {
            object val = "";
            foreach (Instance i in currentclass.instanceinfo)
            {
                if (i.Name == inst)
                {
                    foreach (field f in i.fields)
                    {
                        if (f.Name == name)
                        {
                            val = f.Value;
                        }
                    }
                }
            }
            return val;
        }

        static void SetField(string inst, string name, object value)
        {
            foreach (Instance i in currentclass.instanceinfo)
            {
                if (i.Name == inst)
                {
                    foreach (field f in i.fields)
                    {
                        if (f.Name == name)
                        {
                            f.Value = value;
                        }
                    }
                }
            }
        }

        static void SetVar(string name, object value)
        {
            foreach (Var v in Program.vars)
            {
                if (v.baseclass == currentclass.Name)
                {
                    if (v.Name == name)
                    {
                        v.Value = value;
                    }
                }
            }
        }

        static void execute()
        {
            byte opcode = 0;
            while (true)
            {
                try
                {
                    try
                    {
                        opcode = Program.memory.Read();
                    }
                    catch
                    {
                    }

                    if (opcode == Opcodes.pushint)
                    {
                        int dim = Program.memory.ReadInt();
                        stack.Push(dim);
                    }
                    else if (opcode == Opcodes.pushstring)
                    {
                        string data = Program.memory.ReadString();
                        stack.Push(data);
                    }
                    else if (opcode == Opcodes.pushvar)
                    {
                        string name = Program.memory.ReadString();
                        stack.Push(GetVar(name));
                    }
                    else if (opcode == Opcodes.pushfield)
                    {
                        string inst = Program.memory.ReadString();
                        string name = Program.memory.ReadString();
                        if (inst == "self")
                        {
                            call c = callstack.Pop();
                            callstack.Push(c);
                            object val = GetField(c.inst, name);
                        }
                        else
                        {
                            object val = GetField(inst, name);
                            stack.Push(val);
                        }
                    }
                    else if (opcode == Opcodes.pop)
                    {
                        stack.Pop();
                    }
                    else if (opcode == Opcodes.popa)
                    {
                        stack.Clear();
                    }
                    else if (opcode == Opcodes.print)
                    {
                        Console.Write(stack.Pop());
                    }
                    else if (opcode == Opcodes.printl)
                    {
                        Console.WriteLine(stack.Pop());
                    }
                    else if (opcode == Opcodes.read)
                    {
                        Console.Read();
                    }
                    else if (opcode == Opcodes.readl)
                    {
                        Console.ReadLine();
                    }
                    else if (opcode == Opcodes.get)
                    {
                        int data = Console.Read();
                        stack.Push(data);
                    }
                    else if (opcode == Opcodes.getl)
                    {
                        string data = Console.ReadLine();
                        stack.Push(data);
                    }
                    else if (opcode == Opcodes.defvar)
                    {
                        string name = Program.memory.ReadString();
                        Var v = new Var();
                        v.Name = name;
                        v.baseclass = currentclass.Name;
                        Program.vars.Add(v);
                    }
                    else if (opcode == Opcodes.deffield)
                    {
                        //Console.WriteLine("defining field");
                        string name = Program.memory.ReadString();
                        string classname = Program.memory.ReadString();
                        foreach (vClass c in Program.classes)
                        {
                            if (c.Name == classname)
                            {
                                //Console.WriteLine("found classs");
                                Instance i = new Instance(name);
                                i.Name = name;
                                foreach (string f in c.fields)
                                {
                                    field field = new field(f);
                                    i.fields.Add(field);
                                    //Console.WriteLine("added field " + f);
                                }
                                foreach (Func f in c.funcs)
                                {
                                    i.funcs.Add(f);
                                    //Console.WriteLine("added func " + f.Name);
                                }
                                currentclass.instanceinfo.Add(i);
                                //Console.WriteLine("added instance " + i.Name);
                            }
                        }
                    }
                    else if (opcode == Opcodes.setvar)
                    {
                        string name = Program.memory.ReadString();
                        object val = stack.Pop();
                        SetVar(name, val);
                    }
                    else if (opcode == Opcodes.setfield)
                    {
                        string inst = Program.memory.ReadString();
                        string name = Program.memory.ReadString();
                        object val = stack.Pop();
                        if (inst == "self")
                        {
                            call c = callstack.Pop();
                            callstack.Push(c);
                            SetField(c.inst, name, val);
                        }
                        else
                        {
                            SetField(inst, name, val);
                        }
                    }
                    else if (opcode == Opcodes.add)
                    {
                        object dim1 = stack.Pop();
                        object dim2 = stack.Pop();
                        if (dim1 is string && dim2 is string)
                        {
                            string op = dim1.ToString() + dim2.ToString();
                            stack.Push(op);
                        }
                        else if (dim1 is int && dim2 is int)
                        {
                            int op = Int32.Parse(dim1.ToString()) + Int32.Parse(dim2.ToString());
                            stack.Push(op);
                        }
                    }
                    else if (opcode == Opcodes.sub)
                    {
                        int dim1 = Int32.Parse(stack.Pop().ToString());
                        int dim2 = Int32.Parse(stack.Pop().ToString());
                        int op = dim1 - dim2;
                        stack.Push(op);
                    }
                    else if (opcode == Opcodes.mul)
                    {
                        int dim1 = Int32.Parse(stack.Pop().ToString());
                        int dim2 = Int32.Parse(stack.Pop().ToString());
                        int op = dim1 * dim2;
                        stack.Push(op);
                    }
                    else if (opcode == Opcodes.div)
                    {
                        int dim1 = Int32.Parse(stack.Pop().ToString());
                        int dim2 = Int32.Parse(stack.Pop().ToString());
                        int op = dim1 / dim2;
                        stack.Push(op);
                    }
                    else if (opcode == Opcodes.cmp)
                    {
                        string op1 = stack.Pop().ToString();
                        string op2 = stack.Pop().ToString();
                        if (op1 == op2)
                        {
                            CompareFlag = true;
                        }
                    }
                    else if (opcode == Opcodes.call)
                    {
                        //Console.WriteLine("starting call");
                        string inst = Program.memory.ReadString();
                        //Console.WriteLine("instance " + inst);
                        string name = Program.memory.ReadString();
                        //Console.WriteLine("name " + name);
                        call(inst, name);
                    }
                    else if (opcode == Opcodes.calls)
                    {
                        string name = Program.memory.ReadString();
                        calls(name);
                    }
                    else if (opcode == Opcodes.ce)
                    {
                        string inst = Program.memory.ReadString();
                        string name = Program.memory.ReadString();
                        if (CompareFlag)
                        {
                            call(inst, name);
                        }
                    }
                    else if (opcode == Opcodes.ces)
                    {
                        string name = Program.memory.ReadString();
                        if (CompareFlag)
                        {
                            calls(name);
                        }
                    }
                    else if (opcode == Opcodes.cn)
                    {
                        string inst = Program.memory.ReadString();
                        string name = Program.memory.ReadString();
                        if (!CompareFlag)
                        {
                            call(inst, name);
                        }
                    }
                    else if (opcode == Opcodes.cns)
                    {
                        string name = Program.memory.ReadString();
                        if (!CompareFlag)
                        {
                            calls(name);
                        }
                    }
                    else if (opcode == Opcodes.jmp)
                    {
                        string name = Program.memory.ReadString();
                        jmp(name);
                    }
                    else if (opcode == Opcodes.je)
                    {
                        string name = Program.memory.ReadString();
                        if (CompareFlag)
                        {
                            jmp(name);
                        }
                    }
                    else if (opcode == Opcodes.jn)
                    {
                        string name = Program.memory.ReadString();
                        if (!CompareFlag)
                        {
                            jmp(name);
                        }
                    }
                    else if (opcode == Opcodes.exit)
                    {
                        if (callstack.Count > 0)
                        {
                            call c = callstack.Pop();
                            currentfunc = c.lastfunc;
                            currentclass = c.lastclass;
                            Program.memory.pos = c.Position;
                        }
                        else
                        {
                            System.Environment.Exit(0);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}
