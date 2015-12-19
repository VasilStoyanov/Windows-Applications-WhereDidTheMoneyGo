using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
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
            await connection.CreateTableAsync<Expense>();
            await connection.CreateTableAsync<Category>();

            var numberOfCategories = (await this.GetAllCategoriesAsync()).Count;

            if (numberOfCategories == 0)
            {
                var categories = new List<Category>()
                                            { new Category() { CategoryName = "Food", SubCategoryName="Groceries" },
                                            new Category() { CategoryName = "Food", SubCategoryName="Restaurants" },
                                            new Category() { CategoryName = "Food", SubCategoryName="Lunch/Snacks" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="Mortgage/Rent" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="Electricity" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="Garage" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="Water" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="TV / Internet" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="Heating" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="Maintenance & Repairs" },
                                            new Category() { CategoryName = "Housing", SubCategoryName="House service fee" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Health" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Beauty" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Clothes" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Gadgets" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Entertainment" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Credit card Payment" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Gifts" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Vacation" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Education" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Miscellaneous" },
                                            new Category() { CategoryName = "Personal", SubCategoryName="Car Payment" },
                                            new Category() { CategoryName = "Transportation", SubCategoryName="Petrol" },
                                            new Category() { CategoryName = "Transportation", SubCategoryName="Car Insurance" },
                                            new Category() { CategoryName = "Transportation", SubCategoryName="Repairs & Maintenance" },
                                            };
                await connection.InsertAllAsync(categories);
            }

            //string createTableExpenses = "CREATE TABLE Expense2(Id integer primary key autoincrement not null ,Date datetime ,CategoryId integer ,SubCategory varchar ,Amount float ,Note varchar ,ImgUrl varchar, FOREIGN KEY(CategoryId) REFERENCES Category(Id))";
            //string createTableCategory = "CREATE TABLE Category(Id integer primary key autoincrement not null ,Category varchar)";

            //await connection.ExecuteAsync(createTableCategory);
            //await connection.ExecuteAsync(createTableExpenses);
        }

        private async Task<int> InsertExpenceAsync(Expense item)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.InsertAsync(item);
            return result;
        }

        private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            var amount = 0.0;
            double.TryParse(this.tbAmount.Text, out amount);
            var date = this.dpDate.Date.UtcDateTime;

            var item = new Expense
            {
                Date = date,
                Category = this.tbCategory.Text,
                SubCategory = this.tbSubCategory.Text,
                Amount = amount,
                Note = this.tbNotes.Text,
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

        private async Task<List<Expense>> GetAllExpensesAsync()
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<Expense>().ToListAsync();
            return result;
        }
    }
}
