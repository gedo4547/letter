using System.ComponentModel;
 
namespace Letter
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDgramPipelineReader
    {
        void ReceiveAsync();

        DgramMessageNode Read();
    }
}