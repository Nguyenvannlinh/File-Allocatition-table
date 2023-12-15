using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Disk_inforrmation
{
    class File
    {
        static byte[] Readmulsector(string path, byte pos, byte num)
        {
            byte[] data = new byte[512 * num];
            BinaryReader f;
            try
            {
                f = new BinaryReader(new FileStream(path, FileMode.Open));
                f.BaseStream.Position = pos * 512;
            }
            catch
            {
                Console.Error.WriteLine("lỗi đọc");
                return data;
            }
            try
            {
                for (int i = 0; i < 512 * num; i++)
                {
                    data[i] = f.ReadByte();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            f.Close();
            return data;
        }
        static void print(byte[] data)
        {
            Console.WriteLine("Thông tin trên FAT của đĩa mền");
            int step = 3;
            byte i = 3;
            byte[] ptchan = new byte[data.Length / 3];
            byte[] ptle = new byte[data.Length / 3];
            int chanindex = 0;
            int leindex = 0;
            while (step < data.Length)
            {

                ptchan[chanindex++] = (byte)((data[i + 1] & 0x0F) << 8 | data[i]);
                ptle[leindex++] = (byte)((data[i + 2] << 4) | (data[i + 1] >> 4));
                Console.Write($"{(((data[i + 1] & 0x0F) << 8) + data[i]).ToString("X")}\t ");
                Console.Write($"{((data[i + 2] << 4) + ((data[i + 1]) >> 4)).ToString("X")}\t ");
                i += 3;
                step += 3;
            }
            Console.WriteLine();
            Console.WriteLine("Phần tử chẵn:");
            for (int j = 0; j < ptchan.Length; j++)
            {
                Console.Write($"{ptchan[j].ToString("X")}\t ");
            }
            Console.WriteLine("\nPhần tử lẻ:");
            for (int j = 0; j < ptle.Length; j++)
            {
                Console.Write($"{ptle[j].ToString("X")}\t ");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
        static void root(byte[] data)
        {
            Console.WriteLine("Thông tin Root directory");
            Console.WriteLine("Tên file/thư mục \t\t dung lượng");
            int thumuc = 0, file = 0;
            for(int i = 0; i < 224; i++)
            {
                int pos = i * 32;
                int clust = data[pos + 0x1A] + (data[pos + 0x1B] << 8);
                if(clust == 0)
                {
                    continue;
                }
                else
                {
                    for(int j = 0; j < 8; j++)
                    {
                        Console.Write((char)data[pos + j]);
                    }
                    int kt = data[pos + 11] & 0x10;
                    int size = data[pos + 28] + (data[pos + 29] << 8) + (data[pos + 30] << 16) + (data[pos + 31] << 24);
                    if(kt == 0)
                    {
                        Console.Write($"   {(char)data[pos + 8]}{(char)data[pos + 9]}{(char)data[pos + 10]}");
                        Console.Write($"\t\t\t   {size}");
                        file++;
                    }
                    else
                    {
                        Console.Write("  <DIR>");
                        thumuc++;
                    }
                    Console.Write("\n");
                }
            }
            Console.Write($"Có {thumuc} thư mục và {file} tệp tin");
            Console.WriteLine();
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            string path = @"D:\C#\hệ điều hành\Disk Images\floppy.img";
            byte pos = 1;
            while (true)
            {
                try
                {
                    Console.WriteLine("Hiện thị thông tin\n1. FAT\n2. Root directory\n0. Thoát");
                    int n = int.Parse(Console.ReadLine());
                    switch (n)
                    {
                        case 1:
                            print(Readmulsector(path, pos, 9));
                            Console.Clear();
                            break;
                        case 2:
                            pos = 19;
                            root(Readmulsector(path, pos, 14));
                            Console.Clear();
                            break;
                        default:
                            Console.WriteLine("Nhập lại");
                            break;
                    }
                    if (n == 0)
                    {
                        Environment.Exit(0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
        }
    }
}