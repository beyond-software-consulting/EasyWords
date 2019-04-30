using System;
namespace Questions.Helpers
{
    public class ModelAttribute:Attribute
    {
        public ModelAttribute()
        {
        }
        public string CollectionName { get; set; }
    }
}
