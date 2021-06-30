﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using Network.Enums;
using Network.Converter;
using Chat.Packets;

namespace Chat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client start");

            MessageOwner messageOwner = new MessageOwner("Zaky");
            Message message = new Message(messageOwner);
            message.SetTarget("127.0.0.1", 1234);
            message.SetMessage("Pesan pertama");
            message.SetMessage("Pesan kedua"); 
            message.DoTheThing();

            Console.ReadLine();
        }

        public class Message
        {
            string lastMessage = "";
            string ipAddress; int port = -1;
            MessageOwner messageOwner;
            Connection connection;
            ClientConnectionContainer container;
            public Message(MessageOwner owner)
            {
                messageOwner = owner;
            }
            public void SetTarget(string ipAddress, int port)
            {
                this.ipAddress = ipAddress;
                this.port = port;
                
            }
            public void SetMessage(string message)
            {
                lastMessage = message;
            }

            public void DoTheThing()
            {
                container = ConnectionFactory.CreateClientConnectionContainer(ipAddress, port);
                container.ConnectionEstablished += connectionEstablished;
            }

            private void connectionEstablished(Connection connection, ConnectionType type)
            {
                this.connection = connection;
                Console.WriteLine($"{type.ToString()} Connection established");
                //3. Register what happens if we receive a packet of type "CalculationResponse"
                container.RegisterPacketHandler<MessageResponse>(messageResponseReceived, this);
                //4. Send a calculation request.
                connection.Send(new MessageRequest(messageOwner, lastMessage), this);
            }

            private void messageResponseReceived(MessageResponse response, Connection connection)
            {
                //5. Response received.
                Console.WriteLine($"Answer received {response.response}");
            }
        }



        /// <summary>
        /// Simple example>
        /// 1. Establish a connection
        /// 2. Subscribe connectionEstablished event
        /// 3. Send a raw data packet with the helper classes
        /// 4. Send a raw data packet without the helper classes
        /// </summary>
        public class RawDataExample
        {
            public void Demo()
            {
                ConnectionResult connectionResult = ConnectionResult.TCPConnectionNotAlive;
                //1. Establish a connection to the server.
                TcpConnection tcpConnection = ConnectionFactory.CreateTcpConnection("127.0.0.1", 1234, out connectionResult);
                //2. Register what happens if we get a connection
                if (connectionResult == ConnectionResult.Connected)
                {
                    Console.WriteLine($"{tcpConnection.ToString()} Connection established");
                    //3. Send a raw data packet request.
                    tcpConnection.SendRawData(RawDataConverter.FromUTF8String("HelloWorld", "Hello, this is the RawDataExample!"));
                    tcpConnection.SendRawData(RawDataConverter.FromBoolean("BoolValue", true));
                    tcpConnection.SendRawData(RawDataConverter.FromBoolean("BoolValue", false));
                    tcpConnection.SendRawData(RawDataConverter.FromDouble("DoubleValue", 32.99311325d));
                    //4. Send a raw data packet request without any helper class
                    tcpConnection.SendRawData("HelloWorld", Encoding.UTF8.GetBytes("Hello, this is the RawDataExample!"));
                    tcpConnection.SendRawData("BoolValue", BitConverter.GetBytes(true));
                    tcpConnection.SendRawData("BoolValue", BitConverter.GetBytes(false));
                    tcpConnection.SendRawData("DoubleValue", BitConverter.GetBytes(32.99311325d));
                }
            }
        }
    }
}
