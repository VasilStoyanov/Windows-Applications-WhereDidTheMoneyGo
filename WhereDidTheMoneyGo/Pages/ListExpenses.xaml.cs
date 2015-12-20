using System;
using System.IO;
using System.Linq;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using SQLiteNetExtensionsAsync.Extensions;
using WhereDidTheMoneyGo.DataModels;
using WhereDidTheMoneyGo.ViewModels;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhereDidTheMoneyGo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListExpenses : Page
    {
        public ListExpenses()
        {
            this.InitializeComponent();
            this.ViewModel = new ListExpensesViewModel();

        }

        public ListExpensesViewModel ViewModel
        {
            get
            {
                return this.DataContext as ListExpensesViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public async void GetAllData(ListExpensesParametersViewModel parameters)
        {
            var connection = this.GetDbConnectionAsync();

            var expenses = await connection.GetAllWithChildrenAsync<Expense>();
            foreach (var item in expenses)
            {
                if (item.Category.Name == parameters.SelectedCategory 
                    && item.Date.Month == parameters.SelectedMonth
                    && item.Date.Year == parameters.SelectedYear)
                {
                    this.ViewModel.Expenses.Add(new ExpenceViewModel()
                    {
                        Id = item.Id,
                        Date = item.Date,
                        Category = item.Category.Name,
                        SubCategory = item.SubCategory.Name,
                        Description = item.Description,
                        Amount = item.Amount
                    });
                }
            }
        }

        public SQLiteAsyncConnection GetDbConnectionAsync()
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

        private void OnBackToMainPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameters = e.Parameter as ListExpensesParametersViewModel;
            this.GetAllData(parameters);
            this.ViewModel.SelectedParameters = $"Cateory '{parameters.SelectedCategory}', {parameters.SelectedMonth}.{parameters.SelectedYear}";
        }

        private void ListBox_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var item = sender as Grid;
            //var itemToDelete = item.DataContext.Id;
        }
    }
}