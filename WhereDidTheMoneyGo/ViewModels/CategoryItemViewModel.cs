namespace WhereDidTheMoneyGo.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class CategoryItemViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SubCategoryItemViewModel> items;

        public event PropertyChangedEventHandler PropertyChanged;

        public CategoryItemViewModel()
        {
            this.items = new ObservableCollection<SubCategoryItemViewModel>();
        }

        public Category Category { get; set; }

        public double Amount { get; set; }

        public IEnumerable<SubCategoryItemViewModel> Items { get; set; }
    }
}
