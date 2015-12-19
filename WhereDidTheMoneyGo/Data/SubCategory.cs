namespace WhereDidTheMoneyGo.Data
{
    using System.Collections.Generic;
    using SQLite.Net.Attributes;
    using SQLiteNetExtensions.Attributes;

    public class SubCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Expense> Expenses { get; set; }

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        [ManyToOne]
        public Category Category { get; set; }
    }
}
