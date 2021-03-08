using System;
using ZZ_Common.Interfaces;

namespace Json
{
   public class JsonProcessorService : IJsonProcessorService
   {
      public void Run()
      {
         string singleJson = Generate.SingleJson();
      }
   }
}
