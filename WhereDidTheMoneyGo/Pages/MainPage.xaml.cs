using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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


            // Sample data. Should be deleted when everything is ready
            //var item1 = new SubCategoryItemViewModel() { Category = "Petrol", Amount = 100 };
            //var item2 = new SubCategoryItemViewModel() { Category = "Heating", Amount = 300 };
            //var item3 = new SubCategoryItemViewModel() { Category = "Clothes", Amount = 50 };


            //categoryItems.Add(item1);
            //categoryItems.Add(item2);
            //categoryItems.Add(item3);

            //var category1 = new CategoryItemViewModel() { Category = CategoryViewModel.Food, SubCategories = categoryItems, Amount = 1000 };
            //this.categoryNames.Add("food");
            //this.categories.Add(category1);

            //var category2 = new CategoryItemViewModel() { Category = CategoryViewModel.Housing, SubCategories = categoryItems, Amount = 938 };
            //this.categoryNames.Add("housing");
            //this.categories.Add(category2);

            this.ViewModel = new MainPageViewModel();
            this.GetAllData();

            //this.GetData();

            //contentViewModel.Categories = categories;

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

        private void ValidateText(object sender, KeyRoutedEventArgs e)
        {
            throw new NotImplementedException();
            //    var correct = true;
            //    if (this.nameOfCategory.Text.ToLower().Contains(BadWords.Naughty))
            //    {
            //        SetBorder(!correct);
            //        this.reasonForFailMessage = NotificationMessages.NotifyMessageForBadName;
            //    }
            //    else if (this.nameOfCategory.Text.ToLower().Contains(BadWords.FWord))
            //    {
            //        SetBorder(!correct);
            //        this.reasonForFailMessage = NotificationMessages.NotifyMessageForBadName;
            //    }
            //    else if (this.nameOfCategory.Text.Length >= DefaultValues.MaximumLengthOfCategoryName)
            //    {
            //        SetBorder(!correct);
            //        this.reasonForFailMessage = NotificationMessages.NotifyMessageTooLongName;
            //    }
            //    else if (this.nameOfCategory.Text.Length <= DefaultValues.MinimumLengthOfCategoryName)
            //    {
            //        SetBorder(!correct);
            //        this.reasonForFailMessage = NotificationMessages.NotifyMessageTooShort;
            //    }
            //    else if(this.categoryNames.Contains(this.nameOfCategory.Text.ToLower()))
            //    {
            //        SetBorder(!correct);
            //        this.reasonForFailMessage = NotificationMessages.NotifyMessageNameAlreadyExist;
            //    }
            //    else
            //    {
            //        SetBorder(correct);
            //    }
        }

        private void SetBorder(bool correct)
        {
            if (correct)
            {
                this.nameOfCategory.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                this.categoryNameIsValid = true;
            }
            else
            {
                this.nameOfCategory.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                this.categoryNameIsValid = false;
            }
        }

        private void NotifyUserMessage(bool isValid, string name)
        {
            if (isValid)
            {
                var oldValue = AnimationsProperties.GetShowHideValue(this.notificationBox);
                AnimationsProperties.SetShowHideValue(this.notificationBox, !oldValue);

                var timer = new DispatcherTimer();
                var stopWatch = new Stopwatch();
                timer.Interval = TimeSpan.FromMilliseconds(15);
                timer.Start();
                stopWatch.Start();
                timer.Tick += (sender, args) =>
                {
                    if (stopWatch.ElapsedMilliseconds >= 2000)
                    {
                        timer.Stop();
                        stopWatch.Stop();
                        this.notificationBox.Visibility = Visibility.Collapsed;
                        return;
                    }
                };

                this.notificationBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                this.notificationBox.Text = string.Format("Successfuly added category {0}", name);
            }
            else
            {
                var oldValue = AnimationsProperties.GetShowHideValue(this.notificationBox);
                AnimationsProperties.SetShowHideValue(this.notificationBox, !oldValue);

                var timer = new DispatcherTimer();
                var stopWatch = new Stopwatch();
                timer.Interval = TimeSpan.FromMilliseconds(15);
                timer.Start();
                stopWatch.Start();
                timer.Tick += (sender, args) =>
                {
                    if (stopWatch.ElapsedMilliseconds >= 2000)
                    {
                        timer.Stop();
                        stopWatch.Stop();
                        this.notificationBox.Visibility = Visibility.Collapsed;
                        return;
                    }
                };

                this.notificationBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                this.notificationBox.Text = this.reasonForFailMessage;
            }
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
