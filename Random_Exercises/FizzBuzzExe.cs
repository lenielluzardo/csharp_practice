using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZZ_Common.Interfaces;

namespace Random_Exercises
{
   public class FizzBuzzExe : IRandomExercisesService
   {
      public void Run(string[] args = null)
      {
         for (int i = 1; i <= 100; i++)
         {
            if (i % 3 == 0 && i % 5 == 0)
            {
               Console.WriteLine(i + " Fizz Buzz");
            }
            else if (i % 5 == 0)
            {
               Console.WriteLine(i + " Buzz");
            }
            else if (i % 3 == 0)
            {
               Console.WriteLine(i + " Fizz");
            }
            else
            {
               Console.WriteLine(i);
            }
         }
      }
   }
}
