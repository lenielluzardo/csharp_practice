using System;
using System.Collections.Generic;
using System.Text;

namespace Output.Random_Exercises
{
   public class RecursionExe
   {
      public static List<string> staticMethod(int n, List<string> result = null)
      {
         result = result != null ? result : new List<string>();
         if (n == 0)
         {
            result.Add("No more bottles");
            return result;
         }
         string str = n + "Bottles";
         result.Add(str);
         return staticMethod(n - 1, result);
      }
   }
   public class Y : RecursionExe
   {
      public static void staticMethod()
      {
         Console.WriteLine("Class Y");

      }
   }
}
