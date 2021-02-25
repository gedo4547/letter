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

            //string str = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
            //Console.WriteLine($"频道：{session.Conv}，收到消息：{str}" );
        }

        public void OnTransportWrite(IKcpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            byte[] bytes = args.Value as byte[];
            //Console.WriteLine("kcp pipeline写入数据>>" + bytes.Length);
            writer.Write(bytes);
        }
    }
}