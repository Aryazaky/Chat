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
            new UnSecureServerExample().Demo();

            Console.ReadLine();
        }

        public class UnSecureServerExample
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

            /// <summary>
            /// We got a connection.
            /// </summary>
            /// <param name="connection">The connection we got. (TCP or UDP)</param>
            private void connectionEstablished(Connection connection, ConnectionType type)
            {
                Console.WriteLine($"{serverConnectionContainer.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

                //3. Register packet listeners.
                connection.RegisterStaticPacketHandler<MessageRequest>(MessageReceived);
                connection.RegisterRawDataHandler("HelloWorld", (rawData, con) => Console.WriteLine($"RawDataPacket received. Data: {rawData.ToUTF8String()}"));
                connection.RegisterRawDataHandler("BoolValue", (rawData, con) => Console.WriteLine($"RawDataPacket received. Data: {rawData.ToBoolean()}"));
                connection.RegisterRawDataHandler("DoubleValue", (rawData, con) => Console.WriteLine($"RawDataPacket received. Data: {rawData.ToDouble()}"));
            }
            //Kalau dapat pate tipe MessageRequest, Balas ke client
            private static void MessageReceived(MessageRequest packet, Connection connection)
            {
                connection.Send(new MessageResponse($"Message Received by the Server: {packet.message}", packet));
            }
        }
    }
}
