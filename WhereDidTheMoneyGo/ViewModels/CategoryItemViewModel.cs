namespace WhereDidTheMoneyGo.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class CategoryItemViewModel
    {
        private ObservableCollection<SubCategoryItemViewModel> subCategories;
        
        public CategoryItemViewModel()
        {
            this.subCategories = new ObservableCollection<SubCategoryItemViewModel>();
        }

        public CategoryViewModel Category { get; set; }

        public double Amount { get; set; }

        public IEnumerable<SubCategoryItemViewModel> SubCategories { get; set; }
    }
}
