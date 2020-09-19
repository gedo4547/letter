using System;
using Letter;
using Letter.Tcp;
using System.Buffers;


namespace tcp_test1
{
    public class TcpTestFilter_Client : ITcpChannelFilter
    {
        public void OnChannelActive(ITcpSession session)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes("nihao");
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bytes);

            for (int i = 0; i < 10; i++)
            {
                session.WriteAsync(ref buffer);
            }
            
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelActive)}");
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
            string str = System.Text.Encoding.UTF8.GetString(args.buffer.FirstSpan);
            Console.WriteLine("收到》》"+str);

            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelRead)}");
        }

        public void OnChannelWrite(ITcpSession session, ref WrappedStreamWriter writer, ref ChannelArgs args)
        {
            Console.WriteLine($"{nameof(TcpTestFilter_Client)}.{nameof(OnChannelWrite)}");
        }
    }
}