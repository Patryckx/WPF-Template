using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.Models;


namespace Example.Services.Interfaces
{
    public interface ILoggerService
    {
        void Log(LogCategory category ,string message);

        void Error(LogCategory category, string message,Exception? ex=null);

        void Warn(LogCategory category, string message);

        void Info(LogCategory category, string message);  


    }
}
