using System;
using System.Configuration;
using System.Security.Principal;
using CSVUtility.Configuration;
using CSVUtility.Core;
using CSVUtility.Core.Helpers;
using CSVUtility.Exceptions;
using CSVUtility.Helpers;
using log4net.Config;

namespace CSVUtility
{
    class Program
    {
        private const int S_OK = 0x00000000;            //  Operation successful	
        private const int E_UNEXPECTED = 0x8000FFF;     //  Unexpected failure	

        private static int innerExceptionLevel = 0;

        static int Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();
                var outputHandler = new OutputHandler();

                if (IsAdministrator() == false)
                    throw new AdministratorRequiredException("You must be an administrator to perform this action.");

                UtilityEngine engine = new UtilityEngine(outputHandler);

                ConfigurationHelper.Parse(engine, ReadConfigurationElement());
                ArgumentHelper.ParseArguments(engine, args);

                engine.Action();
            }
            catch (HelpDisplayedException)
            {
                return S_OK;
            }
            catch (Exception ex)
            {
                ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.DarkRed, "The following error has occurred within the software.");
                ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.DarkRed, ex.Message);
                ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.DarkRed, "\tStack Trace:");
                ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.DarkRed, "\t{0}", ex.StackTrace);
                OutputInnerException(ex);

                return E_UNEXPECTED;
            }

            return S_OK;
        }


        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static UtilityConfigurationSection ReadConfigurationElement()
        {
            var section = ConfigurationManager.GetSection(ConfigurationConstants.ConfigurationSectionName) as UtilityConfigurationSection;
            if (section == null)
                throw new ConfigurationErrorsException($"The <{ConfigurationConstants.ConfigurationSectionName}> configuration section is not defined in the applications config file.");

            return section;
        }

        private static void OutputInnerException(Exception ex)
        {
            if (ex.InnerException == null)
                return;

            innerExceptionLevel++;
            ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.Red, "Inner Exception Level {0}", innerExceptionLevel);
            ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.Red, "-> {0}", ex.InnerException.Message);
            ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.Red, "\tStack Trace:");
            ConsoleExtensions.ConsoleWriteLineWithForegroundColor(ConsoleColor.Red, "\t{0}", ex.StackTrace);

            OutputInnerException(ex.InnerException);
        }
    }
}
