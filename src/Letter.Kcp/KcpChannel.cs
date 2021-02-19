using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    delegate void RunnableUnitDelegate();

    sealed class KcpChannel : AChannel<IKcpSession, KcpOptions>, IKcpChannel, IFilter<IUdpSession>, IKcpSessionBuilder, IKcpRunnable, IEventSubscriber
    {
        public KcpChannel(KcpOptions options, IUdpChannel channel, IKcpScheduler scheduler, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);
            
            this.scheduler = scheduler;
            this.channel = channel;
            this.channel.ConfigurationSelfFilter((pipeline) => { pipeline.Add(this); });
            this.scheduler.Register(this);
        }

        private IUdpChannel channel;
        private IUdpSession session;
        
        private IKcpScheduler scheduler;
        
        private bool isStop = false;
        private bool isInvalid = false;
        private AKcpController controller;

        public bool IsActivate 
        {
            get  { return !(isStop || isInvalid); }
        }

        public TController BindSelfController<TController>(TController controller) where TController : AKcpController
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            this.controller = controller;
            this.controller.SetCreator(this);

            return controller;
        }

        public async Task BindAsync(EndPoint address)
        {
            await this.channel.StartAsync(address);
        }

        IKcpSession IKcpSessionBuilder.Build(uint conv, EndPoint remoteAddress, IKcpClosable closable)
        {
            if (this.isStop)
            {
                throw new Exception("The channel has stopped working");
            }
                
            if (this.isInvalid)
            {
                throw new Exception("Channel failure, call the StopAsync method to release the channel");
            }

            var localAddress = this.session.LocalAddress;
            var pipeline = base.CreateFilterPipeline();
            return new KcpSession(conv, remoteAddress, localAddress, options, session, this, pipeline, closable);
        }

        public void OnTransportActive(IUdpSession session)
        {
            this.session = session;
            this.controller.OnUdpActive(session);
        }
        public void OnTransportInactive(IUdpSession session)
        {
            this.session = null;
            this.controller.OnUdpInactive(session);
        }

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            this.isInvalid = true;

            this.controller.OnUdpException(session, ex);
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            if(this.isInvalid || this.isStop) return;

            this.controller.OnUdpMessageInput(session, ref reader, args);
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            if(this.isInvalid || this.isStop) return;

            this.controller.OnUdpMessageOutput(session, ref writer, args);
        }

        private object sync = new object();
        private RunnableUnitDelegate runnableUnits;

        public void Register(RunnableUnitDelegate runnableUnit)
        {
            lock (sync)
            {
                this.runnableUnits += runnableUnit;
            }
        }

        public void Unregister(RunnableUnitDelegate runnableUnit)
        {
            lock (sync)
            {
                this.runnableUnits -= runnableUnit;
            }
        }

        public void Update()
        {
            if (this.runnableUnits != null)
            {
                this.runnableUnits();
            }
        }

        public override async Task StopAsync()
        {
            if (this.isStop)
            {
                return;
            }

            this.isStop = true;
            this.runnableUnits = null;
            this.scheduler.Unregister(this);
            await base.StopAsync();

            if(this.channel != null)
            {
                await this.channel.StopAsync();
                this.channel = null;
            }

            this.session = null;
            this.controller.Dispose();
        }
    }
}