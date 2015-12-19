namespace WhereDidTheMoneyGo.Data
{
    using System;
    using SQLite.Net.Attributes;

    public class Expense
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public double Amount { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }

        public string ImgUrl { get; set; }

        public override string ToString()
        {
            return $"Cat: {this.Category}; Subcat: {this.SubCategory}; Descr: {this.Description}; Amount: {this.Amount}";
        }
    }
}
