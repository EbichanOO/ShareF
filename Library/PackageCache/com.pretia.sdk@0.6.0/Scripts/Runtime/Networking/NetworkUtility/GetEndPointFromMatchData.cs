using System;
using System.Net;
using UnityEngine;

namespace PretiaArCloud
{
    static partial class Utils
    {
        [Serializable]
        public struct MatchData
        {
            [Serializable]
            public struct PortData
            {
                public int port;
                public string protocal;
            }

            public PortData[] ports;
            public string host;
        }

        public static IPEndPoint GetEndPointFromMatchData(string json)
        {
            var matchData = JsonUtility.FromJson<MatchData>(json);
            return new IPEndPoint(IPAddress.Parse(matchData.host), matchData.ports[0].port);
        }
    }
}