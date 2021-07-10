using Network;
using Network.Enums;
using Network.Extensions;
using System;
using Chat.Packets;

namespace Chat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server start");
            new Server().Demo();
            
            Console.ReadLine();
        }

        public class Server
        {
            private ServerConnectionContainer serverConnectionContainer;
            
            public void Demo()
            {
                //1. Start listen on a port
                serverConnectionContainer = ConnectionFactory.CreateServerConnectionContainer(1234, false);

                //2. Apply optional settings.
                #region Optional settings

                serverConnectionContainer.ConnectionLost += (a, b, c) => Console.WriteLine($"{serverConnectionContainer.Count} {b.ToString()} Connection lost {a.IPRemoteEndPoint.Port}. Reason {c.ToString()}");
                serverConnectionContainer.ConnectionEstablished += connectionEstablished;
#if NET46
            serverConnectionContainer.AllowBluetoothConnections = true;
#endif
                serverConnectionContainer.AllowUDPConnections = true;
                serverConnectionContainer.UDPConnectionLimit = 2;

                #endregion Optional settings

                //Call start here, because we had to enable the bluetooth property at first.
                serverConnectionContainer.Start();

                //Don't close the application.
                Console.ReadLine();
            }

            private void connectionEstablished(Connection connection, ConnectionType type)
            {
                Console.WriteLine($"{serverConnectionContainer.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

                //3. Register packet listeners.
                connection.RegisterRawDataHandler("RawMessage", RawMessageReceived);
            }
            //Kalau dapat paket, Balas ke client
            private static void RawMessageReceived(Network.Packets.RawData rawData, Connection con)
            {
                Console.WriteLine($"RawMessage received. Data: {rawData.ToUTF8String()}");
                con.SendRawData(Network.Converter.RawDataConverter.FromUTF8String("RawResponse", $"Pesan \"{rawData.ToUTF8String()}\" diterima"));
            }
        }
    }
}
