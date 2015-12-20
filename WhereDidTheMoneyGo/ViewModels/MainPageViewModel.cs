namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class MainPageViewModel
    {
        private ObservableCollection<CategoryViewModel> categories;

        public MainPageViewModel()
        {
            this.categories = new ObservableCollection<CategoryViewModel>();
        }

        public ObservableCollection<CategoryViewModel> Categories
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