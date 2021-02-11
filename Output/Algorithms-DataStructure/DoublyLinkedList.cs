using System;
using System.Collections.Generic;
using System.Text;

namespace Output.Algorithms_DataStructure
{
   public class DoublyLinkedList
   {
      public DoublyLinkedList(int value)
      {
         Value = value;

      }
      public int Value { get; set; }
      public DoublyLinkedList Previous { get; set; }
      public DoublyLinkedList Next { get; set; }

      public static void Exe()
      {
         var node1 = new DoublyLinkedList(1);
         var node2 = new DoublyLinkedList(2);
         var node3 = new DoublyLinkedList(3);

         node1.Next = node2;

         node2.Previous = node1;
         node2.Next = node3;

         node3.Previous = node2;

         Iterate(node1);
      }

      private static void Iterate(DoublyLinkedList list)
      {
         Console.WriteLine("How do you want iterate the list?\n");
         Console.WriteLine("PRESS F for Forward\n");
         Console.WriteLine("PRESS B for Forward\n");
         
         var input = Console.ReadLine();

         if (input.ToLower().Equals("f"))
            IterateForward(list);
         else if(input.ToLower().Equals("b"))
            IterateBackward(list);
         else
            Console.WriteLine("Incorrect option \n");
      }
      private static void IterateForward(DoublyLinkedList list)
      {
         if (list == null)
         {
            Console.WriteLine("No more nodes in the list, you are on the tail of the list");
            return;
         }

         Console.WriteLine("Node at Position:.... {0}", list.Value);

         IterateForward(list.Next);
      }

      private static void IterateBackward(DoublyLinkedList list)
      {
         if (list.Previous == null)
         {
            Console.WriteLine("No more nodes in the list, you are on the head of the list");
            return;
         }

         Console.WriteLine("Node at Position:.... {0}", list.Value);
         IterateBackward(list.Previous);
      }
   }
}
