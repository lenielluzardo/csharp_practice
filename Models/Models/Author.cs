using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ZZ_Common.Models
{
    [JsonObject(IsReference = true)]
    public class Author
    {
        public string name { get; set; }
        public string[] courses { get; set; }
        public DateTime since { get; set; }
        public bool happy { get; set; }
        public object issues { get; set; }
        public Car car { get; set; }
        public List<Author> favoriteAuthors { get; set; }
        public AuthorRelationship authorRelationship { get; set; }

    }
}
