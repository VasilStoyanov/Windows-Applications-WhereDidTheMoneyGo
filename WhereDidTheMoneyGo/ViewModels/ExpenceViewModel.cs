namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Linq;

    public class ExpenceViewModel
    {
        public DateTime Date { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public string ImgUrl { get; set; }
    }
}