using System;
using Letter.IO;
using Letter.Tcp;
using EventArgs = Letter.EventArgs;

namespace tcp_test1
{
    public class TcpTestFilter_Client : ITcpFilter
    {
        public TcpTestFilter_Client(string name)
        {
            this.name = name;
        }

        private string name;
        
        public void OnFilterActive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Client)}.{nameof(OnFilterActive)}");
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("你好");
            context.WriteAsync(bytes, 0, bytes.Length);
        }

        public void OnFilterInactive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Client)}.{nameof(OnFilterInactive)}");
        }

        public void OnFilterException(ITcpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Client)}.{nameof(OnFilterException)}>>{ex}");
        }

        public void OnFilterRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            // var span = args.buffer.First.Span;
            // string str = System.Text.Encoding.UTF8.GetString(span);
            
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Client)}.{nameof(OnFilterRead)}>{args.buffer.Length}");
        }

        public void OnFilterWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Client)}.{nameof(OnFilterWrite)}");
        }
    }
}