using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhereDidTheMoneyGo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewExpence : Page
    {
        private bool validAmount;
        private bool validDescription;
        private string currentText;
        private string reasonForFailMessage = NotificationMessages.NotifyMessageTooShort;
        private object lastSender;

        public AddNewExpence()
        {
            this.InitializeComponent();
            this.InitAsync();
            this.ViewModel = new AddExpenseViewModel();
            this.PopulateCategoriesAsync();
        }

        public AddExpenseViewModel ViewModel
        {
            get
            {
                return this.DataContext as AddExpenseViewModel;
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

        public async void InitAsync()
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

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<Category>().ToListAsync();

            return result;
        }

        public async void PopulateCategoriesAsync()
        {
            var connection = this.GetDbConnectionAsync();
            var allCategories = await connection.Table<Category>().ToListAsync();

            foreach (var category in allCategories)
            {
                var newCategoryViewModel = new CategoryViewModel { Name = category.Name };
                this.ViewModel.Categories.Add(newCategoryViewModel);
            }
        }

        public async Task<Category> GetCategoriesAsync(string categoryName)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<Category>()
                                        .Where(x => x.Name == categoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        public async Task<SubCategory> GetSubCategoriesAsync(string subCategoryName)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<SubCategory>()
                                        .Where(x => x.Name == subCategoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<SubCategory>> GetSubCategoriesByIdAsync(int categoryId)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.Table<SubCategory>()
                                        .Where(x => x.CategoryId == categoryId)
                                        .ToListAsync();
            return result;
        }

        private async Task<int> SaveExpenceAsync(Expense item)
        {
            var connection = this.GetDbConnectionAsync();
            var result = await connection.InsertAsync(item);
            return result;
        }

        //private void OnCreateNewCategoryClick(object sender, RoutedEventArgs e)
        //{
        //    if (this.newCategory.Visibility == Visibility.Collapsed)
        //    {
        //        this.newCategory.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        this.newCategory.Visibility = Visibility.Collapsed;
        //    }
        //}

        private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if(!validAmount || !validDescription)
            {
                return;
            }

            var connection = this.GetDbConnectionAsync();

            var amount = 0.0;
            double.TryParse(this.tbAmount.Text, out amount);
            var date = this.dpDate.Date.UtcDateTime;
            var category = await this.GetCategoriesAsync(this.tbCategory.SelectedValue.ToString());
            var subCategory = await this.GetSubCategoriesAsync(this.tbSubCategory.SelectedValue.ToString());

            var item = new Expense
            {
                CategoryId = category.Id,
                SubCategoryId = subCategory.Id,
                Date = date,
                Amount = amount,
                Description = this.tbDescription.Text,
                ImgUrl = this.tbImageUrl.Text
            };

            await this.SaveExpenceAsync(item);
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void OntbCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear current selection
            this.tbSubCategory.SelectedIndex = -1;

            // Clear the current SubCategory List
            this.ViewModel.SubCategories.Clear();

            // Add new list of sub-category items
            this.ViewModel.CurrentCategory = this.tbCategory.SelectedValue.ToString();
            var category = await this.GetCategoriesAsync(this.ViewModel.CurrentCategory);
            var allSubCategories = await this.GetSubCategoriesByIdAsync(category.Id);
            foreach (var item in allSubCategories)
            {
                var newSubCategoryViewModel = new SubCategoryViewModel { Name = item.Name };
                this.ViewModel.SubCategories.Add(newSubCategoryViewModel);
            }
        }
        
        private void AmountNotifier(object sender, KeyRoutedEventArgs e)
        {
            this.currentText = this.tbAmount.Text;
            lastSender = this.tbAmount;
            double pesho;
            if(double.TryParse(this.currentText, out pesho))
            {
                if(pesho >= double.MaxValue)
                {
                    this.SetBorder(false);
                    reasonForFailMessage = NotificationMessages.NotifyMessageAmmountTooBig;
                    this.validAmount = false;
                    return;
                }
                else if (pesho <= 0)
                {
                    this.SetBorder(false);
                    reasonForFailMessage = NotificationMessages.NotifyMessageAmmountTooSmall;
                    this.validAmount = false;
                    return;
                }

                this.validAmount = true;
                this.SetBorder(true);
                return;
            }
            this.validAmount = false;
            this.SetBorder(false);
        }

        private void DescriptionNotifier(object sender, KeyRoutedEventArgs e)
        {
            this.currentText = this.tbDescription.Text;
            lastSender = this.tbDescription;
            ValidateDescription(this.currentText);
        }

        private void ValidateDescription(string text)
        {
            var correct = true;
            if (text.ToLower().Contains(BadWords.Naughty))
            {
                this.validDescription = false;
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageForBadName;
            }
            else if (text.ToLower().Contains(BadWords.FWord))
            {
                this.validDescription = false;
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageForBadName;
            }
            else if (text.Length >= DefaultValues.MaximumLengthOfCategoryName)
            {
                this.validDescription = false;
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageTooLongName;
            }
            else if (text.Length <= DefaultValues.MinimumLengthOfCategoryName)
            {
                this.validDescription = false;
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageTooShort;
            }
            else
            {
                this.validDescription = true;
                SetBorder(correct);
            }
        }

        private void SetBorder(bool correct)
        {
            var element = lastSender as TextBox;
            if (correct)
            {
                element.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            }
            else
            {
                element.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
        }

        //private void NotifyUserMessage(bool isValid, string name)
        //{
        //    if (isValid)
        //    {
        //        var oldValue = AnimationsProperties.GetShowHideValue(this.notificationText);
        //        AnimationsProperties.SetShowHideValue(this.notificationText, !oldValue);

        //        var timer = new DispatcherTimer();
        //        var stopWatch = new Stopwatch();
        //        timer.Interval = TimeSpan.FromMilliseconds(15);
        //        timer.Start();
        //        stopWatch.Start();
        //        timer.Tick += (sender, args) =>
        //        {
        //            if (stopWatch.ElapsedMilliseconds >= 3000)
        //            {
        //                timer.Stop();
        //                stopWatch.Stop();
        //                this.notificationText.Visibility = Visibility.Collapsed;
        //                return;
        //            }
        //        };

        //        this.notificationText.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        //        this.notificationText.Text = string.Format("Successfuly added category {0}", name);
        //    }
        //    else
        //    {
        //        var oldValue = AnimationsProperties.GetShowHideValue(this.notificationText);
        //        AnimationsProperties.SetShowHideValue(this.notificationText, !oldValue);

        //        var timer = new DispatcherTimer();
        //        var stopWatch = new Stopwatch();
        //        timer.Interval = TimeSpan.FromMilliseconds(15);
        //        timer.Start();
        //        stopWatch.Start();
        //        timer.Tick += (sender, args) =>
        //        {
        //            if (stopWatch.ElapsedMilliseconds >= 3000)
        //            {
        //                timer.Stop();
        //                stopWatch.Stop();
        //                this.notificationText.Visibility = Visibility.Collapsed;
        //                return;
        //            }
        //        };

        //        this.notificationText.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        //        this.notificationText.Text = this.reasonForFailMessage;
        //    }
        //}
    }
}