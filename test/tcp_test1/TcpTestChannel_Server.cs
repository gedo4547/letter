using System;
using System.Buffers;
using Letter.IO;
using Letter.Tcp;
using EventArgs = Letter.EventArgs;

namespace tcp_test1
{
    public class TcpTestFilter_Server : ITcpFilter
    {
        public TcpTestFilter_Server(string name)
        {
            this.name = name;
        }

        private string name;

        public void OnFilterActive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Server)}.{nameof(OnFilterActive)}");
        }

        public void OnFilterInactive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Server)}.{nameof(OnFilterInactive)}");
        }

        public void OnFilterException(ITcpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Server)}.{nameof(OnFilterException)}>>{ex}");
        }

        public void OnFilterRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Server)}.{nameof(OnFilterRead)}>{args.buffer.Length}");
            var span = args.buffer.First.Span;
            string str = System.Text.Encoding.UTF8.GetString(span);
            Console.WriteLine(str);
        }

        public void OnFilterWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestFilter_Server)}.{nameof(OnFilterWrite)}");
        }
    }
}