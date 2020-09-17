﻿using System;
using System.Threading;

namespace Letter
{
    public partial class DgramPipeline : IDgramPipelineReader
    {
        public void ReceiveAsync()
        {
            if (this.headNode == null)
            {
                Interlocked.Exchange(ref this.waiting, TRUE);
                return;
            }
            
            this.scheduler.Schedule(this.multiThreadedInvoke, null);
        }

        public DgramMessageNode Read()
        {
            lock (this.syncObj)
            {
                if (this.headNode == null)
                {
                    return null;
                }

                DgramMessageNode node;
                if (this.headNode == this.tailNode)
                {
                    node = this.headNode;
                    this.headNode = null;
                    this.tailNode = null;
                }
                else
                {
                    node = this.headNode;
                    this.headNode = node.next;
                }
                return node;
            }
        }
    }
}