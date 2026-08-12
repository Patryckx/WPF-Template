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

        private string _currentLogFile =string.Empty;

        private readonly DateTime _startTime;

        private readonly string _sessionId;

        private readonly bool _isEnabled;

        public LoggerService(IConfigService config)
        {
            _config = config;

            _startTime = DateTime.Now;

            _sessionId = Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpper();

            _isEnabled = _config.LogginEnabled;

            if (!_isEnabled)
                return;

            CreateLogFile();

            WriteHeader();
        }

        public void Log(LogCategory category, string message) => Write("INFO",category, message);
        public void Info(LogCategory category,  string message) => Write("INFO", category, message);
        public void Warn(LogCategory category, string message) => Write("WARNING", category, message);
        public void Error(LogCategory category, string message, Exception? ex = null) => Write("ERROR", category, message + (ex != null ? $" | {ex}" : ""));

        private void CreateLogFile()
        {
            string folder =
                Path.Combine(
                    _config.LogPath,
                    _startTime.ToString("yyyy-MM-dd"));

            Directory.CreateDirectory(folder);

            _currentLogFile =
                    Path.Combine(
                        folder,
                        $"{_startTime:HH-mm-ss}.txt");
        }


        private void WriteHeader()
        {
            StringBuilder sb = new();
            sb.AppendLine("======================================================");
            sb.AppendLine("              Example Application");
            sb.AppendLine("======================================================");
            sb.AppendLine();

            sb.AppendLine($"Inicio : {_startTime:yyyy-MM-dd HH:mm:ss}");

            var version =
                typeof(LoggerService)
                    .Assembly
                    .GetName()
                    .Version;

            sb.AppendLine($"Versión           : {version}");
            sb.AppendLine($"SessionId         : {_sessionId}");

            sb.AppendLine($"Usuario Windows   : {Environment.UserName}");

            sb.AppendLine($"Equipo            : {Environment.MachineName}");

            sb.AppendLine($"Sistema Operativo : {Environment.OSVersion}");

            sb.AppendLine($"Arquitectura      : {(Environment.Is64BitProcess ? "x64" : "x86")}");

            sb.AppendLine($"Runtime           : {Environment.Version}");

            using Process process = Process.GetCurrentProcess();

            sb.AppendLine($"Proceso           : {process.ProcessName}");
            sb.AppendLine($"PID               : {process.Id}");

            sb.AppendLine();
            sb.AppendLine("Configuración");
            sb.AppendLine("--------------------------------------------");

            sb.AppendLine($"Host PLC          : {_config.DAQHost}");
            sb.AppendLine($"Puerto            : {_config.Port}");
            sb.AppendLine($"Base de datos     : {_config.DatabaseAddress}");
            sb.AppendLine($"Logs habilitados  : {_config.LogginEnabled}");

            sb.AppendLine();
            sb.AppendLine("Archivo Log");
            sb.AppendLine("--------------------------------------------");
            sb.AppendLine(_currentLogFile);

            sb.AppendLine();
            sb.AppendLine("======================================================");
            sb.AppendLine();

            try
            {
                File.WriteAllText(
                _currentLogFile,
                sb.ToString());

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }


        private void Write(
            string level,
            LogCategory category,
            string message,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {

            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();

            TimeSpan uptime = DateTime.Now - _startTime;

            string uptimeText = uptime.ToString(@"hh\:mm\:ss");

            string filename=Path.GetFileName(file);


            string logLine =
                $"[{uptimeText}] " +
                $"[{_sessionId}] " +
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
                    File.AppendAllText(
                        _currentLogFile,
                        logLine + Environment.NewLine);
                }

            } catch (Exception ex)
            {
                Debug.WriteLine($"Logger failed: {ex}");
            }

        }
    }
}
