using cocosocket4unity;

namespace cocosocket4unity
{
    public partial class Kcp
    {
      class KcpSegmentFactory
      {
        
      }
        /**
         * SEGMENT
         */
        struct KcpSegment
        {
      
          public int conv;
          public byte cmd;
          public int frg;
          public int wnd;
          public int ts;
          public int sn;
          public int una;
          public int resendts;
          public int rto;
          public int fastack;
          public int xmit;
          public ByteBuf data;
      
          public KcpSegment(int size)
          {
            conv = 0;
            cmd = 0;
            frg = 0;
            wnd = 0;
            ts = 0;
            sn = 0;
            una = 0;
            resendts = 0;
            rto = 0;
            fastack = 0;
            xmit = 0;
            if (size > 0)
            {
              this.data = new ByteBuf(size);
            }
            else
            {
              this.data = null;
            }
          }
      
          /**
           * encode a segment into buffer
           *
           * @param buf
           * @param offset
           * @return
           */
          public int Encode(ByteBuf buf)
          {
            int off = buf.WriterIndex();
            buf.WriteIntLE(conv);
            buf.WriteByte(cmd);
            buf.WriteByte((byte)frg);
            buf.WriteShortLE((short)wnd);
            buf.WriteIntLE(ts);
            buf.WriteIntLE(sn);
            buf.WriteIntLE(una);
            buf.WriteIntLE(data == null ? 0 : data.ReadableBytes());
            return buf.WriterIndex() - off;
          }
        }
    }
}