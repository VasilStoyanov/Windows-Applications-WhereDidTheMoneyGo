using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using WhereDidTheMoneyGo.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhereDidTheMoneyGo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewExpence : Page
    {
        public AddNewExpence()
        {
            this.InitializeComponent();
            this.InitAsync();


            this.newCategory.Visibility = Visibility.Collapsed;
        }

        private void OnCreateNewCategoryClick(object sender, RoutedEventArgs e)
        {
            if (this.newCategory.Visibility == Visibility.Collapsed)
            {
                this.newCategory.Visibility = Visibility.Visible;
            }
            else
            {
                this.newCategory.Visibility = Visibility.Collapsed;
            }
        }

        private SQLiteAsyncConnection GetDbConnectionAsync()
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

        private async void InitAsync()
        {
            var connection = this.GetDbConnectionAsync();
            await connection.CreateTableAsync<SubCategory>();
            await connection.CreateTableAsync<Category>();
            await connection.CreateTableAsync<Expense>();

            var numberOfCategories = (await this.GetAllCategoriesAsync()).Count;

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

        private async Task<int> InsertExpenceAsync(Expense item)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.InsertAsync(item);
            return result;
        }

        private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            var connection = this.GetDbConnectionAsync();

            var amount = 0.0;
            double.TryParse(this.tbAmount.Text, out amount);
            var date = this.dpDate.Date.UtcDateTime;
            var category = await this.GetCategoriesAsync(this.tbCategory.Text);
            var subCategory = await this.GetSubCategoriesAsync(this.tbSubCategory.Text);

            var item = new Expense
            {
                CategoryId = category.Id,
                SubCategoryId = subCategory.Id,
                Date = date,
                Amount = amount,
                Description = this.tbDescription.Text,
                ImgUrl = this.tbImageUrl.Text
            };

            await this.InsertExpenceAsync(item);

            // Shows the entered data - to be deleted
            var userData = await this.GetAllExpensesAsync();
            var userDataAsString = new StringBuilder();
            foreach (var userItem in userData)
            {
                userDataAsString.AppendLine(userItem.ToString());
            }

            this.tbExpenseLIst.Text = userDataAsString.ToString();
        }

        private async Task<List<Category>> GetAllCategoriesAsync()
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<Category>().ToListAsync();
            return result;
        }

        private async Task<Category> GetCategoriesAsync(string categoryName)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<Category>()
                                        .Where(x => x.Name == categoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        private async Task<SubCategory> GetSubCategoriesAsync(string subCategoryName)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<SubCategory>()
                                        .Where(x => x.Name == subCategoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        private async Task<List<Expense>> GetAllExpensesAsync()
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.GetAllWithChildrenAsync<Expense>();
            return result;
        }
    }
}
