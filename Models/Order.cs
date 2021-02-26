using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
   public class Order
   {
      public int OrderNumber { get; set; }
      public int CustomerNumber { get; set; }
      public string Description { get; set; }
      public int Quantity { get; set; }
   }
}
