using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ZZ_Common.Interfaces;

namespace Regular_Expressions
{
   public class RegularExpressionService : IRegexService
   {
      private List<string> _patterns;
      private List<string> _inputs;
      public void Run()
      {
         //MatchQuantifiers();
         //SplitStrings();
         GroupingAndSustituying();
      }
      private void MatchQuantifiers()
      {
         _patterns = new List<string> { "a*b", "a+b", "a?b" }; ;
         _inputs = new List<string> { "a", "b", "ab", "aab", "abb" };

         foreach (var pattern in _patterns)
         {
            Console.WriteLine("Regular Expressions: {0}", pattern);

            var regex = new Regex(pattern);

            foreach (var input in _inputs)
            {
               Console.WriteLine("\n\tInput pattern: {0}", input);

               var results = regex.Matches(input);

               if (results.Count <= 0)
               {
                  Console.WriteLine("\t\tNo matches found");
               }

               foreach (Match result in results)
               {
                  Console.WriteLine(" \t\tMatch found at index {0}. Length {1}.", result.Index, result.Length);
               }
            }
         }
      }
      private void SplitStrings()
      {
         _patterns = new List<string> { @"^\d\d\d-\d\d\d-\d\d\d\d$" };
         _inputs = new List<string>
         {
            "5555555555555",
            "(555)-555-5555",
            "012-345-6789",
            "555-555-5555",
            "555-555-555a",
            "5555-555-5555",
            "000-000-0000",
            "a",
            "5.55-555-5555",
            "...-...-...."
         };

         foreach (var pattern in _patterns)
         {
            Console.WriteLine("Regular Expressions: {0}", pattern);

            var regex = new Regex(pattern);

            foreach (var input in _inputs)
            {
               Console.WriteLine("\tInput pattern: {0}", input);
               var isMatch = regex.IsMatch(input);

               Console.Write("\t\t{0}\n", isMatch ? "Accepted" : "Rejected");

               if (!isMatch)
                  continue;

               var splits = Regex.Split(input, @"-\d\d\d-").ToList();
               Console.WriteLine("\t\t\tArea code: {0}", splits[0]);
               Console.WriteLine("\t\t\tLast 4 Digits: {0}", splits[1]);
            }
         }
      }
      private void GroupingAndSustituying()
      {
         _patterns = new List<string> { @"([A-Za-z]+).*\$(\d+.\d+)" };
         _inputs = new List<string>
         {
            @"|------------------------|
              | Receipt from           |
              | Alexandru's Shop       |
              |                        |
              | Thanks for shopping!   |
              |---------|--------------|
              | Item    |   Price $USD |
              |---------|--------------|
              | Shoes   |       $47.99 |
              | Cabbage |        $2.99 |
              | Carrots |        $1.23 |
              | Chicken |        $9.99 |
              | Beef    |       $12.99 |
              | Shirt   |        $5.99 |
              | Salt    |        $2.99 |
              |---------|--------------|"
         };


         foreach (var pattern in _patterns)
         {
            Console.WriteLine("Regular Expressions: {0}", pattern);

            var regex = new Regex(pattern);

            foreach (var input in _inputs)
            {
               Console.WriteLine("\tInput pattern:");
               Console.WriteLine("\t\t{0}", input);
               var matches = regex.Matches(input);

               if (matches.Count <= 0)
               {
                  Console.WriteLine("\t\tNo matches found.");
               }
               foreach(Match match in matches)
               {
                  Console.WriteLine("\t\tMatch at index {0} with length {1}", match.Index, match.Length);
                  
                  foreach(Group group in match.Groups)
                  {
                     Console.WriteLine("\t\tGroup at index {0} has value {1}", group.Index, group.Value);
                  }
               }
               Console.WriteLine("Simple replacement results: {0}",
                  Regex.Replace(input, @"(Chicken)(.*) \$(9.99)", @"$1$2 $$0.00"));

               var results = Regex.Replace(input, pattern, (match) =>
               {
                  if (match.Groups[1].Value == "Chicken")
                  {
                     return match.Value.Replace(match.Groups[2].Value, "0.00");
                  }

                  return match.Value;
               });
               Console.WriteLine("Advanced replacement results: {0}", results);
            }
         }
      }
   }
}
