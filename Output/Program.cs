using Output.Algorithms_DataStructure;
using System;
using System.Collections.Generic;

namespace Output
{
   class Program
   {
      static void Main(string[] args)
      {
         //PalindromeExe.RunExercise();
         //FizzBuzzExe.RunExercise();
         //SinglyLinkedList.Exe();
         DoublyLinkedList.Exe();



         Console.Read();
      }
   }
   public class X
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
   public class Y : X
   {
      public static void staticMethod()
      {
         Console.WriteLine("Class Y");

      }
   }
}
