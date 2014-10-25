using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Getrix
{
    class Buffer
    {
        public List<byte> buffer;
        public int pos = 0;
        public Buffer()
        {
            buffer = new List<byte>();
        }

        public byte Read()
        {
            byte val = buffer[pos];
            pos++;
            return val;
        }
        public int ReadInt()
        {
            int val = BitConverter.ToInt32(buffer.ToArray(), pos);
            pos += 4;
            return val;
        }
        public string ReadString()
        {
            byte length = Read();
            string final = "";
            for (int i = 0; i < length; i++)
            {
                final += (char)Read();
            }
            return final;
        }
        public void Write(byte data)
        {
            buffer.Add(data);
        }
        public void Write(int data)
        {
            byte[] size = BitConverter.GetBytes(data);
            buffer.Add(size[0]);
            buffer.Add(size[1]);
            buffer.Add(size[2]);
            try
            {
                buffer.Add(size[3]);
            }
            catch
            {
                buffer.Add((byte)0);
            }
        }
        public void Write(string data)
        {
            Write((byte)data.Length);
            foreach (char c in data)
            {
                buffer.Add((byte)c);
            }
        }
    }
}
