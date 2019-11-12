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
        public STATUS status;
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
            else if (message.Contains("BET"))
            {
                status = STATUS.BET;
                value = Int32.Parse(message.Substring("BET".Length));
            }
        }
    }

    public class Receiver
    {
        public delegate void DataReceivedDelegate(DataReceivedEventArgs args);
        public event DataReceivedDelegate DataReceived;
        public delegate void ClientDisconnectedDelegate();
        public event ClientDisconnectedDelegate ClientDisconnected;


        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;
        private Task task;

        public async void BeginReceive(TcpClient client)
        {
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;
            task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                while (true)
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int data = client.GetStream().Read(buffer, 0, client.ReceiveBufferSize);
                    if (data==0)
                    {
                        ClientDisconnected?.Invoke();
                        break;
                    }
                    string receivedMessage = Encoding.UTF8.GetString(buffer,0, data);
                    DataReceived?.Invoke(new DataReceivedEventArgs(receivedMessage));
                    /*if (!MainWindow.dispatcher.CheckAccess())
                        MainWindow.dispatcher.Invoke(() =>
                        {
                            DataReceived?.Invoke(new DataReceivedEventArgs(receivedMessage));
                        });
                    else DataReceived?.Invoke(new DataReceivedEventArgs(receivedMessage));*/
                }
            }, cancellationTokenSource.Token);

            try
            {
                await task;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public void StopReceiving()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
