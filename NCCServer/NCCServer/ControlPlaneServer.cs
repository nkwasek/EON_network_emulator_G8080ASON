using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;

namespace ControlPlaneServer
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public Socket workSocket = null;
    }

    class ControlPlaneServer
    {
        private Socket serverSocket;
        private IPAddress ServerAddress = IPAddress.Parse("127.0.0.1");
        private const int ServerPort = 1225;
        private ManualResetEvent allDone = new ManualResetEvent(false);

        public static Socket CallingPCC;            // Calling Party Call Controller
        public static Socket CalledPCC;             // Called Party Call Controller
        public static string CallingPCCName;        // Nazwa użytkownika inicjującego nawiązanie połączenia
        public static string CalledPCCName;         // Nazwa użytkownika, do którego połączenie jest nawiązywane
        public static int DemandedCapacity;      // żądana przepustowość na połączeniu

        public ControlPlaneServer() { }

        public void StartServerSocket()
        {
            serverSocket = new Socket(ServerAddress.AddressFamily, SocketType.Stream, ProtocolType.Udp);
            ReturnLog("Created server socket");

            try
            {
                serverSocket.Bind(new IPEndPoint(ServerAddress, ServerPort));
                serverSocket.Listen(100);
                ReturnLog("Waiting for connection");

                while (true)
                {
                    allDone.Reset();
                    serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), serverSocket);
                    allDone.WaitOne();
                }
            }
            catch (Exception) { ReturnLog("Exception: server unable to establish connection"); }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Utworzenie obiektu stanu  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            try
            {
                // Odczyt danych od klienta
                int bytesRead = handler.EndReceive(ar);
                var message = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                var splitedMessage = message.Split(' ');

                ReturnLog("Received message: " + splitedMessage[0] + "\tFrom: " + splitedMessage[splitedMessage.Length-1]);

                if (splitedMessage[0] == "HELLO")
                {
                    Send(handler, "HELLO");
                    
                }

                else if (splitedMessage[0] == "CallRequest") // call request: "CallRequest Natalka Piotr [capacity]"
                {
                    CallingPCC = handler;
                    CallingPCCName = splitedMessage[1];
                    CalledPCCName = splitedMessage[2];
                    DemandedCapacity = Convert.ToInt32(splitedMessage[3]);
                    //NCC.CallRequest();
                }

                else if (splitedMessage[0] == "CallAccept")
                {
                    Send(CallingPCC, CalledPCCName + "accepted call from " + CallingPCCName + " Call connection established succsefully. Data transmission enabled");
                }
            }
            catch (Exception) // tutaj nie wiem o co chodzi XDDDDDDDDD
            {
                // if the client has been shutdown, then close the connection
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                return;
            }
        }

        private static void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);

            ReturnLog("Sent message: " + data);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Odzyskanie socketu z obiektu stanu.
                Socket handler = (Socket)ar.AsyncState;

                // Zakończenie wysyłania danych do urządzenia.
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        private static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " " + log + "\n---------------------");
        }

    }
}
