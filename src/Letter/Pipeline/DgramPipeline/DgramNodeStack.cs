namespace System.IO.Pipelines
{
    class DgramNodeStack : ObjectStack<DgramNode>
    {
        public DgramNodeStack(int size) : base(size)
        {
        }
    }
}