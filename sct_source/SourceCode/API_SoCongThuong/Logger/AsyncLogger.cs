using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace API_SoCongThuong.Logger
{
    public class AsyncLogger
    {
        private readonly SoHoa_SoCongThuongContext _context;
        private readonly ILogger _logger;

        public AsyncLogger(ILogger<AsyncLogger> logger, SoHoa_SoCongThuongContext context)
        {
            _logger = logger;
            _context = context;
        }
        public void LogError(string message)
        {
            try
            {
                SaveLog(message, LogLevel.Error);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while logging message");
            }
            _logger.LogError(message);
        }

        public void LogDebug(string message)
        {
            try
            {
                SaveLog(message, LogLevel.Debug);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while logging message");
            }
            _logger.LogDebug(message);
        }

        public void LogWarn(string message)
        {
            try
            {
                SaveLog(message, LogLevel.Warning);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while logging message");
            }
            _logger.LogWarning(message);
        }

        public void LogInfo(string message)
        {
            try
            {
                SaveLog(message, LogLevel.Information);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while logging message");
            }
            _logger.LogInformation(message);
        }

        public void SaveLog(string message, LogLevel loglever)
        {
            try
            {
                SystemLog asynclog = JsonConvert.DeserializeObject<SystemLog>(message);
                _context.SystemLogs.Add(asynclog);
                _context.SaveChanges();
            }
            catch (Exception)
            {

                return;
            }
        }
    }
}
