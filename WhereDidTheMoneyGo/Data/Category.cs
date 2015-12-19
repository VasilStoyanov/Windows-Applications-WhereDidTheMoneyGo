namespace WhereDidTheMoneyGo.Data
{
    using SQLite.Net.Attributes;

    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public string SubCategoryName { get; set; }
    }
}
