namespace WhereDidTheMoneyGo.DataModels
{
    using System.Collections.Generic;
    using SQLite.Net.Attributes;
    using SQLiteNetExtensions.Attributes;

    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<SubCategory> SubCategories { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Expense> Expenses { get; set; }
    }
}
