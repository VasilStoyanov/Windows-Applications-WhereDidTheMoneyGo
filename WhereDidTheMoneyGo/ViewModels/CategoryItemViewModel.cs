namespace WhereDidTheMoneyGo.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class CategoryItemViewModel
    {
        private ObservableCollection<SubCategoryItemViewModel> items;

        public CategoryItemViewModel()
        {
            this.items = new ObservableCollection<SubCategoryItemViewModel>();
        }

        public Category Category { get; set; }

        public double Amount { get; set; }

        public IEnumerable<SubCategoryItemViewModel> Items { get; set; }
    }
}
