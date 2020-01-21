using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CouchPoker_Server.Networking
{
    public class DataReceivedEventArgs : EventArgs
    {
        public STATUS status = STATUS.NO_ACTION;
        public string message;
        public int value;

        public DataReceivedEventArgs(STATUS status)
        {
            this.status = status;
        }

        public DataReceivedEventArgs(string message)
        {
            this.message = message;
            if (message == "FOLD") status = STATUS.FOLD;
            else if (message == "CHECK") status = STATUS.CHECK;
            else if (message.Contains("RAISE"))
            {
                status = STATUS.RAISE;
                value = Int32.Parse(message.Substring("RAISE".Length));
            }
        }
    }

    public class Receiver
    {
        public delegate void DataReceivedDelegate(DataReceivedEventArgs args);
        public event DataReceivedDelegate DataReceived;
        public delegate void ClientDisconnectedDelegate();
        public event ClientDisconnectedDelegate ClientDisconnected;


        private Task task;
        public volatile bool IsRunning = false;

        public async void BeginReceive(TcpClient client)
        {
            task = Task.Run(() =>
            {
                IsRunning = true;
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[client.ReceiveBufferSize];
                        int data = client.GetStream().Read(buffer, 0, client.ReceiveBufferSize);
                        if (data == 0)
                        {
                            ClientDisconnected?.Invoke();
                            break;
                        }
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, data);
                        DataReceived?.Invoke(new DataReceivedEventArgs(Misc.Security.Decrypt(receivedMessage)));
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        ClientDisconnected?.Invoke();
                        break;
                    }
                }
                IsRunning = false;
            });

            try
            {
                await task;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
        }

        public void StopReceiving()
        {
            //cancellationTokenSource.Cancel();
            IsRunning = false;
        }
    }
}
