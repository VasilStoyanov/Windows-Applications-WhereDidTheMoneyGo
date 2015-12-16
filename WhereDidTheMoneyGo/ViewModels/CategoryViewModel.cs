namespace WhereDidTheMoneyGo.ViewModels
{
    using System.Collections.Generic;

    public class CategoryViewModel
    {
        public string Name { get; set; }

        public ICollection<CategoryItemsViewModel> Items { get; set; }
    }
}
