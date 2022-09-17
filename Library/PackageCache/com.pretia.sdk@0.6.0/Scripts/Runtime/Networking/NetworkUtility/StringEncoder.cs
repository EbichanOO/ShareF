using System.Text;

namespace PretiaArCloud.Networking
{
    public static class StringEncoder
    {
        public static Encoding Instance
        {
            get { return Encoding.UTF8; }
        }
    }
}