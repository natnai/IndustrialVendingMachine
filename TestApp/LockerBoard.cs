using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_LockerBoard
{
    class LockerBoard
    {
        SerialPort serial = null;

        List<byte> byteReceived = new List<byte>();
        string dataReceived = "";
        DateTime lastDataReceived = DateTime.MinValue;
        Thread thread = null;

        public LockerBoard(string portName)
        {
            serial = new SerialPort(portName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 1000,
                WriteTimeout = 1000,
                ReceivedBytesThreshold = 1
            };
            serial.DataReceived += DataReceivedHandler;

            this.thread = new Thread(LogicLoop);
            this.thread.Start();
        }

        public void OpenDoor(byte boardAddr, byte lockerAddr)
        {
            var buffer = new List<byte>() { 0x8A, boardAddr, lockerAddr, 0x11 };
            buffer.Add(this.CalculateChecksum(buffer.ToArray()));

            this.SendData(buffer.ToArray());
        }

        public byte[] ReadDoors(byte boardAddr)
        {
            //clear buffer
            try { this.serial.ReadExisting(); } catch { } 

            var buffer = new List<byte>() { 0x80, boardAddr, 0x00, 0x33 };
            buffer.Add(this.CalculateChecksum(buffer.ToArray()));

            this.SendData(buffer.ToArray());

            //Task.Delay(500).Wait();

            return this.ReadData();
        }

        private void SendData(byte[] buffer)
        {
            if (this.serial.IsOpen == false) { this.serial.Open(); }

            serial.Write(buffer, 0, buffer.Length);
        }

        private byte[] ReadData()
        {
            /*
            if (this.serial.IsOpen == false) { this.serial.Open(); }

            bool done = false;
            string res = "";
            while (!done)
            {
                try
                {
                    var data = serial.ReadExisting();
                    if (data != "")
                    {
                        res = res + data;
                        Task.Delay(500).Wait();
                    }
                    else
                    {
                        done = true;
                    }
                }
                catch { done = true; }
            }
            */
            var res = "";
            return Encoding.ASCII.GetBytes(res);
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            var result = new List<byte>();
            var i = sp.ReadByte();
            while(i >=0 )
            {
                result.Add(Convert.ToByte(i));

                try { i = sp.ReadByte(); } catch { i = -1; }
            }

            if (result.Count > 0)
            {
                this.byteReceived.AddRange(result);
                this.lastDataReceived = DateTime.Now;
            }

            /*
            string indata = sp.ReadExisting();

            if (indata != "")
            {
                this.dataReceived = this.dataReceived + indata;
                this.lastDataReceived = DateTime.Now;
            }
            */
        }

        private void LogicLoop()
        {
            while (true)
            {
                /*
                if (this.dataReceived != "" && (DateTime.Now - this.lastDataReceived).TotalSeconds >= 1)
                {
                    Console.WriteLine();
                    Console.Write("Data Received: ");
                    var buffer = Encoding.ASCII.GetBytes(this.dataReceived);
                    foreach (var b in buffer) { Console.Write("{0:X2} ", b); }
                    Console.WriteLine();

                    this.dataReceived = "";
                }
                */

                if (this.byteReceived.Count > 0 && (DateTime.Now - this.lastDataReceived).TotalSeconds >= 1)
                {
                    Console.WriteLine();
                    Console.Write("Data Received: ");
                    foreach (var b in this.byteReceived) { Console.Write("{0:X2} ", b); }
                    Console.WriteLine();

                    this.byteReceived.Clear();
                }

                Task.Delay(100).Wait();
            }
        }

        private byte CalculateChecksum(byte[] buffer)
        {
            Byte result = 0x00;
            for (int i = 0; i < buffer.Length; i++)
                result ^= buffer[i];
            return result;
        }
    }
}
