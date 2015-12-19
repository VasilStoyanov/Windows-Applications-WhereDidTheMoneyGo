namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class CategoryViewModel
    {
        private ICollection<CategoryItemViewModel> categories;

        public CategoryViewModel()
        {
            this.Categories = new ObservableCollection<CategoryItemViewModel>();
        }

        public ICollection<CategoryItemViewModel> Categories
        {
            get
            {
                return this.categories;
            }
            set
            {
                this.categories = value;
            }
        }


    }
}
