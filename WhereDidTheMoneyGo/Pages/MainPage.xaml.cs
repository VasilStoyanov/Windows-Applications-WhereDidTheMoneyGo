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
        //// Temporary
        //private ObservableCollection<CategoryItemViewModel> categories = new ObservableCollection<CategoryItemViewModel>();
        //private ObservableCollection<SubCategoryItemViewModel> categoryItems = new ObservableCollection<SubCategoryItemViewModel>();
        //private HashSet<string> categoryNames = new HashSet<string>();

        public MainPage()
        {
            this.InitializeComponent();
            
            this.ViewModel = new MainPageViewModel();
            this.GetAllData();

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

        public async void GetAllData()
        {
            var connection = this.GetDbConnectionAsync();

            var categories = await connection.GetAllWithChildrenAsync<Category>(null, true);

            foreach (var category in categories)
            {
                var categorySum = 0.0;
                foreach (var expense in category.Expenses)
                {
                    categorySum += expense.Amount;
                }


                var newCategoryViewModel = new CategoryViewModel() { Name = category.Name, Amount = categorySum };

                var newSubCategoryViewModel = new SubCategoryViewModel();
                category.SubCategories.ForEach(
                    s => newCategoryViewModel.SubCategories.Add(
                                                            new SubCategoryViewModel
                                                            {
                                                                Name = s.Name,
                                                                Amount = GetExpenses(s.Expenses)
                                                            })
                );

                this.ViewModel.Categories.Add(newCategoryViewModel);
            }
        }

        public static double GetExpenses(List<Expense> expenses)
        {
            var amount = 0.0;
            if (expenses == null)
            {
                return amount;
            }

            foreach (var expense in expenses)
            {
                amount += expense.Amount;
            }
            return amount;
        }

    }
}
