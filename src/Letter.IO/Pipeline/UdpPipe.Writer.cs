﻿using System;
using System.Buffers;
using System.Net;
using System.Threading;

namespace Letter.IO
{
    partial class UdpPipe : IUdpPipeWriter
    {
        public UdpMessageNode GetDgramNode()
        {
            return this.CreationOrGetNode();
        }
        
        public void Write(EndPoint point, byte[] bytes)
        {
            this.Write(point, bytes, 0, bytes.Length);
        }

        public void Write(EndPoint point, byte[] bytes, int offset, int count)
        {
            ReadOnlyMemory<byte> memory = new ReadOnlyMemory<byte>(bytes, offset, count);
            this.Write(point, ref memory);
        }

        public void Write(EndPoint point, ref ReadOnlyMemory<byte> memory)
        {
            var node = this.GetDgramNode();
            node.SettingPoint(point);
            node.Write(ref memory);
            this.Write(node);
        }

        public void Write(UdpMessageNode node)
        {
            lock (this.syncObj)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));
                if (node.Point == null)
                    throw new ArgumentNullException(nameof(node.Point));
                if (node.ReadableLength < 1)
                    throw new Exception("The readable length of a node cannot be 0");

                if ((this.headNode == null && this.tailNode != null) ||
                    (this.headNode != null && this.tailNode == null))
                    throw new Exception("pipe exception");

                if (this.headNode == null && this.tailNode == null)
                {
                    this.headNode = node;
                    this.tailNode = node;
                }
                else
                {
                    this.tailNode.next = node;
                    this.tailNode = node;
                }
            }
            
            if (Interlocked.CompareExchange(ref this.waiting, FALSE, TRUE) == TRUE)
            {
                this.scheduler.Schedule(this.multiThreadedInvoke, null);
            }
        }
    }
}