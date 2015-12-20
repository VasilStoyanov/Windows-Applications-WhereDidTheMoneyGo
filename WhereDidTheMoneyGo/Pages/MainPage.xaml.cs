using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using WhereDidTheMoneyGo.AttachedProperties;
using WhereDidTheMoneyGo.Common;
using WhereDidTheMoneyGo.DataModels;
using WhereDidTheMoneyGo.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WhereDidTheMoneyGo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool categoryNameIsValid = false;
        private string reasonForFailMessage = NotificationMessages.NotifyMessageTooShort;

        public MainPage()
        {
            this.InitializeComponent();

            this.ViewModel = new MainPageViewModel();
            var showMonth = this.datePicker.Date.Month;
            var showYear = this.datePicker.Date.Year;
            var selectedCategory = string.Empty;

            if (this.lbMain.SelectedItem != null)
            {
                selectedCategory = this.lbMain.SelectedItem.ToString();
            }
            this.GetAllData(showMonth, showYear, selectedCategory);

            this.notificationBox.Visibility = Visibility.Collapsed;
        }

        public MainPageViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainPageViewModel;
            }
            set
            {
                this.DataContext = value;
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

        //private void OnShowNewCategoryMenuClick(object sender, RoutedEventArgs e)
        //{
        //    var oldValue = AnimationsProperties.GetShowHideValue(this.newCategory);
        //    AnimationsProperties.SetShowHideValue(this.newCategory, !oldValue);

        public async void GetAllData(int month, int year, string selectedCategory)
        {
            var connection = this.GetDbConnectionAsync();

        public async void GetAllData(int month, int year, string selectedCategory)
        {
            var connection = this.GetDbConnectionAsync();
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

                this.ViewModel.Categories.Add(newCategoryViewModel);
            }
        }

        public async void GetSubCategoriesOnly(int month, int year, string selectedCategory)
        {
            var connection = this.GetDbConnectionAsync();

            var subCategories = await connection.GetAllWithChildrenAsync<SubCategory>(null, true);

            foreach (var subCategory in subCategories)
            {

                if (subCategory.Category.Name == selectedCategory)
                {
                    var newSubCategoryViewModel = new SubCategoryViewModel() { Name = subCategory.Name };
                    var amount = 0.0;

                    foreach (var expense in subCategory.Expenses)
                    {
                        newSubCategoryViewModel.Amount += expense.Amount;
                    }


                    var categoriesCount = this.ViewModel.Categories.Count;
                    for (int i = 0; i < categoriesCount; i++)
                    {
                        if (this.ViewModel.Categories[i].Name == selectedCategory)
                        {
                            this.ViewModel.Categories[i].SubCategories.Add(newSubCategoryViewModel);
                        }
                    }
                }
            }

        }
        //    if (oldValue)
        //    {
        //        this.appBarButton.Icon = new SymbolIcon(Symbol.Remove);
        //    }
        //    else
        //    {
        //        this.appBarButton.Icon = new SymbolIcon(Symbol.Add);
        //    }
        //}

        private void OnNewExpenceAddClick(object sender, RoutedEventArgs e)
        public static double GetExpenses(List<Expense> expenses, int month, int year)
        {
            this.Frame.Navigate(typeof(AddNewExpence)); 
            //    var newCategoryName = this.nameOfCategory.Text;
            //    if (categoryNameIsValid)
            //    {
            //        var newCategory = new CategoryItemViewModel()
            //        {
            //            /*Category = CategoryViewModel.Food,*/ // TODO - fix it later
            //            SubCategories = new ObservableCollection<SubCategoryItemViewModel>(),
            //            Amount = DefaultValues.DefaultCategoryValue
            //        };

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

        private void OnShowNewCategoryMenuClick(object sender, RoutedEventArgs e)
        {
            var oldValue = AnimationsProperties.GetShowHideValue(this.newCategory);
            AnimationsProperties.SetShowHideValue(this.newCategory, !oldValue);

            if (oldValue)
            {
                this.appBarButton.Icon = new SymbolIcon(Symbol.Remove);
            }
            else
            {
                this.appBarButton.Icon = new SymbolIcon(Symbol.Add);
            }
        }

        private void OnCreateNewCategoryClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            //    var newCategoryName = this.nameOfCategory.Text;
            //    if (categoryNameIsValid)
            //    {
            //        var newCategory = new CategoryItemViewModel()
            //        {
            //            /*Category = CategoryViewModel.Food,*/ // TODO - fix it later
            //            SubCategories = new ObservableCollection<SubCategoryItemViewModel>(),
            //            Amount = DefaultValues.DefaultCategoryValue
            //        };

            //        this.categories.Add(newCategory);
            //        this.categoryNames.Add(newCategoryName.ToLower());
            //        NotifyUserMessage(true, newCategoryName);
            //        this.nameOfCategory.Text = String.Empty;
            //        this.categoryNameIsValid = false;
            //    }
            //    else
            //    {
            //        NotifyUserMessage(false, newCategoryName);
            //    }
        }

        private void onDatePickerDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            this.ViewModel.Categories.Clear();
            var showMonth = this.datePicker.Date.Month;
            var showYear = this.datePicker.Date.Year;
            var selectedCategory = string.Empty;
            if (this.lbMain.SelectedItem != null)
            {
                selectedCategory = this.lbMain.SelectedValue.ToString();
            }
            this.GetAllData(showMonth, showYear, selectedCategory);
        }

        private void onlbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = sender as ListBox;
            var selectedCategory = string.Empty;
            if (lb.SelectedItem != null)
            {
                selectedCategory = lb.SelectedValue.ToString();
            }
            
            var showMonth = this.datePicker.Date.Month;
            var showYear = this.datePicker.Date.Year;

            var categoriesCount = this.ViewModel.Categories.Count;
            for (int i = 0; i < categoriesCount; i++)
            {
                this.ViewModel.Categories[i].SubCategories.Clear();
            }

            this.GetSubCategoriesOnly(showMonth, showYear, selectedCategory);
        }
    }
}