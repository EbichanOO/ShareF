using System.Net;

namespace PretiaArCloud.Networking
{
    internal interface IUdpClientProxy
    {
        void Connect(IPEndPoint endPoint);
        byte[] Receive(ref IPEndPoint remoteEP);
        int Send(byte[] dgram, int bytes);
        void Close();
    }
}