using System.Net.Sockets;

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

           // Saves nick Client side to use as indentifier for the msg
           string nick = Console.ReadLine();
           w.Write(nick); // Nick

           string msg = null;
           while (msg!="q")
            {
                // Writes data continueously from chat server to client
                Task chatStreamUpdate = Task.Run(() =>
                        {
                                while (s.DataAvailable)
                                Console.WriteLine(r.ReadString()); // response
                                Thread.Sleep(1000);
                        });

                // Writes data from client to server
                Task chatStreamWriter = Task.Run(() =>
                        {   
                                msg = Console.ReadLine();
                                w.Write($"{nick}: {msg}"); // request
                        });
            }
            // Default message for server notifying other clients that client has left the server
            w.Write($"*** {nick} has left the chat ***");

            tcpclient.Close();
       }
   }
