namespace WhereDidTheMoneyGo.ViewModels
{
    using System.Collections.ObjectModel;

    public class CategoryViewModel
    {
        private ObservableCollection<SubCategoryViewModel> subCategoris;
        private bool isSelected;

        public CategoryViewModel()
        {
            this.subCategoris = new ObservableCollection<SubCategoryViewModel>();
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public double Amount { get; set; }

        public ObservableCollection<SubCategoryViewModel> SubCategories
        {
            get
            {
                return this.subCategoris;
            }
            set
            {
                this.subCategoris = value;
            }
        }
    }
}