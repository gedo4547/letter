﻿using System;
using System.Buffers;
using System.IO.Pipelines;

 namespace Letter
{
    public partial class UdpPipe : IDisposable
    {
        internal const int FALSE = 0;
        internal const int TRUE = 1;

        internal const int InitialSegmentPoolSize = 16; // 65K
        internal const int MaxSegmentPoolSize = 256; // 1MB
        
        public UdpPipe(MemoryPool<byte> memoryPool, PipeScheduler scheduler, Action<IUdpPipeReader> onReceived)
        {
            if (memoryPool == null)
                throw new ArgumentNullException(nameof(memoryPool));
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (onReceived == null)
                throw new ArgumentNullException(nameof(onReceived));

            this.memoryPool = memoryPool;
            this.scheduler = scheduler;
            this.onReceived = onReceived;
            this.nodeStack = new UdpMessageNodeStack(InitialSegmentPoolSize);
            this.multiThreadedInvoke = (o) => this.onReceived(this);
        }

        private object syncObj = new object();

        private MemoryPool<byte> memoryPool;
        private PipeScheduler scheduler;
        private UdpMessageNodeStack nodeStack;
        private Action<IUdpPipeReader> onReceived;
        private Action<object> multiThreadedInvoke;

        private UdpMessageNode headNode;
        private UdpMessageNode tailNode;

        private int waiting = FALSE;


        private UdpMessageNode CreationOrGetNode()
        {
            lock (syncObj)
            {
                if (!this.nodeStack.TryPop(out var node))
                {
                    IMemoryOwner<byte> memoryOwner = this.memoryPool.Rent();
                    node = new UdpMessageNode(memoryOwner, this.OnDgramNodeRelease);
                }

                node.next = null;
                return node;
            }
        }

        private void OnDgramNodeRelease(UdpMessageNode node)
        {
            lock (syncObj)
            {
                if (this.nodeStack.Count < MaxSegmentPoolSize)
                {
                    this.nodeStack.Push(node);
                }
                else
                {
                    node.Dispose();
                }
            }
        }
        

        public void Dispose()
        {
            if (this.memoryPool != null)
            {
                this.memoryPool.Dispose();
                this.memoryPool = null;
            }

            if (this.headNode != null)
            {
                this.headNode.Dispose();
                this.headNode = null;
            }


            if (this.tailNode != null)
            {
                this.tailNode.Dispose();
                this.tailNode = null;
            }

            if (this.nodeStack != null)
            {
                while (this.nodeStack.TryPop(out var node))
                {
                    node.Dispose();
                }

                this.nodeStack = null;
            }
        }
    }
}