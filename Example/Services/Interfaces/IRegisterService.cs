using Example.Models.Database_registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services.Interfaces
{
    public interface IRegisterService
    {

        Task AddLogAsync(Coil_register log);
    }
}
