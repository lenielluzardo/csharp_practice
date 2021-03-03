using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZZ_Common.Interfaces;

namespace Random_Exercises
{
   public class PalindromeExe : IRandomExercisesService
   {
      public void Run(string[] args = null)
      {
         var palindromeExe = new PalindromeExe();

         Console.WriteLine("Write a word to check if is a Palindrome:....");
         var input = Console.ReadLine();

         var isPalindrome = palindromeExe.CheckPalindrome(input);

         if (isPalindrome)
            Console.WriteLine("The word is a Palindrome");
         else
            Console.WriteLine("The word is not a Palindrome");
      }
      private bool CheckPalindrome(string word)
      {
         char[] wordArray = word.ToCharArray();
         Array.Reverse(wordArray);

         string reverseWord = new string(wordArray);

         if (word.Equals(reverseWord, StringComparison.OrdinalIgnoreCase))
            return true;
         else
            return false;
      }

   }
}
