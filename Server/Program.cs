using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;

namespace ConsoleApplication1
{
    public class Serv
    {
        public static Dictionary<string, ClientHandler> clientHandlers = new Dictionary<string, ClientHandler>();
        public static void Main()
        {
            try
            {           
                IPAddress ipAd = IPAddress.Parse("127.0.0.1"); //localhost IP address, // 192.168.1.151
                TcpListener myList = new TcpListener(ipAd, 8001); //and use the same at
                myList.Start();                                //the client side
                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
                while (true)
                {
                    string myNick = "test";
                    Console.WriteLine("Waiting for a new connection.....");
                    Socket s = myList.AcceptSocket();
                    ClientHandler ch = new ClientHandler(s);
                    lock (clientHandlers)
                    {
                        NetworkStream socketStream = new NetworkStream(s);
                        BinaryReader reader = new BinaryReader(socketStream);
                        string txt = reader.ReadString(); // get nickname
                        string nick = txt.Substring(0,4);
                        Console.WriteLine("New user has entered the chatroom, nickname: " + nick); 
                        clientHandlers[nick] = ch;                        
                    }
                    Thread t = new Thread(new ThreadStart(ch.HandleClient));
                    t.Start();
                }
                //   myList.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }
    }

    public class ClientHandler
    {
        private Socket s;
        private NetworkStream socketStream;
        public BinaryWriter writer;
        private BinaryReader reader;

        public ClientHandler(Socket s)
        {
            this.s = s;
            socketStream = new NetworkStream(s);
            writer = new BinaryWriter(socketStream);
            reader = new BinaryReader(socketStream);
        }

        public void HandleClient()
        {
            Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);
            string txt = "retur ";
            while (true)
            {
                try
                {
                    txt = reader.ReadString(); // NICK: MSG
                    Console.WriteLine(txt); // echo on server
                    string option = null;
                    string nick = null;
                    string privateMsg = null;
                    lock (Serv.clientHandlers)
                    {
                        if(txt == "q") return; // User leaves the chat returns nothing

                        // The following if statement could be less "hacky", but it works:)
                        else if(txt.Substring(6,1) == "-")
                        {
                            option = txt.Substring(7);

                            if(option.Substring(0,2) == "to")
                            {
                                nick = option.Substring(3,4);
                                privateMsg = option.Substring(8);
                                Serv.clientHandlers[nick].writer.Write($"PRIVATE {nick}: {privateMsg}");
                            }

                            else if(option == "list")
                            {
                                writer.Write("*** List of online users ***\n");
                                foreach (var user in Serv.clientHandlers)
                                {
                                    writer.Write(user.Key);
                                }
                                writer.Write("\n****************************");
                            }
                        }
                        else 
                        {
                            foreach (var pair in Serv.clientHandlers)
                                    pair.Value.writer.Write(txt);
                        }
                    }
                }
                catch (Exception e) { } // dont bring me down
            }
        }
    }
}