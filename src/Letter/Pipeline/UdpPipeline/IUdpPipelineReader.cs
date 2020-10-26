using System.ComponentModel;
 
namespace Letter
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IUdpPipelineReader
    {
        void ReceiveAsync();

        UdpMessageNode Read();
    }
}