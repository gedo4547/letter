namespace System.Threading
{
    public sealed class SchedulerOptions
    {
        public SchedulerOptions(SchedulerType type)
        {
            switch (type)
            {
                case SchedulerType.Kestrel:
                    this.SchedulerCount = Math.Min(Environment.ProcessorCount, 16);
                    break;
                case SchedulerType.Processor:
                    this.SchedulerCount = Environment.ProcessorCount * 2;
                    break;
                case SchedulerType.ThreadPool:
                    this.SchedulerCount = 0;
                    break;
            }

            this.Type = type;
        }
        
        public SchedulerOptions(int count)
        {
            this.Type = SchedulerType.Node;
            this.SchedulerCount = count;
        }

        public SchedulerType Type { get; }
        public int SchedulerCount { get; }

        public override string ToString()
        {
            return $"Type:{this.Type}, Count:{this.SchedulerCount}";
        }
    }


    public enum SchedulerType
    {
        Node,
        Kestrel,
        ThreadPool,
        Processor,
    }
}