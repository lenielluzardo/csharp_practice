using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Output.Algorithms_DataStructure
{
   public class DoublyLinkedList<T> : ICollection<T>
   {
      public DoublyLinkedList(T value)
      {
         Value = value;

      }
      public T Value { get; set; }
      public DoublyLinkedListNode<T> Tail { get; private set; }
      public DoublyLinkedListNode<T> Head { get; private set; }
      public int Count { get; private set; }

      public bool IsReadOnly => false;

      public static void Exe()
      {
         var node1 = new DoublyLinkedListNode<int>(1);
         var node2 = new DoublyLinkedListNode<int>(2);
         var node3 = new DoublyLinkedListNode<int>(3);

         node1.Next = node2;

         node2.Previous = node1;
         node2.Next = node3;

         node3.Previous = node2;

      }
      public bool GetHead(out T value)
      {
         if (Count > 0)
         {
            value = Head.Value;
            return true;
         }

         value = default(T);
         return false;
      }

      public void Clear()
      {
         Head = null;
         Tail = null;
         Count = 0;
      }
      private static void Iterate(DoublyLinkedListNode<T> list)
      {
         Console.WriteLine("How do you want iterate the list?\n");
         Console.WriteLine("PRESS F for Forward\n");
         Console.WriteLine("PRESS B for Forward\n");

         var input = Console.ReadLine();

         if (input.ToLower().Equals("f"))
            IterateForward(list);
         else if (input.ToLower().Equals("b"))
            IterateBackward(list);
         else
            Console.WriteLine("Incorrect option \n");
      }
      private static void IterateForward(DoublyLinkedListNode<T> list)
      {
         if (list == null)
         {
            Console.WriteLine("No more nodes in the list, you are on the tail of the list");
            return;
         }

         Console.WriteLine("Node at Position:.... {0}", list.Value);

         IterateForward(list.Next);
      }

      private static void IterateBackward(DoublyLinkedListNode<T> list)
      {
         if (list.Previous == null)
         {
            Console.WriteLine("No more nodes in the list, you are on the head of the list");
            return;
         }

         Console.WriteLine("Node at Position:.... {0}", list.Value);
         IterateBackward(list.Previous);
      }

      public void AddHead(T value)
      {
         AddHead(new DoublyLinkedListNode<T>(value));
      }
      public void AddHead(DoublyLinkedListNode<T> node)
      {
         var temp = Head;
         Head = node;

         Head.Next = temp;

         if(Count == 0)
         {
            Tail = Head;
         }
         else
         {
            temp.Previous = Head;
         }

         Count++;

      }
      public void AddTail(T value)
      {
         AddTail(new DoublyLinkedListNode<T>(value));
      }
      public void AddTail(DoublyLinkedListNode<T> node)
      {
         if(Count == 0)
         {
            Head = node;
         }
         else
         {
            Tail.Next = node;
            node.Previous = Tail;
         }
         Tail = node;
         Count++;
      }

      public void RemoveHead()
      {
         if(Count != 0)
         {
            Head = Head.Next;
            Count--;
            if(Count == 0)
            {
               Tail = null;
            }
            else
            {
               Head.Previous = null;
            }
         }
      }

      public void RemoveTail()
      {
         if(Count != 0)
         {
            if(Count == 1)
            {
               Head = null;
               Tail = null;
            }
            else
            {
               Tail.Previous.Next = null;
               Tail = Tail.Previous;
            }
            Count--;
         }
      }
      public DoublyLinkedListNode<T> Find(T item)
      {
         DoublyLinkedListNode<T> current = Head;
         while(current != null)
         {
            if (current.Value.Equals(item))
            {
               return current;
            }

            current = current.Next;
         }

         return null;
      }

      public void Add(T item)
      {
         AddHead(item);
      }


      public bool Contains(T item)
      {
         return Find(item) != null;
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
         DoublyLinkedListNode<T> current = Head;
         while(current != null)
         {
            array[arrayIndex++] = current.Value;
            current = current.Next;
         }
      }

      public bool Remove(T item)
      {
         DoublyLinkedListNode<T> found = Find(item);
         if(found == null)
         {
            return false;
         }
         DoublyLinkedListNode<T> previous = found.Previous;
         DoublyLinkedListNode<T> next = found.Next;

         if(previous == null)
         {
            Head = next;
            if(Head != null)
            {
               Head.Previous = null;
            }
         }
         else
         {
            previous.Next = next;
         }

         if(next == null)
         {
            Tail = previous;
            if(Tail != null)
            {
               Tail.Next = null;
            }
         }
         else
         {
            next.Previous = previous;
         }
         Count--;
         return true;
      }

      public IEnumerator<T> GetEnumerator()
      {
         DoublyLinkedListNode<T> current = Head;
         while(current != null)
         {
            yield return current.Value;
            current = current.Next;
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   
      public IEnumerable<T> GetReverseEnumerator()
      {
         DoublyLinkedListNode<T> current = Tail;
         while(current != null)
         {
            yield return current.Value;
            current = current.Previous;
         }

      }
   
   }

   public class DoublyLinkedListNode<T>
   {
      public DoublyLinkedListNode(T value)
      {
         Value = value;
      }
      public T Value { get; set; }
      public DoublyLinkedListNode<T> Previous { get; set; }
      public DoublyLinkedListNode<T> Next { get; set; }
   }
}
