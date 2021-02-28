using System;
using System.Collections.Generic;
using System.Buffers;

namespace System.Net
{
    sealed class Kcp : IDisposable
    {
         public const int IKCP_RTO_NDL = 30;  // no delay min rto
        public const int IKCP_RTO_MIN = 100; // normal min rto
        public const int IKCP_RTO_DEF = 200;
        public const int IKCP_RTO_MAX = 60000;
        public const int IKCP_CMD_PUSH = 81; // cmd: push data
        public const int IKCP_CMD_ACK = 82; // cmd: ack
        public const int IKCP_CMD_WASK = 83; // cmd: window probe (ask)
        public const int IKCP_CMD_WINS = 84; // cmd: window size (tell)
        public const int IKCP_ASK_SEND = 1;  // need to send IKCP_CMD_WASK
        public const int IKCP_ASK_TELL = 2;  // need to send IKCP_CMD_WINS
        public const int IKCP_WND_SND = 32;
        public const int IKCP_WND_RCV = 32;
        public const int IKCP_MTU_DEF = 1400;
        public const int IKCP_ACK_FAST = 3;
        public const int IKCP_INTERVAL = 100;
        public const int IKCP_OVERHEAD = 24;
        public const int IKCP_DEADLINK = 20;
        public const int IKCP_THRESH_INIT = 2;
        public const int IKCP_THRESH_MIN = 2;
        public const int IKCP_PROBE_INIT = 7000;   // 7 secs to probe window size
        public const int IKCP_PROBE_LIMIT = 120000; // up to 120 secs to probe window
        public const int IKCP_SN_OFFSET = 12;



        internal struct ackItem
        {
            internal UInt32 sn;
            internal UInt32 ts;
        }

        // kcp members.
        UInt32 conv; UInt32 mtu; UInt32 mss; UInt32 state;
        UInt32 snd_una; UInt32 snd_nxt; UInt32 rcv_nxt;
        UInt32 ts_recent; UInt32 ts_lastack; UInt32 ssthresh;
        UInt32 rx_rttval; UInt32 rx_srtt;
        UInt32 rx_rto; UInt32 rx_minrto;
        UInt32 snd_wnd; UInt32 rcv_wnd; UInt32 rmt_wnd; UInt32 cwnd; UInt32 probe;
        UInt32 interval; UInt32 ts_flush;
        UInt32 nodelay; UInt32 updated;
        UInt32 ts_probe; UInt32 probe_wait;
        UInt32 dead_link; UInt32 incr;

        Int32 fastresend;
        Int32 nocwnd; Int32 stream;

        List<Segment> snd_queue = new List<Segment>(16);
        List<Segment> rcv_queue = new List<Segment>(16);
        List<Segment> snd_buf = new List<Segment>(16);
        List<Segment> rcv_buf = new List<Segment>(16);

        List<ackItem> acklist = new List<ackItem>(16);

        byte[] buffer;
        Int32 reserved;
        Action<byte[], int> output; // buffer, size

        // send windowd & recv window
        public UInt32 SndWnd { get { return snd_wnd; } }
        public UInt32 RcvWnd { get { return rcv_wnd; } }
        public UInt32 RmtWnd { get { return rmt_wnd; } }
        public UInt32 Mss { get { return mss; } }

        // get how many packet is waiting to be sent
        public int WaitSnd { get { return snd_buf.Count + snd_queue.Count; } }

        // internal time.
        public UInt32 CurrentMS { get { return Helper.currentMS(); } }

        private bool littleEndian;
        private SegmentAllotter segmentAllotter;

        // create a new kcp control object, 'conv' must equal in two endpoint
        // from the same connection.
        public Kcp(UInt32 conv_, bool littleEndian, MemoryPool<byte> memoryPool, Action<byte[], int> output_)
        {
            this.littleEndian = littleEndian;
            this.segmentAllotter = new SegmentAllotter(memoryPool, this.littleEndian);


            conv = conv_;
            snd_wnd = IKCP_WND_SND;
            rcv_wnd = IKCP_WND_RCV;
            rmt_wnd = IKCP_WND_RCV;
            mtu = IKCP_MTU_DEF;
            mss = mtu - IKCP_OVERHEAD;
            rx_rto = IKCP_RTO_DEF;
            rx_minrto = IKCP_RTO_MIN;
            interval = IKCP_INTERVAL;
            ts_flush = IKCP_INTERVAL;
            ssthresh = IKCP_THRESH_INIT;
            dead_link = IKCP_DEADLINK;
            buffer = new byte[mtu];
            output = output_;
        }

        // check the size of next message in the recv queue
        public int PeekSize()
        {

            if (0 == rcv_queue.Count) return -1;

            var seq = rcv_queue[0];

            if (0 == seq.frg) return seq.data.ReadableLength;

            if (rcv_queue.Count < seq.frg + 1) return -1;

            int length = 0;

            foreach (var item in rcv_queue)
            {
                length += item.data.ReadableLength;
                if (0 == item.frg)
                    break;
            }

            return length;
        }


        public int Recv(byte[] buffer)
        {
            // return Recv(buffer, 0, buffer.Length);
            return default;
        }

        // Receive data from kcp state machine
        //
        // Return number of bytes read.
        //
        // Return -1 when there is no readable data.
        //
        // Return -2 if len(buffer) is smaller than kcp.PeekSize().
        public int Recv(Buffer buffer)
        {
            var peekSize = PeekSize();
            if (peekSize < 0)
                return -1;

            // if (peekSize > length)
            //     return -2;

            var fast_recover = false;
            if (rcv_queue.Count >= rcv_wnd)
                fast_recover = true;

            // merge fragment.
            var count = 0;
            var n = 0;
            foreach (var seg in rcv_queue)
            {
                // copy fragment data into buffer.
                var readableBuffer = seg.data.ReadableBuffer;
                var readableLength = (int)readableBuffer.Length;
                buffer.WriteBytes(readableBuffer);
                // readableBuffer.CopyTo(buffer.AsSpan(n, readableLength));

                n += readableLength;

                count++;
                var fragment = seg.frg;
                this.segmentAllotter.Put(seg);
                if (0 == fragment) break;
            }

            if (count > 0)
            {
                rcv_queue.RemoveRange(0, count);
            }

            // move available data from rcv_buf -> rcv_queue
            count = 0;
            foreach (var seg in rcv_buf)
            {
                if (seg.sn == rcv_nxt && rcv_queue.Count + count < rcv_wnd)
                {
                    rcv_queue.Add(seg);
                    rcv_nxt++;
                    count++;
                }
                else
                {
                    break;
                }
            }

            if (count > 0)
            {
                rcv_buf.RemoveRange(0, count);
            }


            // fast recover
            if (rcv_queue.Count < rcv_wnd && fast_recover)
            {
                // ready to send back IKCP_CMD_WINS in ikcp_flush
                // tell remote my window size
                probe |= IKCP_ASK_TELL;
            }

            return n;
        }

        public int Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer.Length);
        }

        // user/upper level send, returns below zero for error
        public int Send(byte[] buffer, int index, int length)
        {
            if (0 == length) return -1;

            if (stream != 0)
            {
                var n = snd_queue.Count;
                if (n > 0)
                {
                    var seg = snd_queue[n - 1];
                    if (seg.data.ReadableLength < mss)
                    {
                        var capacity = (int)(mss - seg.data.ReadableLength);
                        var writen = Math.Min(capacity, length);
                        seg.data.WriteBytes(buffer, index, writen);
                        index += writen;
                        length -= writen;
                    }
                }
            }

            if (length == 0)
                return 0;

            var count = 0;
            if (length <= mss)
                count = 1;
            else
                count = (int)(((length) + mss - 1) / mss);

            if (count > 255) return -2;

            if (count == 0) count = 1;

            for (var i = 0; i < count; i++)
            {
                var size = Math.Min(length, (int)mss);

                var seg = this.segmentAllotter.Get();
                seg.data.WriteBytes(buffer, index, size);
                index += size;
                length -= size;

                seg.frg = (stream == 0 ? (byte)(count - i - 1) : (byte)0);
                snd_queue.Add(seg);
            }

            return 0;
        }

        // update ack.
        void update_ack(Int32 rtt)
        {
            // https://tools.ietf.org/html/rfc6298
            if (0 == rx_srtt)
            {
                rx_srtt = (UInt32)rtt;
                rx_rttval = (UInt32)rtt >> 1;
            }
            else
            {
                Int32 delta = (Int32)((UInt32)rtt - rx_srtt);
                rx_srtt += (UInt32)(delta >> 3);
                if (0 > delta) delta = -delta;

                if (rtt < rx_srtt - rx_rttval)
                {
                    // if the new RTT sample is below the bottom of the range of
                    // what an RTT measurement is expected to be.
                    // give an 8x reduced weight versus its normal weighting
                    rx_rttval += (uint)((delta - rx_rttval) >> 5);
                }
                else
                {
                    rx_rttval += (uint)((delta - rx_rttval) >> 2);
                }
            }

            var rto = (int)(rx_srtt + Helper._imax_(interval, rx_rttval << 2));
            rx_rto = Helper._ibound_(rx_minrto, (UInt32)rto, IKCP_RTO_MAX);
        }

        void shrink_buf()
        {
            if (snd_buf.Count > 0)
                snd_una = snd_buf[0].sn;
            else
                snd_una = snd_nxt;
        }

        void parse_ack(UInt32 sn)
        {

            if (Helper._itimediff(sn, snd_una) < 0 || Helper._itimediff(sn, snd_nxt) >= 0) return;

            foreach (var seg in snd_buf)
            {
                if (sn == seg.sn)
                {
                    // mark and free space, but leave the segment here,
                    // and wait until `una` to delete this, then we don't
                    // have to shift the segments behind forward,
                    // which is an expensive operation for large window
                    seg.acked = 1;
                    break;
                }
                if (Helper._itimediff(sn, seg.sn) < 0)
                    break;
            }
        }

        void parse_fastack(UInt32 sn, UInt32 ts)
        {
            if (Helper._itimediff(sn, snd_una) < 0 || Helper._itimediff(sn, snd_nxt) >= 0)
                return;

            foreach (var seg in snd_buf)
            {
                if (Helper._itimediff(sn, seg.sn) < 0)
                    break;
                else if (sn != seg.sn && Helper._itimediff(seg.ts, ts) <= 0)
                    seg.fastack++;
            }
        }

        void parse_una(UInt32 una)
        {
            var count = 0;
            foreach (var seg in snd_buf)
            {
                if (Helper._itimediff(una, seg.sn) > 0) {
                    count++;
                    this.segmentAllotter.Put(seg);
                }
                else
                    break;
            }

            if (count > 0)
                snd_buf.RemoveRange(0, count);
        }

        void ack_push(UInt32 sn, UInt32 ts)
        {
            acklist.Add(new ackItem { sn = sn, ts = ts });
        }

        bool parse_data(Segment newseg)
        {
            var sn = newseg.sn;
            if (Helper._itimediff(sn, rcv_nxt + rcv_wnd) >= 0 || Helper._itimediff(sn, rcv_nxt) < 0)
                return true;

            var n = rcv_buf.Count - 1;
            var insert_idx = 0;
            var repeat = false;
            for (var i = n; i >= 0; i--)
            {
                var seg = rcv_buf[i];
                if (seg.sn == sn)
                {
                    repeat = true;
                    //TODO gyd
                    this.segmentAllotter.Put(newseg);
                    break;
                }

                if (Helper._itimediff(sn, seg.sn) > 0)
                {
                    insert_idx = i + 1;
                    break;
                }
            }

            if (!repeat)
            {
                if (insert_idx == n + 1)
                    rcv_buf.Add(newseg);
                else
                    rcv_buf.Insert(insert_idx, newseg);
            }

            // move available data from rcv_buf -> rcv_queue
            var count = 0;
            foreach (var seg in rcv_buf)
            {
                if (seg.sn == rcv_nxt && rcv_queue.Count + count < rcv_wnd)
                {
                    rcv_nxt++;
                    count++;
                }
                else
                {
                    break;
                }
            }

            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                    rcv_queue.Add(rcv_buf[i]);
                rcv_buf.RemoveRange(0, count);
            }
            return repeat;
        }

        // Input when you received a low level packet (eg. UDP packet), call it
        // regular indicates a regular packet has received(not from FEC)
        // 
        // 'ackNoDelay' will trigger immediate ACK, but surely it will not be efficient in bandwidth
        public int Input(byte[] data, int index, int size, bool regular, bool ackNoDelay)
        {
            var s_una = snd_una;
            if (size < IKCP_OVERHEAD) return -1;

            Int32 offset = index;
            UInt32 latest = 0;
            int flag = 0;
            UInt64 inSegs = 0;

            while (true)
            {
                Feature data1 = new Feature()
                {
                    littleEndian = this.littleEndian
                };
                

                if (size - (offset - index) < IKCP_OVERHEAD) break;

                if (!data1.decode(this.conv, data, ref offset))
                {
                    return -1;
                }

                if (size - (offset - index) < data1.length) return -2;

                switch (data1.cmd)
                {
                    case IKCP_CMD_PUSH:
                    case IKCP_CMD_ACK:
                    case IKCP_CMD_WASK:
                    case IKCP_CMD_WINS:
                        break;
                    default:
                        return -3;
                }

                // only trust window updates from regular packets. i.e: latest update
                if (regular)
                {
                    rmt_wnd = data1.wnd;
                }

                parse_una(data1.una);
                shrink_buf();

                if (IKCP_CMD_ACK == data1.cmd)
                {
                    parse_ack(data1.sn);
                    parse_fastack(data1.sn, data1.ts);
                    flag |= 1;
                    latest = data1.ts;
                }
                else if (IKCP_CMD_PUSH == data1.cmd)
                {
                    var repeat = true;
                    if (Helper._itimediff(data1.sn, rcv_nxt + rcv_wnd) < 0)
                    {
                        ack_push(data1.sn, data1.ts);
                        if (Helper._itimediff(data1.sn, rcv_nxt) >= 0)
                        {
                            var seg = this.segmentAllotter.Get();
                            seg.conv = data1.conv;
                            seg.cmd = data1.cmd;
                            seg.frg = data1.frg;
                            seg.wnd = data1.wnd;
                            seg.ts = data1.ts;
                            seg.sn = data1.sn;
                            seg.una = data1.una;
                            seg.data.WriteBytes(data, offset, (int)data1.length);
                            repeat = parse_data(seg);
                        }
                    }
                }
                else if (IKCP_CMD_WASK == data1.cmd)
                {
                    // ready to send back IKCP_CMD_WINS in Ikcp_flush
                    // tell remote my window size
                    probe |= IKCP_ASK_TELL;
                }
                else if (IKCP_CMD_WINS == data1.cmd)
                {
                    // do nothing
                }
                else
                {
                    return -3;
                }

                inSegs++;
                offset += (int)data1.length;
            }

            // update rtt with the latest ts
            // ignore the FEC packet
            if (flag != 0 && regular)
            {
                var current = Helper.currentMS();
                if (Helper._itimediff(current, latest) >= 0)
                {
                    update_ack(Helper._itimediff(current, latest));
                }
            }

            // cwnd update when packet arrived
            if (nocwnd == 0)
            {
                if (Helper._itimediff(snd_una, s_una) > 0)
                {
                    if (cwnd < rmt_wnd)
                    {
                        var _mss = mss;
                        if (cwnd < ssthresh)
                        {
                            cwnd++;
                            incr += _mss;
                        }
                        else
                        {
                            if (incr < _mss)
                            {
                                incr = _mss;
                            }
                            incr += (_mss * _mss) / incr + (_mss) / 16;
                            if ((cwnd + 1) * _mss <= incr)
                            {
                                if (_mss > 0)
                                    cwnd = (incr + _mss - 1) / _mss;
                                else
                                    cwnd = incr + _mss - 1;
                            }
                        }
                        if (cwnd > rmt_wnd)
                        {
                            cwnd = rmt_wnd;
                            incr = rmt_wnd * _mss;
                        }
                    }
                }
            }

            // ack immediately
            if (ackNoDelay && acklist.Count > 0)
            {
                Flush(true);
            }

            return 0;
        }

        UInt16 wnd_unused()
        {
            if (rcv_queue.Count < rcv_wnd)
                return (UInt16)(rcv_wnd - rcv_queue.Count);
            return 0;
        }

        // flush pending data
        public UInt32 Flush(bool ackOnly)
        {
            var seg = new Feature()
            {
                littleEndian = this.littleEndian
            };
            
            seg.conv = conv;
            seg.cmd = IKCP_CMD_ACK;
            seg.wnd = wnd_unused();
            seg.una = rcv_nxt;

            var writeIndex = reserved;

            void makeSpace(int space)
            {
                if (writeIndex + space > mtu)
                {
                    output(buffer, writeIndex);
                    writeIndex = reserved;
                }
            }

            void flushBuffer()
            {
                if (writeIndex > reserved)
                {
                    output(buffer, writeIndex);
                }
            }

            // flush acknowledges
            for (var i = 0; i < acklist.Count; i++)
            {
                makeSpace(Kcp.IKCP_OVERHEAD);
                var ack = acklist[i];
                if ( Helper._itimediff(ack.sn, rcv_nxt) >=0 || acklist.Count - 1 == i)
                {
                    seg.sn = ack.sn;
                    seg.ts = ack.ts;
                    writeIndex += seg.encode(buffer, writeIndex);
                }
            }
            acklist.Clear();

            // flash remain ack segments
            if (ackOnly)
            {
                flushBuffer();
                //this.segmentAllotter.Put(seg);
                return interval;
            }

            uint current = 0;
            // probe window size (if remote window size equals zero)
            if (0 == rmt_wnd)
            {
                current = Helper.currentMS();
                if (0 == probe_wait)
                {
                    probe_wait = IKCP_PROBE_INIT;
                    ts_probe = current + probe_wait;
                }
                else
                {
                    if (Helper._itimediff(current, ts_probe) >= 0)
                    {
                        if (probe_wait < IKCP_PROBE_INIT)
                            probe_wait = IKCP_PROBE_INIT;
                        probe_wait += probe_wait / 2;
                        if (probe_wait > IKCP_PROBE_LIMIT)
                            probe_wait = IKCP_PROBE_LIMIT;
                        ts_probe = current + probe_wait;
                        probe |= IKCP_ASK_SEND;
                    }
                }
            }
            else
            {
                ts_probe = 0;
                probe_wait = 0;
            }

            // flush window probing commands
            if ((probe & IKCP_ASK_SEND) != 0)
            {
                seg.cmd = IKCP_CMD_WASK;
                makeSpace(IKCP_OVERHEAD);
                writeIndex += seg.encode(buffer, writeIndex);
            }

            if ((probe & IKCP_ASK_TELL) != 0)
            {
                seg.cmd = IKCP_CMD_WINS;
                makeSpace(IKCP_OVERHEAD);
                writeIndex += seg.encode(buffer, writeIndex);
            }

            probe = 0;

            // calculate window size
            var cwnd_ = Helper._imin_(snd_wnd, rmt_wnd);
            if (0 == nocwnd)
                cwnd_ = Helper._imin_(cwnd, cwnd_);

            // sliding window, controlled by snd_nxt && sna_una+cwnd
            var newSegsCount = 0;
            for (var k = 0; k < snd_queue.Count; k++)
            {
                if (Helper._itimediff(snd_nxt, snd_una + cwnd_) >= 0)
                    break;

                var newseg = snd_queue[k];
                newseg.conv = conv;
                newseg.cmd = IKCP_CMD_PUSH;
                newseg.sn = snd_nxt;
                snd_buf.Add(newseg);
                snd_nxt++;
                newSegsCount++;
            }

            if (newSegsCount > 0)
            {
                snd_queue.RemoveRange(0, newSegsCount);
            }

            // calculate resent
            var resent = (UInt32)fastresend;
            if (fastresend <= 0) resent = 0xffffffff;

            // check for retransmissions
            current = Helper.currentMS();
            UInt64 change = 0; UInt64 lostSegs = 0; UInt64 fastRetransSegs = 0; UInt64 earlyRetransSegs = 0;
            var minrto = (Int32)interval;

            for (var k = 0; k < snd_buf.Count; k++)
            {
                var segment = snd_buf[k];
                var needsend = false;
                if (segment.acked == 1)
                    continue;
                if (segment.xmit == 0)  // initial transmit
                {
                    needsend = true;
                    segment.rto = rx_rto;
                    segment.resendts = current + segment.rto;
                }
                else if (segment.fastack >= resent) // fast retransmit
                {
                    needsend = true;
                    segment.fastack = 0;
                    segment.rto = rx_rto;
                    segment.resendts = current + segment.rto;
                    change++;
                    fastRetransSegs++;
                }
                else if (segment.fastack > 0 && newSegsCount == 0) // early retransmit
                {
                    needsend = true;
                    segment.fastack = 0;
                    segment.rto = rx_rto;
                    segment.resendts = current + segment.rto;
                    change++;
                    earlyRetransSegs++;
                }
                else if (Helper._itimediff(current, segment.resendts) >= 0) // RTO
                {
                    needsend = true;
                    if (nodelay == 0)
                        segment.rto += rx_rto;
                    else
                        segment.rto += rx_rto / 2;
                    segment.fastack = 0;
                    segment.resendts = current + segment.rto;
                    lostSegs++;
                }
                
                if (needsend)
                {
                    current = CurrentMS;
                    segment.xmit++;
                    segment.ts = current;
                    segment.wnd = seg.wnd;
                    segment.una = seg.una;

                    var need = IKCP_OVERHEAD + segment.data.ReadableLength;
                    makeSpace(need);
                    writeIndex += segment.encode(buffer, writeIndex);

                    var readableBuffer = segment.data.ReadableBuffer;
                    var readableLength = (int)readableBuffer.Length;
                    readableBuffer.CopyTo(buffer.AsSpan(writeIndex, readableLength));

                    writeIndex += readableLength;

                    if (segment.xmit >= dead_link)
                    {
                        state = 0xFFFFFFFF;
                    }
                }

                // get the nearest rto
                var _rto = Helper._itimediff(segment.resendts, current);
                if (_rto > 0 && _rto < minrto)
                {
                    minrto = _rto;
                }
            }

            // flash remain segments
            flushBuffer();

            // cwnd update
            if (nocwnd == 0)
            {
                // update ssthresh
                // rate halving, https://tools.ietf.org/html/rfc6937
                if (change > 0)
                {
                    var inflght = snd_nxt - snd_una;
                    ssthresh = inflght / 2;
                    if (ssthresh < IKCP_THRESH_MIN)
                        ssthresh = IKCP_THRESH_MIN;
                    cwnd = ssthresh + resent;
                    incr = cwnd * mss;
                }

                // congestion control, https://tools.ietf.org/html/rfc5681
                if (lostSegs > 0)
                {
                    ssthresh = cwnd / 2;
                    if (ssthresh < IKCP_THRESH_MIN)
                        ssthresh = IKCP_THRESH_MIN;
                    cwnd = 1;
                    incr = mss;
                }

                if (cwnd < 1)
                {
                    cwnd = 1;
                    incr = mss;
                }
            }
            //this.segmentAllotter.Put(seg);
            return (UInt32)minrto;
        }

        // update state (call it repeatedly, every 10ms-100ms), or you can ask
        // ikcp_check when to call it again (without ikcp_input/_send calling).
        // 'current' - current timestamp in millisec.
        public void Update()
        {
            var current = Helper.currentMS();

            if (0 == updated)
            {
                updated = 1;
                ts_flush = current;
            }

            var slap = Helper._itimediff(current, ts_flush);

            if (slap >= 10000 || slap < -10000)
            {
                ts_flush = current;
                slap = 0;
            }
            //Console.WriteLine("slap>>>>" + slap);
            if (slap >= 0)
            {
                ts_flush += interval;
                if (Helper._itimediff(current, ts_flush) >= 0)
                    ts_flush = current + interval;
                Flush(false);
            }
        }

        // Determine when should you invoke ikcp_update:
        // returns when you should invoke ikcp_update in millisec, if there
        // is no ikcp_input/_send calling. you can call ikcp_update in that
        // time, instead of call update repeatly.
        // Important to reduce unnacessary ikcp_update invoking. use it to
        // schedule ikcp_update (eg. implementing an epoll-like mechanism,
        // or optimize ikcp_update when handling massive kcp connections)
        public UInt32 Check()
        {
            var current = Helper.currentMS();

            var ts_flush_ = ts_flush;
            var tm_flush_ = 0x7fffffff;
            var tm_packet = 0x7fffffff;
            var minimal = 0;

            if (updated == 0)
                return current;

            if (Helper._itimediff(current, ts_flush_) >= 10000 || Helper._itimediff(current, ts_flush_) < -10000)
                ts_flush_ = current;

            if (Helper._itimediff(current, ts_flush_) >= 0)
                return current;

            tm_flush_ = (int)Helper._itimediff(ts_flush_, current);

            foreach (var seg in snd_buf)
            {
                var diff = Helper._itimediff(seg.resendts, current);
                if (diff <= 0)
                    return current;
                if (diff < tm_packet)
                    tm_packet = (int)diff;
            }

            minimal = (int)tm_packet;
            if (tm_packet >= tm_flush_)
                minimal = (int)tm_flush_;
            if (minimal >= interval)
                minimal = (int)interval;

            return current + (UInt32)minimal;
        }

        // change MTU size, default is 1400
        public int SetMtu(Int32 mtu_)
        {
            if (mtu_ < 50 || mtu_ < (Int32)IKCP_OVERHEAD)
                return -1;
            if (reserved >= (int)(mtu - IKCP_OVERHEAD) || reserved < 0)
                return -1;

            var buffer_ = new byte[mtu_];
            if (null == buffer_)
                return -2;

            mtu = (UInt32)mtu_;
            mss = mtu - IKCP_OVERHEAD - (UInt32)reserved;
            buffer = buffer_;
            return 0;
        }

        // fastest: ikcp_nodelay(kcp, 1, 20, 2, 1)
        // nodelay: 0:disable(default), 1:enable
        // interval: internal update timer interval in millisec, default is 100ms
        // resend: 0:disable fast resend(default), 1:enable fast resend
        // nc: 0:normal congestion control(default), 1:disable congestion control
        public int NoDelay(int nodelay_, int interval_, int resend_, int nc_)
        {

            if (nodelay_ > 0)
            {
                nodelay = (UInt32)nodelay_;
                if (nodelay_ != 0)
                    rx_minrto = IKCP_RTO_NDL;
                else
                    rx_minrto = IKCP_RTO_MIN;
            }

            if (interval_ >= 0)
            {
                if (interval_ > 5000)
                    interval_ = 5000;
                else if (interval_ < 10)
                    interval_ = 10;
                interval = (UInt32)interval_;
            }

            if (resend_ >= 0)
                fastresend = resend_;

            if (nc_ >= 0)
                nocwnd = nc_;

            return 0;
        }

        // set maximum window size: sndwnd=32, rcvwnd=32 by default
        public int WndSize(int sndwnd, int rcvwnd)
        {
            if (sndwnd > 0)
                snd_wnd = (UInt32)sndwnd;

            if (rcvwnd > 0)
                rcv_wnd = (UInt32)rcvwnd;
            return 0;
        }

        public bool ReserveBytes(int reservedSize)
        {
            if (reservedSize >= (mtu - IKCP_OVERHEAD) || reservedSize < 0)
                return false;

            reserved = reservedSize;
            mss = mtu - IKCP_OVERHEAD - (uint)(reservedSize);
            return true;
        }

        public void SetStreamMode(bool enabled)
        {
            stream = enabled ? 1 : 0;
        }

        public void Dispose()
        {
            foreach (var item in this.snd_queue)
                this.segmentAllotter.Put(item);
            foreach (var item  in this.rcv_queue)
                this.segmentAllotter.Put(item);
            foreach (var item in this.snd_buf)
                this.segmentAllotter.Put(item);
            foreach (var item in this.rcv_buf)
                this.segmentAllotter.Put(item);
            
            this.snd_queue.Clear();
            this.rcv_queue.Clear();
            this.snd_buf.Clear();
            this.rcv_buf.Clear();
            
            this.acklist.Clear();
            
            this.segmentAllotter.Dispose();
        }
    }
    
}
