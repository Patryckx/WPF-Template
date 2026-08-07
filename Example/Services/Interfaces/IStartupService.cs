using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services.Interfaces
{
    public interface IStartupService :INotifyPropertyChanged
    {
        double Progress { get; }

        string StatusMessage { get; }

        Task StartAsync();
    }
}
