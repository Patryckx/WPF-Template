using Example.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services.Interfaces
{
    public interface IAppStateService
    {
        ProcessStatus DCVoltageStatus { get; set; }

        ProcessStatus SecondTestStatus { get; set; }

        ProcessStatus ThirdTestStatus { get; set; }
    }
}
