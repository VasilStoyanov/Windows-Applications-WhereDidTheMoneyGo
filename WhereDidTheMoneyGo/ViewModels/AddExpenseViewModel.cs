namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class AddExpenseViewModel: BaseViewModel
    {
        private ObservableCollection<CategoryViewModel2> categories;
        private string currentCategory;
        private ObservableCollection<SubCategoryViewModel> subCategories;

        public AddExpenseViewModel()
        {
            this.categories = new ObservableCollection<CategoryViewModel2>();
            this.subCategories = new ObservableCollection<SubCategoryViewModel>();
        }

        public ObservableCollection<CategoryViewModel2> Categories
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

        public string CurrentCategory
        {
            get
            {
                return this.currentCategory;
            }
            set
            {
                this.currentCategory = value;
                NotifyOnPropertyChange("CurrentCategory");
            }
        }

        public ObservableCollection<SubCategoryViewModel> SubCategories
        {
            get
            {
                return this.subCategories;
            }
            set
            {
                this.subCategories = value;
            }
        }
    }
}
