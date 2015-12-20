namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Linq;

    public class ListExpensesParametersViewModel
    {
        public string SelectedCategory { get; set; }

        public int SelectedMonth { get; set; }

        public int SelectedYear { get; set; }
    }
}
