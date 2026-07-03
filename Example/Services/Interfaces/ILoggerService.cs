using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services.Interfaces
{
    public interface ILoggerService
    {
        void Log(string message);

        void Error(string message,Exception? ex=null);

        void Warn(string message);

        void Info(string message);  


    }
}
