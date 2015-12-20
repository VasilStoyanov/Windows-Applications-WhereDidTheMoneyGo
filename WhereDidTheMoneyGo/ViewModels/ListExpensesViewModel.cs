namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

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