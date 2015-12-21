namespace WhereDidTheMoneyGo.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using DataModels;
    using SQLite.Net;
    using SQLite.Net.Async;
    using SQLite.Net.Platform.WinRT;
    using SQLiteNetExtensionsAsync.Extensions;
    using ViewModels;
    using Windows.Storage;
    using System.Threading.Tasks;

    public static class Database
    {
        public static AddExpenseViewModel ViewModel { get; set; }

        public static SQLiteAsyncConnection GetDbConnectionAsync()
        {
            var dbFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            var connectionFactory =
                new Func<SQLiteConnectionWithLock>(
                    () =>
                    new SQLiteConnectionWithLock(
                        new SQLitePlatformWinRT(),
                        new SQLiteConnectionString(dbFilePath, storeDateTimeAsTicks: false)));

            var asyncConnection = new SQLiteAsyncConnection(connectionFactory);

            return asyncConnection;
        }

        public static async void GetAllData(int month, int year, string selectedCategory)
        {
            var connection = GetDbConnectionAsync();

            var categories = await connection.GetAllWithChildrenAsync<Category>(null, true);

            foreach (var category in categories)
            {
                var categorySum = 0.0;
                foreach (var expense in category.Expenses)
                {
                    if (expense.Date.Year == year && expense.Date.Month == month)
                    {
                        categorySum += expense.Amount;
                    }
                }


                var newCategoryViewModel = new CategoryViewModel() { Name = category.Name, Amount = categorySum };

                if (category.Name == selectedCategory)
                {
                    var newSubCategoryViewModel = new SubCategoryViewModel();
                    category.SubCategories.ForEach(
                        s => newCategoryViewModel.SubCategories.Add(
                                                                new SubCategoryViewModel
                                                                {
                                                                    Name = s.Name,
                                                                    Amount = GetExpenses(s.Expenses, month, year)
                                                                })
                    );
                }

                ViewModel.Categories.Add(newCategoryViewModel);
            }
        }

        public static async void GetSubCategoriesOnly(int month, int year, string selectedCategory)
        {
            var connection = GetDbConnectionAsync();

            var subCategories = await connection.GetAllWithChildrenAsync<SubCategory>(null, true);

            foreach (var subCategory in subCategories)
            {

                if (subCategory.Category.Name == selectedCategory)
                {
                    var newSubCategoryViewModel = new SubCategoryViewModel() { Name = subCategory.Name };

                    foreach (var expense in subCategory.Expenses)
                    {
                        newSubCategoryViewModel.Amount += expense.Amount;
                    }


                    var categoriesCount = ViewModel.Categories.Count;
                    for (int i = 0; i < categoriesCount; i++)
                    {
                        if (ViewModel.Categories[i].Name == selectedCategory)
                        {
                            ViewModel.Categories[i].SubCategories.Add(newSubCategoryViewModel);
                        }
                    }
                }
            }

        }

        public static double GetExpenses(List<Expense> expenses, int month, int year)
        {
            var amount = 0.0;
            if (expenses == null)
            {
                return amount;
            }

            foreach (var expense in expenses)
            {
                if (expense.Date.Year == year && expense.Date.Month == month)
                {
                    amount += expense.Amount;
                }
            }
            return amount;
        }

        public static async void InitAsync()
        {
            var connection = GetDbConnectionAsync();
            await connection.CreateTableAsync<SubCategory>();
            await connection.CreateTableAsync<Category>();
            await connection.CreateTableAsync<Expense>();

            var numberOfCategories = (await GetAllCategoriesAsync()).Count;

            if (numberOfCategories == 0)
            {
                // Insert all categories
                var categoryFood = new Category() { Name = "Food" };
                var categoryHousing = new Category() { Name = "Housing" };
                var categoryPersonal = new Category() { Name = "Personal" };
                var categoryTransportation = new Category() { Name = "Transportation" };

                var categories = new List<Category>()
                                {
                                    categoryFood,
                                    categoryHousing,
                                    categoryPersonal,
                                    categoryTransportation
                                };
                await connection.InsertAllAsync(categories);

                // Insert Food sub-categories
                var subCategoriesFood = new List<SubCategory>()
                                {
                                    new SubCategory() { Name = "Groceries" },
                                    new SubCategory() { Name = "Restaurants" },
                                    new SubCategory() { Name = "Lunch/Snacks" },
                                };
                await connection.InsertAllAsync(subCategoriesFood);
                categoryFood.SubCategories = new List<SubCategory>();
                foreach (var item in subCategoriesFood)
                {
                    categoryFood.SubCategories.Add(item);
                }
                await connection.UpdateWithChildrenAsync(categoryFood);

                // Insert Housing sub-categories
                var subCategoriesHousing = new List<SubCategory>()
                                {
                                    new SubCategory() { Name = "Mortgage/Rent" },
                                    new SubCategory() { Name = "Electricity" },
                                    new SubCategory() { Name = "Garage" },
                                    new SubCategory() { Name = "Water" },
                                    new SubCategory() { Name = "TV / Internet" },
                                    new SubCategory() { Name = "Heating" },
                                    new SubCategory() { Name = "Maintenance & Repairs" }
                                };
                await connection.InsertAllAsync(subCategoriesHousing);
                categoryHousing.SubCategories = new List<SubCategory>();
                foreach (var item in subCategoriesHousing)
                {
                    categoryHousing.SubCategories.Add(item);
                }
                await connection.UpdateWithChildrenAsync(categoryHousing);

                // Insert Personal sub-categories
                var subCategoriesPersonal = new List<SubCategory>()
                                {
                                    new SubCategory() { Name = "Health" },
                                    new SubCategory() { Name = "Beauty" },
                                    new SubCategory() { Name = "Clothes" },
                                    new SubCategory() { Name = "Gadgets" },
                                    new SubCategory() { Name = "Entertainment" },
                                    new SubCategory() { Name = "Credit card Payment" },
                                    new SubCategory() { Name = "Gifts" },
                                    new SubCategory() { Name = "Vacation" },
                                    new SubCategory() { Name = "Education" },
                                    new SubCategory() { Name = "Miscellaneous" },
                                    new SubCategory() { Name = "Car Payment" }
                                };
                await connection.InsertAllAsync(subCategoriesPersonal);
                categoryPersonal.SubCategories = new List<SubCategory>();
                foreach (var item in subCategoriesPersonal)
                {
                    categoryPersonal.SubCategories.Add(item);
                }
                await connection.UpdateWithChildrenAsync(categoryPersonal);

                // Insert Transportation sub-categories
                var subCategoriesTransportation = new List<SubCategory>()
                                {
                                    new SubCategory() { Name = "Petrol" },
                                    new SubCategory() { Name = "Car Insurance" },
                                    new SubCategory() { Name = "Repairs & Maintenance" }
                                };
                await connection.InsertAllAsync(subCategoriesTransportation);
                categoryTransportation.SubCategories = new List<SubCategory>();
                foreach (var item in subCategoriesTransportation)
                {
                    categoryTransportation.SubCategories.Add(item);
                }
                await connection.UpdateWithChildrenAsync(categoryTransportation);
            }
        }

        public static async Task<List<Category>> GetAllCategoriesAsync()
        {
            var connection = GetDbConnectionAsync();
            var result = await connection.Table<Category>().ToListAsync();

            return result;
        }

        public static async void PopulateCategoriesAsync()
        {
            var connection = GetDbConnectionAsync();
            var allCategories = await connection.Table<Category>().ToListAsync();

            foreach (var category in allCategories)
            {
                var newCategoryViewModel = new CategoryViewModel { Name = category.Name };
                ViewModel.Categories.Add(newCategoryViewModel);
            }
        }

        public static async Task<Category> GetCategoriesAsync(string categoryName)
        {
            var connection = GetDbConnectionAsync();
            var result = await connection.Table<Category>()
                                        .Where(x => x.Name == categoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        public static async Task<SubCategory> GetSubCategoriesAsync(string subCategoryName)
        {
            var connection = GetDbConnectionAsync();

            var result = await connection.Table<SubCategory>()
                                        .Where(x => x.Name == subCategoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        public static async Task<List<SubCategory>> GetSubCategoriesByIdAsync(int categoryId)
        {
            var connection = GetDbConnectionAsync();
            var result = await connection.Table<SubCategory>()
                                        .Where(x => x.CategoryId == categoryId)
                                        .ToListAsync();
            return result;
        }

        public static async Task<int> InsertExpenceAsync(Expense item)
        {
            var connection = GetDbConnectionAsync();
            var result = await connection.InsertAsync(item);
            return result;
        }
    }
}
