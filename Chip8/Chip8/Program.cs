using System;
using System.Collections.Generic;
using System.IO;

namespace Chip8
{
    class Program
    {
        static void Main(string[] args)
        {
            CPU cpu = new CPU();

            using (BinaryReader reader = new BinaryReader(new FileStream("IBM Logo.ch8", FileMode.Open)))
            {
                while(reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var opcode = (ushort)((reader.ReadByte() << 8) | reader.ReadByte());

                    try
                    {
                        cpu.ExecuteOpCode(opcode);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                   // 
                   // Console.WriteLine($"{opcode.ToString("X4")}");
                }
             }

            Console.ReadKey();

        }
    }

    public class CPU
    {
        public byte[] RAM = new byte[4096];
        public byte[] V = new byte[16]; //registers
        public ushort I = 0;            //instructions
        //public ushort[] Stack = new ushort[24];
        public Stack<ushort> Stack = new Stack<ushort>();
        
        public byte DelayTimer;
        public byte SecondTimer;
        public byte Keyboard;

        public byte[] Display = new byte[64 * 32];

        

        public void ExecuteOpCode(ushort opcode)
        {
            ushort nibble = (ushort)(opcode & 0xF000);

            switch (nibble)
            {
                case 0x0000:
                    if (opcode == 0x00e0)
                    {
                        for (int i = 0; i < Display.Length; i++) Display[0] = 0;
                    }
                    else if (opcode == 0x00ee)
                    {
                        I = Stack.Pop();
                    }
                    else
                    {
                        throw new Exception($"Unsuported opcode {opcode.ToString("X4")}");
                    }
                    break;
                case 0x1000:
                    I = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x2000:
                    Stack.Push(I);
                    I = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x3000:
                    if (V[(ushort)(opcode & 0x0F00) >> 8] == (opcode & 0x00FF))  I += 2;
                    break;
                default:
                case 0x4000:
                    if (V[(ushort)(opcode & 0x0F00) >> 8] != (opcode & 0x00FF)) I += 2;
                    break;
                    throw new Exception($"Unsuported opcode {opcode.ToString("X4")}");
                    break;
            }
        }
    
    }

}
