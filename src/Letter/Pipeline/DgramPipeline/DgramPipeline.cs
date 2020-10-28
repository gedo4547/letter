﻿using System;
using System.Buffers;
using System.ComponentModel;
using System.IO.Pipelines;
 
namespace System.IO.Pipelines
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class DgramPipeline : IDisposable
    {
        internal const int FALSE = 0;
        internal const int TRUE = 1;

        internal const int InitialSegmentPoolSize = 16; // 65K
        internal const int MaxSegmentPoolSize = 256; // 1MB
        
        public DgramPipeline(MemoryPool<byte> memoryPool, PipeScheduler scheduler, Action<IDgramPipelineReader> onReceived)
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
            this.nodeStack = new DgramNodeStack(InitialSegmentPoolSize);
            this.multiThreadedInvoke = (o) => this.onReceived(this);
        }

        private object syncObj = new object();

        private MemoryPool<byte> memoryPool;
        private PipeScheduler scheduler;
        private DgramNodeStack nodeStack;
        private Action<IDgramPipelineReader> onReceived;
        private Action<object> multiThreadedInvoke;

        private DgramNode headNode;
        private DgramNode tailNode;

        private int waiting = FALSE;


        private DgramNode CreationOrGetNode()
        {
            lock (syncObj)
            {
                if (!this.nodeStack.TryPop(out var node))
                {
                    IMemoryOwner<byte> memoryOwner = this.memoryPool.Rent();
                    node = new DgramNode(memoryOwner, this.OnDgramNodeRelease);
                }

                node.next = null;
                return node;
            }
        }

        private void OnDgramNodeRelease(DgramNode node)
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
            DgramNode tempNode = this.headNode;
            while (tempNode != null)
            {
                var nextNode = tempNode.next;
                tempNode.Dispose();
                tempNode = nextNode;
            }
            
            if (this.headNode != null)
            {
                this.headNode = null;
            }

            if (this.tailNode != null)
            {
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
            
            if (this.memoryPool != null)
            {
                this.memoryPool = null;
            }
        }
    }
}