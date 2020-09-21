using System;
using Letter;
using Letter.Tcp;
using System.Buffers;


namespace tcp_test1
{
    public class TcpTestFilter_Client : ITcpChannelFilter
    {
        public async void OnChannelActive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelActive)}");
            for (int i = 0; i < 10; i++)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes("你好"+i);
                ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bytes);
                await session.WriteAsync(ref buffer);
            }
            await session.DisposeAsync();
        }

        public void OnChannelException(ITcpSession session, Exception ex)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelException)}"+ex.ToString());
        }

        public void OnChannelInactive(ITcpSession session)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelInactive)}");
        }

        public void OnChannelRead(ITcpSession session, ref WrappedStreamReader reader, ref ChannelArgs args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelRead)}");
            var buffers = args.buffers;
            for (int i = 0; i < buffers.Count; i++)
            {
                var buffer = buffers[i];
                string str = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
                Console.WriteLine("收到》》"+str);
            }
        }

        public void OnChannelWrite(ITcpSession session, ref WrappedStreamWriter writer, ref ChannelArgs args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelWrite)}");
        }
    }
}