using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Getrix
{
    class Scanner
    {
        public Scanner(string code)
        {
            code = code.Replace(((char)13).ToString(), "");
            vClass currentclass = null;
            Func currentfunc = null;
            foreach (string a in code.Split('\n'))
            {
                if (a.StartsWith("class "))
                {
                    string name = a.Substring(6);
                    currentclass = new vClass(name);
                }
                else if (a.StartsWith("func "))
                {
                    string name = a.Substring(5);
                    currentfunc = new Func(name);
                    currentfunc.Position = Program.memory.buffer.Count;
                }
                else if (a.StartsWith("."))
                {
                    string name = a.Substring(1);
                    Label l = new Label(name);
                    l.Position = Program.memory.buffer.Count;
                    currentfunc.labels.Add(l);
                }
                else if (a.StartsWith("push "))
                {
                    object dim = a.Substring(5);
                    if (dim.ToString().StartsWith("'"))
                    {
                        string dim1 = dim.ToString().Substring(1, dim.ToString().LastIndexOf("'") - 1);
                        Program.memory.Write(Opcodes.pushstring);
                        Program.memory.Write(dim1);
                    }
                    else if (dim.ToString() == "read")
                    {
                        Program.memory.Write(Opcodes.get);
                    }
                    else if (dim.ToString() == "readl")
                    {
                        Program.memory.Write(Opcodes.getl);
                    }
                    else if (dim.ToString().StartsWith("$"))
                    {
                        dim = dim.ToString().Substring(1);
                        if (dim.ToString().Contains("."))
                        {
                            string inst = dim.ToString().Substring(0, dim.ToString().IndexOf('.'));
                            string name = dim.ToString().Substring(dim.ToString().IndexOf('.') + 1);
                            Program.memory.Write(Opcodes.pushfield);
                            Program.memory.Write(inst);
                            Program.memory.Write(name);
                        }
                        else
                        {
                            string name = dim.ToString();
                            Program.memory.Write(Opcodes.pushvar);
                            Program.memory.Write(name);
                        }
                    }
                    else
                    {
                        int op = Convert.ToInt32(dim);
                        Program.memory.Write(Opcodes.pushint);
                        Program.memory.Write(op);
                    }
                }
                else if (a == "pop")
                {
                    Program.memory.Write(Opcodes.pop);
                }
                else if (a == "popa")
                {
                    Program.memory.Write(Opcodes.popa);
                }
                else if (a == "print")
                {
                    Program.memory.Write(Opcodes.print);
                }
                else if (a == "printl")
                {
                    Program.memory.Write(Opcodes.printl);
                }
                else if (a == "read")
                {
                    Program.memory.Write(Opcodes.read);
                }
                else if (a == "readl")
                {
                    Program.memory.Write(Opcodes.readl);
                }
                else if (a.StartsWith("def $"))
                {
                    string dim = a.Substring(5);
                    if (dim.Contains(":"))
                    {
                        string name = dim.Substring(0, dim.IndexOf(':'));
                        string classname = dim.Substring(dim.IndexOf(':') + 1);
                        Program.memory.Write(Opcodes.deffield);
                        Program.memory.Write(name);
                        Program.memory.Write(classname);
                    }
                    else
                    {
                        string name = dim;
                        Program.memory.Write(Opcodes.defvar);
                        Program.memory.Write(name);
                    }
                }
                else if (a.StartsWith("set $"))
                {
                    string dim = a.Substring(5);
                    if (dim.Contains("."))
                    {
                        string inst = dim.Substring(0, dim.IndexOf('.'));
                        string name = dim.Substring(dim.IndexOf('.') + 1);
                        Program.memory.Write(Opcodes.setfield);
                        Program.memory.Write(inst);
                        Program.memory.Write(name);
                    }
                    else
                    {
                        string name = dim;
                        Program.memory.Write(Opcodes.setvar);
                        Program.memory.Write(name);
                    }
                }
                else if (a.StartsWith("public $"))
                {
                    string name = a.Substring(8);
                    currentclass.fields.Add(name);
                }
                else if (a == "add")
                {
                    Program.memory.Write(Opcodes.add);
                }
                else if (a == "sub")
                {
                    Program.memory.Write(Opcodes.sub);
                }
                else if (a == "mul")
                {
                    Program.memory.Write(Opcodes.mul);
                }
                else if (a == "div")
                {
                    Program.memory.Write(Opcodes.div);
                }
                else if (a == "cmp")
                {
                    Program.memory.Write(Opcodes.cmp);
                }
                else if (a.StartsWith("call "))
                {
                    string dim = a.Substring(5);
                    if (dim.StartsWith("$"))
                    {
                        dim = dim.Substring(1);
                        string inst = dim.Substring(0, dim.IndexOf('.'));
                        string name = dim.Substring(dim.IndexOf('.') + 1);
                        Program.memory.Write(Opcodes.call);
                        Program.memory.Write(inst);
                        Program.memory.Write(name);
                    }
                    else
                    {
                        string name = dim;
                        Program.memory.Write(Opcodes.calls);
                        Program.memory.Write(name);
                    }
                }
                else if (a.StartsWith("ce "))
                {
                    string dim = a.Substring(3);
                    if (dim.StartsWith("$"))
                    {
                        dim = dim.Substring(1);
                        string inst = dim.Substring(0, dim.IndexOf('.'));
                        string name = dim.Substring(dim.IndexOf('.') + 1);
                        Program.memory.Write(Opcodes.ce);
                        Program.memory.Write(inst);
                        Program.memory.Write(name);
                    }
                    else
                    {
                        string name = dim;
                        Program.memory.Write(Opcodes.ces);
                        Program.memory.Write(name);
                    }
                }
                else if (a.StartsWith("cn "))
                {
                    string dim = a.Substring(3);
                    if (dim.StartsWith("$"))
                    {
                        dim = dim.Substring(1);
                        string inst = dim.Substring(0, dim.IndexOf('.'));
                        string name = dim.Substring(dim.IndexOf('.') + 1);
                        Program.memory.Write(Opcodes.cn);
                        Program.memory.Write(inst);
                        Program.memory.Write(name);
                    }
                    else
                    {
                        string name = dim;
                        Program.memory.Write(Opcodes.cns);
                        Program.memory.Write(name);
                    }
                }
                else if (a.StartsWith("jmp "))
                {
                    string labelname = a.Substring(4);
                    Program.memory.Write(Opcodes.jmp);
                    Program.memory.Write(labelname);
                }
                else if (a.StartsWith("je "))
                {
                    string labelname = a.Substring(3);
                    Program.memory.Write(Opcodes.je);
                    Program.memory.Write(labelname);
                }
                else if (a.StartsWith("jn "))
                {
                    string labelname = a.Substring(3);
                    Program.memory.Write(Opcodes.jn);
                    Program.memory.Write(labelname);
                }
                else if (a == "end func")
                {
                    Program.memory.Write(Opcodes.exit);
                    currentclass.funcs.Add(currentfunc);
                    currentfunc = null;
                }
                else if (a == "end class")
                {
                    Program.classes.Add(currentclass);
                    currentclass = null;
                }
            }
        }
    }
}
