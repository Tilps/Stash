using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApplication8
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5678);
            listener.Start();
            client = new TcpClient("localhost", 5678);
            var server = listener.AcceptTcpClient();
            ThreadPool.QueueUserWorkItem(callback =>
                {
                    byte[] bufferInner = new byte[8192];
                    while (true)
                    {
                        try
                        {
                            int bytes = server.Client.Receive(bufferInner);
                            if (bytes == 0)
                                return;
                            server.Client.Send(bufferInner, 0, bytes, SocketFlags.None);
                        }
                        catch
                        {
                            break;
                        }
                    }

                });
            ThreadPool.QueueUserWorkItem(callback =>
                {
                    byte[] bufferInner = new byte[24];
                    for (int i = 0; i < bufferInner.Length; i++)
                        bufferInner[i] = (byte) (i+1);
                    for (int i = 0; i < 10000000; i++)
                        client.Client.Send(bufferInner);
                });
            asyncEventArgs1 = new SocketAsyncEventArgs();
            asyncEventArgs1.Completed += AsyncEventArgsOnCompleted;
            asyncEventArgs2 = new SocketAsyncEventArgs();
            asyncEventArgs2.Completed += AsyncEventArgsOnCompleted;
            buffer = new byte[bigBufferLength];
            BeginReceive();
            while (true)
            {
                Console.ReadKey();
            }
        }

        private const int bigBufferLength = 1024*1024;

        private static void BeginReceive()
        {
            if (asyncEventArgs == asyncEventArgs1)
                asyncEventArgs = asyncEventArgs2;
            else
                asyncEventArgs = asyncEventArgs2;
            int length = rnd.Next(2) == 0 ? 8192 : 100;
            prevLastWrapped = lastWrapped;
            lastWrapped = false;
            if (lastWriteIndex + length <= bigBufferLength)
                asyncEventArgs.BufferList = new ArraySegment<byte>[]
                    {new ArraySegment<byte>(buffer, lastWriteIndex, length),};
            else
            {
                asyncEventArgs.BufferList = new ArraySegment<byte>[]
                    {
                        new ArraySegment<byte>(buffer, lastWriteIndex, bigBufferLength - lastWriteIndex),
                        new ArraySegment<byte>(buffer, 0, lastWriteIndex + length - bigBufferLength),
                    };
                lastWrapped = true;
            }
            Clear(length);
            prevLastWriteLength = lastWriteLength;
            lastWriteLength = length;
            prevWasSync = wasSync;
            wasSync = false;
            int newVal = Interlocked.Decrement(ref counter);
            if (newVal != -1)
            {
                Console.WriteLine("BeginReceive called wrong number of times. {0}", newVal);
                throw new Exception("Fail!");
            }
            //callerExited.Reset();
            if (!client.Client.ReceiveAsync(asyncEventArgs))
            {
                wasSync = true;
                //callerExited.Set();
                AsyncEventArgsOnCompleted(null, asyncEventArgs);
            }
            else
            {
                //callerExited.Set();
            }
        }
        //private static ManualResetEventSlim callerExited = new ManualResetEventSlim(false);

        private static bool lastWrapped = false;
        private static bool prevLastWrapped = false;

        private static bool wasSync = false;
        private static bool prevWasSync = false;

        private static volatile int counter = 0;

        private static void Clear(int length)
        {
            if (lastWriteIndex + length <= bigBufferLength)
                Array.Clear(buffer, lastWriteIndex, length);
            else
            {
                Array.Clear(buffer, lastWriteIndex, bigBufferLength - lastWriteIndex);
                Array.Clear(buffer, 0, lastWriteIndex + length - bigBufferLength);                
            }
        }

        private static void AsyncEventArgsOnCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            int newVal = Interlocked.Increment(ref counter);
            if (newVal != 0)
            {
                Console.WriteLine("OnCompleted called wrong number of times. {0}", newVal);
                throw new Exception("Fail!");
            }

            if (socketAsyncEventArgs.SocketError != SocketError.Success)
                return;
            if (socketAsyncEventArgs.BytesTransferred == 0)
                return;
            if (socketAsyncEventArgs.BytesTransferred > lastWriteLength)
            {
                Console.WriteLine("Receive too long, Sync:{0}:{1} LastWrapped:{2}:{3} Length:{4}:{5} ClaimedLength:{6}", wasSync, prevWasSync, lastWrapped, prevLastWrapped, lastWriteLength, prevLastWriteLength, socketAsyncEventArgs.BytesTransferred);
            }
            if (buffer[(socketAsyncEventArgs.BytesTransferred-1 + lastWriteIndex) % bigBufferLength] == 0)
            {
                Console.WriteLine("Receive too short-quickcheck , Sync:{0}:{1} LastWrapped:{2}:{3} Length:{4}:{5}", wasSync, prevWasSync, lastWrapped, prevLastWrapped, lastWriteLength, prevLastWriteLength);
            }
            //for (int i = 0; i < socketAsyncEventArgs.BytesTransferred; i++)
            //{
            //    if (buffer[(i + lastWriteIndex) % bigBufferLength] == 0)
            //    {
            //        Console.WriteLine("Receive too short , Sync:{0}:{1} LastWrapped:{2}:{3} Length:{4}:{5} ActualLength:{6}", wasSync, prevWasSync, lastWrapped, prevLastWrapped, lastWriteLength, prevLastWriteLength, i);
            //        break;
            //    }
            //}
            lastWriteIndex = (lastWriteIndex + socketAsyncEventArgs.BytesTransferred)%bigBufferLength;
            //callerExited.Wait();
            BeginReceive();
        }

        private static SocketAsyncEventArgs asyncEventArgs;
        private static SocketAsyncEventArgs asyncEventArgs1;
        private static SocketAsyncEventArgs asyncEventArgs2;
        private static byte[] buffer;
        private static int lastWriteIndex;
        private static int lastWriteLength;
        private static int prevLastWriteLength = 0;
        private static Random rnd =new Random();
        private static TcpClient client;
    }
}
