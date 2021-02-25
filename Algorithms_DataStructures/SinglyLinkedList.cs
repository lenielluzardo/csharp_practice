using System;
using System.Collections.Generic;
using System.Text;

namespace Files_Streams.Algorithms_DataStructure
{
  public class SinglyLinkedList
  {
    public SinglyLinkedList(int value)
    {
      Value = value;
    }
    public int Value { get; set; }
    public SinglyLinkedList Next { get; set; }

    public static void Exe()
    {
      var head = new SinglyLinkedList(1);
      head.Next = new SinglyLinkedList(2);
      head.Next.Next = new SinglyLinkedList(3);

      IterateList(head);
    }

    private static void IterateList(SinglyLinkedList list)
    {
         if (list == null)
         {
            Console.WriteLine("No more nodes in the list");
            return;
         }

         Console.WriteLine("Node at Position:.... {0}", list.Value);
      
         IterateList(list.Next);
    }

  }
}
