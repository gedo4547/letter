using System;
using System.Threading.Tasks;

namespace Letter
{
    public interface INetwork<TOptions> where TOptions : class, IOptions
    {
        void ConfigureOptions(Action<TOptions> optionsFactory);
        
        void Build();
        
        Task StopAsync();
    }
}