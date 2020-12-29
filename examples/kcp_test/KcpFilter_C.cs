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
            
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("nihao");
            session.Write(bytes);
            session.FlushAsync().NoAwait();
        }
    }
}