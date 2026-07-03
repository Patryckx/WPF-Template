using ConfigIniLib.interfaces;
using Example.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Example.Models;


namespace Example.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly IConfigService _config;

        private readonly object _lock = new();

        public LoggerService(IConfigService config)
        {
            _config = config;

            if (_config.LogginEnabled)
            {
                Directory.CreateDirectory(_config.LogPath);
            }


        }

        public void Log(LogCategory category, string message) => Write("INFO",category, message);
        public void Info(LogCategory category,  string message) => Write("INFO", category, message);
        public void Warn(LogCategory category, string message) => Write("WARNING", category, message);
        public void Error(LogCategory category, string message, Exception? ex = null) => Write("ERROR", category, message + (ex != null ? $" | {ex}" : ""));


        private void Write(
            string level,
            LogCategory category,
            string message,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();


            string filename=Path.GetFileName(file);


            string logLine =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                $"[{level}] " +
                $"[{category}] " +
                $"[Thread:{threadId}] " +
                $"[{filename}:{member}:{line}] " +
                $"=> {message}";


            Debug.WriteLine(logLine);


            if (!_config.LogginEnabled)
                return;

            try
            {
                string path =Path.Combine(
                    _config.LogPath,
                    $"log_{DateTime.Now:yyyy-MM-dd}.txt");
                lock (_lock)
                {
                    File.AppendAllText(path, logLine + Environment.NewLine);
                }

            } catch (Exception ex)
            {
                Debug.WriteLine($"Logger failed: {ex}");
            }



        }
    }
}
