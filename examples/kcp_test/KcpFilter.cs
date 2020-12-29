using System;
using System.IO.Pipelines;
using Letter.IO;
using Letter.Kcp;

namespace kcp_test
{
    public abstract class KcpFilter : IKcpFilter
    {
        public KcpFilter(string filterName)
        {
            this.filterName = filterName;
        }

        protected string filterName;

        public virtual void OnTransportActive(IKcpSession session)
        {
            Console.WriteLine($"{filterName}>>>>>OnTransportActive");
        }

        public void OnTransportInactive(IKcpSession session)
        {
        }

        public void OnTransportException(IKcpSession session, Exception ex)
        {
            Console.WriteLine(">>>>>>>>>>>>>>>>>" + ex.ToString());
        }

        public void OnTransportRead(IKcpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            int length = (int)reader.Length;
            var buffer = reader.ReadBuffer(length);
            string str = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
            Console.WriteLine("收到》》" + str);
        }

        public void OnTransportWrite(IKcpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            Console.WriteLine("写入数据");
            byte[] bytes = args.Value as byte[];
            writer.Write(bytes);
        }
    }
}