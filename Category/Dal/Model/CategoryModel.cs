using System;

namespace Category.Dal.Model
{
    public class CategoryModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string CreatedOn { get; set; }
    }
}