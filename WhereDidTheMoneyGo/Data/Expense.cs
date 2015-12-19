namespace WhereDidTheMoneyGo.Data
{
    using System;
    using SQLite.Net.Attributes;
    using SQLiteNetExtensions.Attributes;

    public class Expense
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        [ManyToOne]
        public Category Category { get; set; }

        [ForeignKey(typeof(SubCategory))]
        public int SubCategoryId { get; set; }

        [ManyToOne]
        public SubCategory SubCategory { get; set; }

        public double Amount { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public string ImgUrl { get; set; }

        public override string ToString()
        {
            return $"Cat: {this.Category.Name}, SubCat: {this.SubCategory.Name}, Desc: {this.Description}, Amount: {this.Amount}";
        }
    }
}
