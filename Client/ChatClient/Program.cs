// TCP client by JTM
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
   class Program
   {
       public static async Task Main()
       {
           TcpClient tcpclient = new TcpClient();
           tcpclient.Connect("127.0.0.1", 8001);
           NetworkStream s = tcpclient.GetStream();
           BinaryReader r = new BinaryReader(s);
           BinaryWriter w = new BinaryWriter(s);
           Console.Write("Enter nickname: ");
           string nick = Console.ReadLine();
           w.Write(nick); // Nick
           string msg = null;
           while (msg!="q")
            {
                Task t = Task.Run(() =>
                        {
                                while (s.DataAvailable)
                                Console.WriteLine(r.ReadString()); // response
                        });
                    Task c = Task.Run(() =>
                    {   
                            msg = Console.ReadLine();
                            w.Write($"{nick}: {msg}"); // request

                            // while (s.DataAvailable)
                            //     Console.WriteLine(r.ReadString()); // response
                    });
            }
            w.Write($"*** {nick} has left the chat ***");
            tcpclient.Close();
           
           
       }
   }
