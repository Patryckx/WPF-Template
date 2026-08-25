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
        ProcessStatus AppGeneralStatus { get; set; }

        ProcessStatus DCVoltageStatus { get; set; }

        ProcessStatus ResistanceTestStatus{ get; set; }
        
        ProcessStatus LCDVisionTestStatus { get; set; }

    }
}
