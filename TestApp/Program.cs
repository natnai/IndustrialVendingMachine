using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_LockerBoard
{
          //https://th.aliexpress.com/item/33005274059.html?spm=a2g0o.productlist.0.0.30e83f3621w5H0&algo_pvid=366d0efb-327e-4234-883f-41d6cb557bd7&algo_expid=366d0efb-327e-4234-883f-41d6cb557bd7-1&btsid=0b0a556f16218408785565470e33ce&ws_ab_test=searchweb0_0,searchweb201602_,searchweb201603_
    class Program
    {
        static void Main(string[] args)
        {
            byte controllerId = 1;

            Console.Write("Enter COM Port [0-255]: ");
            var portName = "COM" + Console.ReadLine();

            Console.WriteLine("Openning " + portName);
            LockerBoard locker = new LockerBoard(portName);

            ConsoleKeyInfo cmd = new ConsoleKeyInfo();
            while (cmd.Key != ConsoleKey.D0 && cmd.Key != ConsoleKey.NumPad0)
            {
                Console.WriteLine("");
                Console.WriteLine("MENU");
                Console.WriteLine("  [1] Open all doors");
                Console.WriteLine("  [2] Open 1 door");
                Console.WriteLine("  [3] Read status all doors");
                Console.WriteLine("  ---------");
                Console.WriteLine("  [0] Exit");

                Console.Write("Enter command: ");

                cmd = Console.ReadKey();
                Console.WriteLine("");

                try
                {
                    if (cmd.Key == ConsoleKey.D1 || cmd.Key == ConsoleKey.NumPad1)
                    {
                        //locker.OpenDoor(1, 0);
                        //locker.OpenDoor(2, 0);
                        //locker.OpenDoor(3, 0);
                        locker.OpenDoor(4, 0);
                        locker.OpenDoor(5, 0);
                    }
                    else if (cmd.Key == ConsoleKey.D2 || cmd.Key == ConsoleKey.NumPad2)
                    {
                        Console.Write("Enter locker number [1-255]: ");
                        var num = Console.ReadLine();
                        if (byte.TryParse(num, out byte b))
                        {
                            locker.OpenDoor(
                                Convert.ToByte(Math.Ceiling((decimal)b / (decimal)12)),
                                (byte)(b - Convert.ToByte((Math.Floor((decimal)b / (decimal)12)) * 12))
                            );
                        }
                    }
                    else if (cmd.Key == ConsoleKey.D3 || cmd.Key == ConsoleKey.NumPad3)
                    {
                        var bytes = locker.ReadDoors(controllerId);
                        //Console.Write("Response: ");
                        foreach (var b in bytes) { Console.Write("{0:X2} ", b); }

                        Console.WriteLine();
                    }
                }
                catch( Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("EXCEPTION: " + ex.Message);
                    Console.WriteLine();
                }
            }
        }
    }
}
