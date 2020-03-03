#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlyChain.Network {
    public sealed class TcpServer : IAsyncDisposable {
        private readonly IClient client;
        private readonly Socket tcpSocket;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public IPAddress BindIPAddress { get; }

        public int TcpPort { get; }

        public delegate ValueTask AcceptClientHandler(Node node, Socket client);

        public event AcceptClientHandler AcceptClient = null!;

        public TcpServer(IClient client, int tcpPort, IPAddress? bindIP = null) {
            bindIP ??= IPAddress.IPv6Any;
            this.client = client;
            BindIPAddress = bindIP;

            #region Socket相关
            tcpSocket = new Socket(bindIP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            if (bindIP.AddressFamily == AddressFamily.InterNetworkV6) {
                tcpSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            }
            tcpSocket.Bind(new IPEndPoint(bindIP, tcpPort));
            tcpSocket.Listen(10);
            TcpPort = ((IPEndPoint)tcpSocket.LocalEndPoint).Port;
            #endregion
        }

        private async void Start() {
            while (!cancellationTokenSource.IsCancellationRequested) {
                try {
                    StartAcceptClient(await tcpSocket.AcceptAsync());
                } catch { }
            }
        }

        private async void StartAcceptClient(Socket client) {
            try {
                using var stream = new NetworkStream(client, ownsSocket: true) {
                    ReadTimeout = 2000
                };
                if (AcceptClient is null) return;
                using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
                

                await AcceptClient(null, client);
            } catch {

            }
        }


        #region IDisposable Support
        private bool isDisposed = false;

        async ValueTask Dispose(bool disposing) {
            if (!isDisposed) {

                if (disposing) {
                    GC.SuppressFinalize(this);
                }
                isDisposed = true;
            }
        }

        ~TcpServer() {
            Dispose(false).AsTask().Wait();
        }

        public ValueTask DisposeAsync() => Dispose(true);
        #endregion
    }
}
