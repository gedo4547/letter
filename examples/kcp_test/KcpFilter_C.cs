using System.Threading.Tasks;
using Letter.Kcp;

namespace kcp_test
{
    public class KcpFilter_C : KcpFilter
    {
        public KcpFilter_C(string filterName) : base(filterName)
        {
        }

        public override void OnTransportActive(IKcpSession session)
        {
            base.OnTransportActive(session);

            int count = 10;
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("kcp>>>>>>>>>>nihao_" + i.ToString());
                session.SafeSendAsync(bytes);
            }

            for (int i = 0; i < count; i++)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("udp>>>>>>>>>>nihao_" + i.ToString());
                session.UnsafeSendAsync(Program.s_address, bytes);
            }
           
        }
    }
}