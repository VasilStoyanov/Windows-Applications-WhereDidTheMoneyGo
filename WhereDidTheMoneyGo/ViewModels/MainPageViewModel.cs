using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereDidTheMoneyGo.ViewModels
{
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
