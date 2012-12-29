using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ProcessUtil
{
    class Program
    {

        static string argumentMessage = "Usage ps -option[l for list,k for kill,-s for suspend, -r for resume] -regex[Expression for the opertion of the process id]";
        static void Main(string[] args)
        {
            try{
                var param = ProcessArgs(args);
                ExecuteOption(param);
            }
            catch(ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ExecuteOption(Param param)
        {
            if (param.PId != 0)
            {
                var process = Process.GetProcessById(param.PId);
                ExecuteOption(param, process);
                return;
            }
            if (param.Expression != null)
            {
                var list = Process.GetProcesses().Where(x => x.ProcessName.IndexOf(param.Expression,StringComparison.OrdinalIgnoreCase)>=0);
                foreach (var process in list)
                    ExecuteOption(param, process);
                return;
            }

        }

        private static void ExecuteOption(Param param, Process process)
        {
            Options option = param.Option;
            switch (option)
            {
                case Options.List:
                    process.Print();
                    break;
                case Options.Kill:
                    process.Kill();
                    break;
                case Options.Suspend:
                    process.Suspend();
                    break;
                case Options.Resume:
                    process.Resume();
                    break;
                default:
                    throw new ArgumentException(argumentMessage);
            }
        }
        
        public static Param ProcessArgs(string[] args)
        {
            Param param;
            string commandLineParam2;
            if(args.Length < 1)
                throw new ArgumentException(argumentMessage);
            var option = ProcessOption(args[0]);
            if(option != Options.List && args.Length < 2)
                throw new ArgumentException(argumentMessage);
            if (args.Length < 2)
                commandLineParam2 = string.Empty;
            else
                commandLineParam2 = args[1];
            param = ProcessParam(commandLineParam2);
            param.Option =option;
            return param;
        }
        public static Param ProcessParam(string rawParam)
        {
            int result;
            var param = new Param();
            if (int.TryParse(rawParam, out result))
            {
                param.PId = result;
            }
            else
            {
                param.PId = 0;
                param.Expression = rawParam;
            }
            return param;
        }
        public static Options ProcessOption(string option)
        {
            Options progOptions;
            switch(option)
            {
                case "-l":
                    progOptions = Options.List;
                break;
                case "-k":
                progOptions = Options.Kill;
                break;
                case "-s":
                progOptions = Options.Suspend;
                break;
                case "-r":
                progOptions = Options.Resume;
                break;
                default:
                    throw new ArgumentException("Param 1 can be -l for list and -k for kill -s for suspend and -r for resume");
            }
            return progOptions;
        }
    }
}
