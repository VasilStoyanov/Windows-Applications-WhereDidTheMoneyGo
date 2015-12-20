using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereDidTheMoneyGo.ViewModels
{
    public class ListExpensesViewModel
    {
        private ObservableCollection<ExpenceViewModel> expenses;

        public ListExpensesViewModel()
        {
            this.expenses = new ObservableCollection<ExpenceViewModel>();
        }

        public ObservableCollection<ExpenceViewModel> Expenses
        {
            get
            {
                return this.expenses;
            }
            set
            {
                this.expenses = value;
            }
        }
    }
}

