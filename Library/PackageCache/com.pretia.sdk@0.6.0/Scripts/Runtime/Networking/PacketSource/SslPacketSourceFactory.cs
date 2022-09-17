using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PretiaArCloud.Networking
{
    public class SslPacketSourceFactory : IPacketSourceFactory
    {
        private const string certString = @"-----BEGIN CERTIFICATE-----
            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
            -----END CERTIFICATE-----
            ";

        private TcpClient CreateTcpClient()
        {
            TcpClient client = new TcpClient();

            client.NoDelay = true;
            client.ReceiveBufferSize = ushort.MaxValue;
            client.SendBufferSize = ushort.MaxValue;

            return client;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
           if (localCert.GetPublicKeyString() == certificate.GetPublicKeyString())
                return true;

            Debug.LogError($"Certificate error: {sslPolicyErrors}");

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        X509Certificate localCert;

        public async Task<IPacketSource> CreateAsync(string address, int port, CancellationToken cancellationToken = default)
        {
            localCert = new X509Certificate(System.Text.Encoding.UTF8.GetBytes(certString));
            var certs = new X509CertificateCollection();
            certs.Add(localCert);

            TcpClient client = CreateTcpClient();
            cancellationToken.Register(client.Close);

            await client.ConnectAsync(address, port);

            if (client.Connected)
            {
                SslStream sslStream = new SslStream(
                    client.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null
                );

                try
                {
                    sslStream.AuthenticateAsClient("localhost", certs, SslProtocols.Tls12, true);
                }
                catch(AuthenticationException e)
                {
                    Debug.LogException(e);
                    if (e.InnerException != null)
                    {
                        Debug.LogException(e.InnerException);
                    }

                    client.Close();
                    throw e;
                }
                
                var packetSource = new TcpPacketSource(sslStream, client);
                return packetSource;
            }
            else
            {
                throw new System.NullReferenceException();
            }
        }

        public async Task<IPacketSource> CreateAsync(IPAddress address, int port, CancellationToken cancellationToken = default)
        {
            TcpClient client = CreateTcpClient();
            client.NoDelay = true;

            cancellationToken.Register(client.Close);

            await client.ConnectAsync(address, port);

            if (client.Connected)
            {
                var packetSource = new TcpPacketSource(client.GetStream(), client);
                return packetSource;
            }
            else
            {
                throw new System.NullReferenceException();
            }
        }
    }
}