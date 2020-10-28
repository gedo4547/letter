using System.ComponentModel;
 
namespace System.IO.Pipelines
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDgramPipelineReader
    {
        void ReceiveAsync();

        DgramNode Read();
    }
}