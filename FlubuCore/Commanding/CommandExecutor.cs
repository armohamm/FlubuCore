﻿using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly CommandArguments _args;
        private readonly IBuildScriptLocator _locator;
        private readonly ITaskSession _taskSession;

        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(
            CommandArguments args,
            IBuildScriptLocator locator,
            ITaskSession taskSession,
            ILogger<CommandExecutor> log)
        {
            _args = args;
            _locator = locator;
            _taskSession = taskSession;
            _log = log;
        }

        public async Task<int> ExecuteAsync()
        {
            string version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            _log.LogInformation($"Flubu v.{version}");

            if (_args.Help)
            {
                return 1;
            }

            try
            {
                IBuildScript script = await _locator.FindBuildScript(_args);

                if (script == null)
                {
                    ReportUnspecifiedBuildScript();
                    return -1;
                }

	            _taskSession.ScriptArgs = _args.ScriptArguments;
                var result = script.Run(_taskSession);

                return result;
            }
            catch (BuildScriptLocatorException e)
            {
                if (_args.RethrowOnException)
                    throw;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e}", null, (t, ex) => t);
                return StatusCodes.BuildScriptNotFound;
            }
            catch (Exception e)
            {
                if (_args.RethrowOnException)
                    throw;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e}", null, (t, ex) => t);
                return 3;
            }
        }

        private static void ReportUnspecifiedBuildScript()
        {
            StringBuilder errorMsg = new StringBuilder("The build script file was not specified. Please specify it as the first argument or use some of the default paths for script file: ");
            foreach (var defaultScriptLocation in BuildScriptLocator.DefaultScriptLocations)
            {
                errorMsg.AppendLine(defaultScriptLocation);
            }

            throw new BuildScriptLocatorException(errorMsg.ToString());
        }
    }
}