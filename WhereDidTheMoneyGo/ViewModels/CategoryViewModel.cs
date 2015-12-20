using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WhereDidTheMoneyGo.ViewModels
{
    public class CategoryViewModel
    {
        private ObservableCollection<SubCategoryViewModel> subCategoris;

        public CategoryViewModel()
        {
            this.subCategoris = new ObservableCollection<SubCategoryViewModel>();
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
