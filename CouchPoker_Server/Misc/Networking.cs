using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Misc
{
    public static class Networking
    {
        public static string GetResponseFromRemote(TcpClient client, string question)
        {
            try
            {
                byte[] message = Encoding.UTF8.GetBytes(Security.Encrypt(question) + "\n");
                client.GetStream().Write(message, 0, message.Length);

                byte[] buffer = new byte[client.ReceiveBufferSize];
                int data = client.GetStream().Read(buffer, 0, client.ReceiveBufferSize);

                if (data == 0) return null;

                string msg = Encoding.UTF8.GetString(buffer, 0, data);
                return Security.Decrypt(msg);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public static void SendToRemote(TcpClient client, string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(Security.Encrypt(message)+"\n");
            client.GetStream().Write(msg, 0, msg.Length);
        }
    }
}
