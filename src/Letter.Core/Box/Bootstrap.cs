using System;

namespace Letter.Box
{
    public class Bootstrap<TNetwork> where TNetwork : INetwork
    {
        private Func<TNetwork> networkFactory;
        
        public void Network<T>() where T : TNetwork, new()
        {
            this.Network(() => { return new T(); });
        }

        public void Network(Func<TNetwork> networkFactory)
        {
            if (networkFactory == null)
                throw new ArgumentNullException(nameof(networkFactory));

            this.networkFactory = networkFactory;
        }
        
        
        
        
        
    }
}