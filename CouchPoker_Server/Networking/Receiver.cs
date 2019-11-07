using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Networking
{
    class DataReceivedEventArgs : EventArgs
    {
        public STATUS status;

        public DataReceivedEventArgs(STATUS status)
        {
            this.status = status;
        }
    }

    class Receiver
    {
        public delegate void DataReceivedDelegate(DataReceivedEventArgs args);
        public event DataReceivedDelegate DataReceived;


    }
}
