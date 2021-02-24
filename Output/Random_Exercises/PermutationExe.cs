using System;
using System.Collections.Generic;
using System.Text;

namespace Output.Random_Exercises
{
    public class PermutationExe
    {
        public static void RunExercise()
        {
            var per = new PermutationExe();
            Console.WriteLine("Enter a word or a set of characters to permute");
            var word = Console.ReadLine();
            Console.WriteLine("\n PERMUTATIONS OF: " + word + "\n");

            int count = word.Length;
            
            per.Permute(word, 0, count - 1);
        }

        public void Permute(string word, int l, int r)
        {
            if (l == r)
                Console.WriteLine(word);
            else
            {
                for (int i = l; i <= r; i++)
                {
                    word = Swap(word, l, i);  
                    Permute(word, l + 1, r);
                    word = Swap(word, l, i);
                }
            }
        }

        private string Swap(string word, int i, int j)
        {
            char temp;
            char[] charArray = word.ToCharArray();
            temp = charArray[i];
            charArray[i] = charArray[j];
            charArray[j] = temp;

            string s = new string(charArray);
            return s;
        }
    }
}
