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

            for (int i = 0; i < 1; i++)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("nihao" + i.ToString());
                session.SafeSendAsync(bytes);
            }
        }
    }
}