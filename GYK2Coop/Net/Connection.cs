using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GYK2Coop.Net
{
    /// <summary>
    /// One TCP link to the other player. Reading and writing happen on background threads;
    /// the game thread only touches <see cref="Incoming"/> and <see cref="Send"/>.
    /// </summary>
    internal class Connection
    {
        private static readonly Stopwatch Clock = Stopwatch.StartNew();

        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly BlockingCollection<byte[]> outgoing = new BlockingCollection<byte[]>();
        private long lastReceiveMs;
        private volatile bool closed;

        public readonly ConcurrentQueue<Packet> Incoming = new ConcurrentQueue<Packet>();

        public string CloseReason { get; private set; }
        public bool IsClosed => closed;
        public string RemoteEndPoint { get; }

        // Progress of the frame currently being read (for the "receiving world" bar).
        public long IncomingFrameSize;
        public long IncomingFrameReceived;
        // Bytes still waiting to be written (for the "sending world" bar).
        public long PendingOutgoingBytes;

        public double SecondsSinceReceive => (Clock.ElapsedMilliseconds - Interlocked.Read(ref lastReceiveMs)) / 1000.0;

        public Connection(TcpClient client)
        {
            this.client = client;
            client.NoDelay = true;
            client.SendBufferSize = 256 * 1024;
            client.ReceiveBufferSize = 256 * 1024;
            stream = client.GetStream();
            try
            {
                RemoteEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "?";
            }
            catch
            {
                RemoteEndPoint = "?";
            }
            Interlocked.Exchange(ref lastReceiveMs, Clock.ElapsedMilliseconds);

            new Thread(ReadLoop) { IsBackground = true, Name = "GYK2Coop-read" }.Start();
            new Thread(WriteLoop) { IsBackground = true, Name = "GYK2Coop-write" }.Start();
        }

        public void Send(byte[] frame)
        {
            if (closed || outgoing.IsAddingCompleted)
                return;
            try
            {
                Interlocked.Add(ref PendingOutgoingBytes, frame.Length);
                outgoing.Add(frame);
            }
            catch (InvalidOperationException)
            {
                // Raced with close.
            }
        }

        /// <summary>Send whatever is queued, then close the socket.</summary>
        public void CloseAfterFlush(string reason)
        {
            if (CloseReason == null)
                CloseReason = reason;
            try
            {
                outgoing.CompleteAdding();
            }
            catch
            {
            }
            // Safety net in case the writer is stuck on a dead peer.
            var t = new Thread(() =>
            {
                Thread.Sleep(3000);
                Close(reason);
            }) { IsBackground = true };
            t.Start();
        }

        public void Close(string reason)
        {
            if (closed)
                return;
            closed = true;
            if (CloseReason == null)
                CloseReason = reason;
            try
            {
                outgoing.CompleteAdding();
            }
            catch
            {
            }
            try
            {
                client.Close();
            }
            catch
            {
            }
        }

        private void ReadLoop()
        {
            var header = new byte[4];
            try
            {
                while (!closed)
                {
                    ReadExactly(header, 4, false);
                    int len = header[0] | (header[1] << 8) | (header[2] << 16) | (header[3] << 24);
                    if (len < 1 || len > Protocol.MaxFrame)
                        throw new Exception("bad frame length " + len);

                    var frame = new byte[len];
                    Interlocked.Exchange(ref IncomingFrameSize, len);
                    Interlocked.Exchange(ref IncomingFrameReceived, 0);
                    ReadExactly(frame, len, true);
                    Interlocked.Exchange(ref IncomingFrameSize, 0);

                    var payload = new byte[len - 1];
                    Buffer.BlockCopy(frame, 1, payload, 0, payload.Length);
                    Incoming.Enqueue(new Packet { Type = (MsgType)frame[0], Payload = payload });
                }
            }
            catch (Exception e)
            {
                Close(closed ? CloseReason : "connection lost (" + e.Message + ")");
            }
        }

        private void ReadExactly(byte[] buf, int count, bool trackProgress)
        {
            int off = 0;
            while (off < count)
            {
                int n = stream.Read(buf, off, Math.Min(count - off, 64 * 1024));
                if (n <= 0)
                    throw new Exception("closed by remote");
                off += n;
                Interlocked.Exchange(ref lastReceiveMs, Clock.ElapsedMilliseconds);
                if (trackProgress)
                    Interlocked.Exchange(ref IncomingFrameReceived, off);
            }
        }

        private void WriteLoop()
        {
            try
            {
                foreach (byte[] frame in outgoing.GetConsumingEnumerable())
                {
                    int off = 0;
                    while (off < frame.Length)
                    {
                        int n = Math.Min(64 * 1024, frame.Length - off);
                        stream.Write(frame, off, n);
                        off += n;
                        Interlocked.Add(ref PendingOutgoingBytes, -n);
                    }
                }
                stream.Flush();
                Close(CloseReason ?? "closed");
            }
            catch (Exception e)
            {
                Close(closed ? CloseReason : "connection lost (" + e.Message + ")");
            }
        }

        /// <summary>Connects on a worker thread so the game does not freeze.</summary>
        public static void ConnectAsync(string host, int port, Action<TcpClient, string> done)
        {
            new Thread(() =>
            {
                var c = new TcpClient();
                try
                {
                    IAsyncResult ar = c.BeginConnect(host, port, null, null);
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10)))
                    {
                        c.Close();
                        done(null, "timed out connecting to " + host + ":" + port);
                        return;
                    }
                    c.EndConnect(ar);
                    done(c, null);
                }
                catch (Exception e)
                {
                    try
                    {
                        c.Close();
                    }
                    catch
                    {
                    }
                    done(null, e.Message);
                }
            }) { IsBackground = true, Name = "GYK2Coop-connect" }.Start();
        }
    }

    /// <summary>Accepts incoming TCP connections on a worker thread.</summary>
    internal class Listener
    {
        private readonly TcpListener listener;
        private volatile bool stopped;

        public readonly ConcurrentQueue<TcpClient> Accepted = new ConcurrentQueue<TcpClient>();

        public Listener(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            new Thread(AcceptLoop) { IsBackground = true, Name = "GYK2Coop-accept" }.Start();
        }

        private void AcceptLoop()
        {
            while (!stopped)
            {
                try
                {
                    TcpClient c = listener.AcceptTcpClient();
                    if (stopped)
                    {
                        c.Close();
                        return;
                    }
                    Accepted.Enqueue(c);
                }
                catch
                {
                    if (stopped)
                        return;
                    Thread.Sleep(200);
                }
            }
        }

        public void Stop()
        {
            stopped = true;
            try
            {
                listener.Stop();
            }
            catch
            {
            }
            while (Accepted.TryDequeue(out TcpClient c))
            {
                try
                {
                    c.Close();
                }
                catch
                {
                }
            }
        }
    }
}
