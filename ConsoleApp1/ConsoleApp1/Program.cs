using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        const int ECHO_PORT = 8080;
        static TcpClient eClient;
        static StreamReader readerStream;
        static NetworkStream writeStream;
        static string clientName;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter name: ");
            string name = Console.ReadLine();
            Console.WriteLine("Enter connection ip: ");
            string connection = Console.ReadLine();

            try
            {
                eClient = new TcpClient(connection, ECHO_PORT);
                Console.WriteLine("---Logged in---");
                readerStream = new StreamReader(eClient.GetStream());
                writeStream = eClient.GetStream();

                clientName = name;

                string dataToSend;
                dataToSend = name;
                dataToSend += "\r\n";
                byte[] data = Encoding.ASCII.GetBytes(dataToSend);

                writeStream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                while (true)
                {
                    dataToSend = Console.ReadLine();
                    dataToSend += "\r\n";

                    data = Encoding.ASCII.GetBytes(dataToSend);
                    writeStream.Write(data, 0, data.Length);

                    if (dataToSend.IndexOf("QUIT") > -1)
                    {
                        break;
                    }
                }
                eClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }

        static void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    string receivedMessage = readerStream.ReadLine();
                    string[] parts = receivedMessage.Split(':');

                    string senderName = parts[0].Trim();

                    if (senderName != clientName)
                    {
                        Console.WriteLine(receivedMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
