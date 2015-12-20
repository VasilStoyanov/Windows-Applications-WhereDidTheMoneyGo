namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class ListExpensesViewModel: BaseViewModel
    {
        private ObservableCollection<ExpenceViewModel> expenses;
        private string selectedParameters;

        public string SelectedParameters
        {
            get
            {
                return this.selectedParameters;
            }
            set
            {
                this.selectedParameters = value;
                NotifyOnPropertyChange("SelectedParameters");
            }
        }

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