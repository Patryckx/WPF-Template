using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.Models;


namespace Example.Services.Interfaces
{
    public interface IStartupService 
    {
        Task InitializeAsync(
             IProgress<StartupProgress> progress);
    }
}
