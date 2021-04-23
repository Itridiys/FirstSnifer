using System;
using SharpPcap;
using SharpPcap.WinPcap;
using PacketDotNet;
using System.Net.NetworkInformation;
using System.Text;

namespace FirstSnifer
{
    class Program
    {
        static void Main(string[] args)
        {
            string hi = "TNX";
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap ver = {0}, Example1.IfList.cs {1}", ver, hi);

            CaptureDeviceList devices = CaptureDeviceList.Instance;

            if (devices.Count < 1)
            {
                Console.WriteLine("No divaces were found on this machine");
                return;
            }
            Console.WriteLine("\nThe following devices are avalibale: ");
            Console.WriteLine("-------------------------------------\n");

            for (int i = 0; i < devices.Count; i++)
            {
                Console.WriteLine("{0} .{1}\n ", i, devices[i].ToString());
            }

            Console.WriteLine("Choose Divace");

            int num = 0;
            if ((!Int32.TryParse(Console.ReadLine(), out num)) || (num > devices.Count))  ///Проверка на ввод числа и кол-ва доступных девайсов
            {
                Console.WriteLine("Incorrect device number");
                Console.ReadLine();
                return;
            }

            ICaptureDevice device = devices[num];

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival); ///device_OnPacketArrival метод на обработчик событий
           

            device.StartCapture(); // Начинаем процедуру захвата пакетов

            Console.ReadLine();

            device.StopCapture(); /// заканчиваем захват пакетов

            Console.Read();


            Console.ReadKey();




        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {

            var MyPacket = PacketDotNet.Packet.ParsePacket(packet.Packet.LinkLayerType, packet.Packet.Data);

            if (MyPacket is PacketDotNet.EthernetPacket)
            {
                //var eth = ((PacketDotNet.EthernetPacket)MyPacket);
                //Console.WriteLine("Original Eth packet: " + eth.ToString() + "\n");

                ////Manipulate ethernet parameters
                //eth.SourceHardwareAddress = PhysicalAddress.Parse("00-11-22-33-44-55");
                //eth.DestinationHardwareAddress = PhysicalAddress.Parse("00-99-88-77-66-55");

                //Console.WriteLine("Changed Eth packet: " + eth.ToString() + "\n");

                //Console.WriteLine("--------------------------------------------");

                var ip = MyPacket.Extract<IPPacket>(); // GetEncapsulated
                if (ip != null)
                {
                    //Console.WriteLine("Original IP packet: " + ip.ToString() + "\n");

                    ////manipulate IP parameters
                    //ip.SourceAddress = System.Net.IPAddress.Parse("1.2.3.4");
                    //ip.DestinationAddress = System.Net.IPAddress.Parse("44.33.22.11");
                    //ip.TimeToLive = 11;

                    //Console.WriteLine("Changed IP packet: " + ip.ToString() + "\n");
                    //Console.WriteLine("--------------------------------------------");

                    var tcp = MyPacket.Extract<TcpPacket>();

                    if (tcp != null)
                    {
                        //Console.WriteLine("Original TCP packet: " + tcp.ToString() + "\n" );

                        //manipulate TCP parameters
                        //tcp.SourcePort = 9999;                        
                        //tcp.DestinationPort = 8888;
                        //tcp.Synchronize = !tcp.Synchronize; // Syn = Synchronize
                        //tcp.Finished = !tcp.Finished; // Fin = Finished
                        //tcp.Acknowledgment = !tcp.Acknowledgment; // Ack = Acknowledgment
                        //tcp.WindowSize = 500;
                        //tcp.AcknowledgmentNumber = 800;
                        //tcp.SequenceNumber = 800;

                        //Console.WriteLine("Changed TCP packet: " + tcp.ToString() + "\n");
                        //Console.WriteLine("--------------------------------------------");


                        var hex = BitConverter.ToString(tcp.PayloadData);

                        hex = hex.Replace("-", "");

                        var l = hex.Length;
                        string hex1 = "";

                        if (l > 0)
                        {
                            string sum = hex[l - 2].ToString() + hex[l - 1].ToString();
                            if (sum == "00")
                            {
                                hex1 += hex;
                               // this.Invoke(new Action(datapaket));
                            }
                            hex1 += hex;
                            Console.WriteLine(hex1);
                        }

                        Console.WriteLine("--------------------------------------------");
                    }

                   

                    

                    //var udp = MyPacket.Extract<UdpPacket>();
                    //if (udp != null)
                    //{
                    //    Console.WriteLine("Original UDP packet: " + udp.ToString() + "\n");

                    //    //manipulate UDP parameters
                    //    udp.SourcePort = 9999;
                    //    udp.DestinationPort = 8888;
                        

                    //    Console.WriteLine("Changed UDP packet: " + udp.ToString() + "\n");
                    //}
                }

                //Console.WriteLine("Manipulated Eth packet: " + eth.ToString());

               
            }
        }
    }
}