using System;
using Microsoft.Web.XmlTransform;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;

namespace Calamari.Xdt {
    public class VerboseTransformLogger : IXmlTransformationLogger
    {
        public event LogDelegate Warning;
        readonly bool _suppressWarnings;
        readonly bool _suppressLogging;

        public VerboseTransformLogger(bool suppressWarnings = false, bool suppressLogging = false)
        {
            _suppressWarnings = suppressWarnings;
            _suppressLogging = suppressLogging;
        }

        public void LogMessage(string message, params object[] messageArgs)
        {
            if (!_suppressLogging)
            {
                Log.VerboseFormat(message, messageArgs);
            }
        }

        public void LogMessage(MessageType type, string message, params object[] messageArgs)
        {
            if (!_suppressLogging)
            {
                LogMessage(message, messageArgs);
            }
        }

        public void LogWarning(string message, params object[] messageArgs)
        {
            if (Warning != null) { Warning(this, new WarningDelegateArgs(string.Format(message, messageArgs))); }
            if (_suppressWarnings)
            {
                Log.Info(message, messageArgs);
            }
            else
            {
                Log.WarnFormat(message, messageArgs);
            }
        }

        public void LogWarning(string file, string message, params object[] messageArgs)
        {
            if (Warning != null) { Warning(this, new WarningDelegateArgs(string.Format("{0}: {1}", file, string.Format(message, messageArgs)))); }
            if (_suppressWarnings)
            {
                Log.Info("File {0}: ", file);
                Log.Info(message, messageArgs);
            }
            else
            {
                Log.WarnFormat("File {0}: ", file);
                Log.WarnFormat(message, messageArgs);
            }
        }

        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            if (Warning != null) { Warning(this, new WarningDelegateArgs(string.Format("{0}({1},{2}): {3}", file, lineNumber, linePosition, string.Format(message, messageArgs)))); }
            if (_suppressWarnings)
            {
                Log.Info("File {0}, line {1}, position {2}: ", file, lineNumber, linePosition);
                Log.Info(message, messageArgs);
            }
            else
            {
                Log.WarnFormat("File {0}, line {1}, position {2}: ", file, lineNumber, linePosition);
                Log.WarnFormat(message, messageArgs);
            }
        }

        public void LogError(string message, params object[] messageArgs)
        {
            Log.ErrorFormat(message, messageArgs);
        }

        public void LogError(string file, string message, params object[] messageArgs)
        {
            Log.ErrorFormat("File {0}: ", file);
            Log.ErrorFormat(message, messageArgs);
        }

        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            Log.ErrorFormat("File {0}, line {1}, position {2}: ", file, lineNumber, linePosition);
            Log.ErrorFormat(message, messageArgs);
        }

        public void LogErrorFromException(Exception ex)
        {
            Log.ErrorFormat(ex.ToString());
        }

        public void LogErrorFromException(Exception ex, string file)
        {
            Log.ErrorFormat("File {0}: ", file);
            Log.ErrorFormat(ex.ToString());
        }

        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            Log.ErrorFormat("File {0}, line {1}, position {2}: ", file, lineNumber, linePosition);
            Log.ErrorFormat(ex.ToString());
        }

        public void StartSection(string message, params object[] messageArgs)
        {
            if (!_suppressLogging)
            {
                Log.VerboseFormat(message, messageArgs);
            }
        }

        public void StartSection(MessageType type, string message, params object[] messageArgs)
        {
            if (!_suppressLogging)
            {
                StartSection(message, messageArgs);
            }
        }

        public void EndSection(string message, params object[] messageArgs)
        {
            if (!_suppressLogging)
            {
                Log.VerboseFormat(message, messageArgs);
            }
        }

        public void EndSection(MessageType type, string message, params object[] messageArgs)
        {
            if (!_suppressLogging)
            {
                EndSection(message, messageArgs);
            }
        }
    }
    public delegate void LogDelegate(object sender, WarningDelegateArgs args);
    public class WarningDelegateArgs
    {
        public string Message { get; set; }

        public WarningDelegateArgs(string message)
        {
            Message = message;
        }
    }

    public class Log {
        //static string stdOutMode;

        static readonly object Sync = new object();

        internal static IndentedTextWriter StdOut;
        internal static IndentedTextWriter StdErr;

        static Log()
        {
            StdOut = new IndentedTextWriter(Console.Out, "  ");
            StdErr = new IndentedTextWriter(Console.Error, "  ");
        }

        static void SetMode(string mode)
        {
            /*
            if (stdOutMode == mode) return;
            StdOut.WriteLine("##octopus[stdout-" + mode + "]");
            stdOutMode = mode;
            */
            return;
        }

        public static void Verbose(string message)
        {
            lock (Sync)
            {
                SetMode("verbose");
                StdOut.WriteLine(message);
            }
        }
        
        /*
        public static void SetOutputVariable(string name, string value)
        {
            SetOutputVariable(name, value, null);
        }

        public static void SetOutputVariable(string name, string value, VariableDictionary variables)
        {
            Info($"##octopus[setVariable name=\"{ConvertServiceMessageValue(name)}\" value=\"{ConvertServiceMessageValue(value)}\"]");

            variables?.SetOutputVariable(name, value);
        }

        static string ConvertServiceMessageValue(string value)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(value));
        }
        */

        public static void VerboseFormat(string messageFormat, params object[] args)
        {
            Verbose(string.Format(messageFormat, args));
        }

        public static void Info(string message)
        {
            lock (Sync)
            {
                SetMode("default");
                StdOut.WriteLine(message);
            }
        }

        public static void Info(string messageFormat, params object[] args)
        {
            Info(String.Format(messageFormat, args));
        }

        public static void Warn(string message)
        {
            lock (Sync)
            {
                SetMode("warning");
                StdOut.WriteLine(message);
            }
        }

        public static void WarnFormat(string messageFormat, params object[] args)
        {
            Warn(String.Format(messageFormat, args));
        }

        public static void Error(string message)
        {
            lock (Sync)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                StdErr.WriteLine(message);
                Console.ResetColor();
            }
        }

        public static void ErrorFormat(string messageFormat, params object[] args)
        {
            Error(string.Format(messageFormat, args));
        }  
    }
}