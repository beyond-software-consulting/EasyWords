using System;
using System.ComponentModel;

namespace Questions.Helpers
{
    public class ModelAttribute:Attribute
    {
        public ModelAttribute()
        {
            IgnoreIdentitySeed = false;
        }
        public string CollectionName { get; set; }
        [DefaultValue(false)]
        public bool IgnoreIdentitySeed { get; set; }
    }
}
