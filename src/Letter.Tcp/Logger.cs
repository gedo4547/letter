using System;


 
 public static class Logger
 {
     public static bool ConsoleEnabled = true;

     public static void Setting(Action<string> info, Action<string> warn, Action<string> error, Action<string> debug,
         Action<string> exception)
     {
         Logger.infoFunc = info;
         Logger.warnFunc = warn;
         Logger.errorFunc = error;
         Logger.debugFunc = debug;
         Logger.exceptionFunc = exception;
     }

     private static Action<string> infoFunc = (str) => { Console.WriteLine($"info>>{str}"); };
     private static Action<string> warnFunc = (str) => { Console.WriteLine($"warn>>{str}"); };
     private static Action<string> errorFunc = (str) => { Console.WriteLine($"error>>{str}"); };
     private static Action<string> debugFunc = (str) => { Console.WriteLine($"debug>>{str}"); };
     private static Action<string> exceptionFunc = (str) => { Console.WriteLine($"exc>>{str}"); };


     public static void Info(object text)
     {
         if (Logger.ConsoleEnabled)
         {
             if (Logger.infoFunc != null) Logger.infoFunc(text.ToString());
         }
     }

     public static void Exception(object text)
     {
         string tempText = $"{text}\n{Logger.GetLoggerStackTrace()}";
         if (Logger.ConsoleEnabled)
         {
             if (Logger.exceptionFunc != null) Logger.exceptionFunc(tempText);
         }
     }


     public static void Warn(object text)
     {
         string tempText = $"{text}\n{Logger.GetLoggerStackTrace()}";
         if (Logger.ConsoleEnabled)
         {
             if (Logger.warnFunc != null) Logger.warnFunc(tempText);
         }
     }

     public static void Error(object text)
     {
         string tempText = $"{text}\n{Logger.GetLoggerStackTrace()}";
         if (Logger.ConsoleEnabled)
         {
             if (Logger.errorFunc != null) Logger.errorFunc(tempText);
         }
     }

     public static void Debug(object text)
     {
         string tempText = $"{text}\n{Logger.GetLoggerStackTrace()}";
         if (Logger.ConsoleEnabled)
         {
             if (Logger.debugFunc != null) Logger.debugFunc(tempText);
         }
     }



     private static string GetLoggerStackTrace(bool isInfo = false)
     {
         System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
         System.Diagnostics.StackFrame[] sfs = st.GetFrames();
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         int tempIndex = 0;
         foreach (var item in sfs)
         {
             tempIndex++;
             if (item.GetMethod().DeclaringType.FullName == "Sugar.Logger") continue;
             sb.Append("    ");
             sb.Append($"{item.GetMethod().DeclaringType.FullName}:{item.GetMethod().Name}");
             sb.Append("(");
             System.Reflection.ParameterInfo tempParameterInfo;
             System.Reflection.ParameterInfo[] parameters = item.GetMethod().GetParameters();
             for (int i = 0; i < parameters.Length; i++)
             {
                 tempParameterInfo = parameters[i];
                 sb.Append($"{tempParameterInfo.ParameterType} {tempParameterInfo.Name}");
                 if (i < parameters.Length - 1)
                 {
                     sb.Append(",");
                 }
             }

             sb.Append(")");
             sb.Append($" at ({item.GetFileName()}:{item.GetFileLineNumber()})");
             if (tempIndex != sfs.Length) sb.AppendLine();
             //if (isInfo)
             //{
             //    if (tempIndex == 1)
             //    {
             //        break;
             //    }
             //}
         }

         return sb.ToString();
     }
 }
 
