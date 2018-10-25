using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace KLogNLight
{
    public sealed class KLogger:LoggerBase
    {
        public KLogger(string filename, string delimita = ",") : base(filename, delimita) { }

        protected override string messageCat(StringBuilder sb, string kind, string mes, string delimita)
        {
            string result;
            //test
            sb.Append("00:00");
            //sb.Append(DateTime.Now);
            sb.Append(delimita);
            sb.Append(kind);

            if (string.IsNullOrEmpty(mes))
            {
                result = sb.ToString();
                sb.Clear();
                return result;
            }

            sb.Append(delimita);
            sb.Append(mes);
            result = sb.ToString();
            sb.Clear();
            return result;
        }

        protected override string CheckMessage(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("message is null");
            }

            return s;
        }
    }

    public class SimpleLogger:LoggerBase
    {
        public SimpleLogger(string filename, string delimita = ",") : base(filename, delimita) { }
    }

    public abstract class LoggerBase:ILogger
    {
        public LoggerBase(string filename, string _delimita)
        {
            loglevel = LogLevel.GetInstance;
            fileope = TextFileOperator.GetInstance(filename);
            delimita = _delimita;
            sb=new StringBuilder();
        }

        public void Dispose()
        {
            fileope.Dispose();
        }

        public virtual string Info(string mes)
        {
            if (!CheckContainsLevel("Info")) return "";
            string ret;
            lock (lockobj)
            {
                ret = messageCat(sb, "[Info]", CheckMessage(mes), delimita);
                fileope.Write(ret);
            }

            return ret;
        }

        public virtual string Operation(string mes)
        {
            if (!CheckContainsLevel("Operation")) return "";
            string ret = messageCat(sb, "[Operation]", CheckMessage(mes), delimita);
            fileope.Write(ret);
            return ret;
        }

        public virtual string Debug(string mes)
        {
            if (!CheckContainsLevel("Debug")) return "";
            string ret = messageCat(sb, "[Debug]", CheckMessage(mes), delimita);
            fileope.Write(ret);
            return ret;
        }

        public virtual string Trace(string mes)
        {
            if (!CheckContainsLevel("Trace")) return "";
            string ret = messageCat(sb, "[Trace]", CheckMessage(mes), delimita);
            fileope.Write(ret);
            return ret;
        }

        public virtual string Warn(string mes)
        {
            if (!CheckContainsLevel("Warn")) return "";
            string ret = messageCat(sb, "[Warn]", CheckMessage(mes), delimita);
            fileope.Write(ret);
            return ret;
        }

        public virtual string Error(string mes)
        {
            if (!CheckContainsLevel("Error")) return "";
            string ret = messageCat(sb, "[Error]", CheckMessage(mes), delimita);
            fileope.Write(ret);
            return ret;
        }

        public virtual string Fatal(string mes)
        {
            if (!CheckContainsLevel("Fatal")) return "";
            string ret = messageCat(sb, "[Fatal]", CheckMessage(mes), delimita);
            fileope.Write(ret);
            return ret;
        }

        protected virtual string CheckMessage(string s)
        {
            if (s == null) return "null";
            return s;
        }

        protected virtual string messageCat(StringBuilder sb, string kind, string mes, string _delimita)
        {
            string ret;
                sb.Append(kind);
                sb.Append(_delimita);
                sb.Append(mes);
                ret = sb.ToString();
                sb.Clear();

            return ret;
        }

        public bool CheckContainsLevel(string s)
        {
            return loglevel.Contains(s);
        }

        private StringBuilder sb;
        private string delimita;
        private LogLevel loglevel;
        private TextFileOperator fileope;
        private object lockobj = new object();
    }

    public sealed class LogLevel
    {
        public static LogLevel GetInstance => Loglevel;
        private static readonly LogLevel Loglevel = new LogLevel();
        private LogLevel() { }

        public IList<Level> LevelList { get; } = new List<Level>();

        public void SetLevel(ICollection<string> levels)
        {
            if (levels == null)
            {
                throw new ArgumentNullException("level nothing");
            }

            var x = levels.SelectMany(level =>
                LevelEvt.TryParse(level, out Level lv) ? new Level[] {lv} : new Level[] { });
            foreach (var l in x)
            {
                LevelList.Add(l);
                setLevelValues.Add(l.ToString());
            }
        }

        public void Clear()
        {
            LevelList.Clear();
            setLevelValues.Clear();
        }

        public bool Contains(string level)
        {
            return setLevelValues.Contains(level);
        }

        private IList<string> setLevelValues = new List<string>();
    }

    public sealed class TextFileOperator : IDisposable
    {
        public static TextFileOperator GetInstance(string filename)
        {
            if (textfileope == null)
            {
                textfileope=new TextFileOperator(filename);
            }

            if (!alreadyOpened)
            {
                textfileope.open();
            }

            return textfileope;
        }

        private TextFileOperator(string filename)
        {
            if(filename==null) throw new ArgumentNullException("filename is nothing");
            if(string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("filename is blank");
            this.filename = filename;
        }

        public void Write(string str)
        {
            sw.WriteLine(str);
            sw.Flush();
        }

        private void open()
        {
            sw = new StreamWriter(filename, true);
            alreadyOpened = true;
        }

        private void close()
        {
            sw.Close();
            sw.Dispose();
            alreadyOpened = false;
        }

        public void Dispose()
        {
            Disopse(true);
            GC.SuppressFinalize(this);
        }

        private void Disopse(bool disposing)
        {
            if (disposing) { }
            close();    
        }

        ~TextFileOperator()
        {
            Disopse(false);
        }

        private static TextFileOperator textfileope;
        private string filename;
        private StreamWriter sw;
        private static bool alreadyOpened;
    }

    public static class LevelEvt
    {
        public static bool TryParse(string s, out Level wd)
        {
            return Enum.TryParse(s, out wd) && Enum.IsDefined(typeof(Level), wd);
        }
    }

    public interface ILogger : IDisposable
    {
        string Info(string mes);
        string Operation(string mes);
        string Debug(string mes);
        string Trace(string mes);
        string Warn(string mes);
        string Error(string mes);
        string Fatal(string mes);
    }
    public enum Level
    {
        None,
        Info,
        Operation,
        Debug,
        Trace,
        Warn,
        Error,
        Fatal,
    }
}
