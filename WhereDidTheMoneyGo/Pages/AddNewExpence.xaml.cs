namespace WhereDidTheMoneyGo.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using SQLite.Net;
    using SQLite.Net.Async;
    using SQLite.Net.Platform.WinRT;
    using WhereDidTheMoneyGo.AttachedProperties;
    using WhereDidTheMoneyGo.Common;
    using WhereDidTheMoneyGo.DataModels;
    using WhereDidTheMoneyGo.ViewModels;
    using Windows.Storage;
    using Windows.UI;
    using Windows.UI.Notifications;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;

    public sealed partial class AddNewExpence : Page
    {
        private bool validAmount;
        private bool validDescription;
        private string currentText;
        private string reasonForFailMessage = NotificationMessages.NameTooShort;
        private object lastSender;

        public AddNewExpence()
        {
            this.InitializeComponent();
            this.ViewModel = new AddExpenseViewModel();
            this.PopulateCategoriesAsync();

            var oldSaveButtonValue = AnimationsProperties.GetShowHideValue(this.saveButton);
            AnimationsProperties.SetShowHideValue(this.saveButton, !oldSaveButtonValue);

            var oldBackButtonValue = AnimationsProperties.GetShowHideValue(this.backButton);
            AnimationsProperties.SetShowHideValue(this.backButton, !oldBackButtonValue);
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

        public async void PopulateCategoriesAsync()
        {
            var connection = GetDbConnectionAsync();
            var allCategories = await connection.Table<Category>().ToListAsync();

            foreach (var category in allCategories)
            {
                var newCategoryViewModel = new CategoryViewModel { Name = category.Name };
                ViewModel.Categories.Add(newCategoryViewModel);
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

        public async Task<Category> GetCategoriesAsync(string categoryName)
        {
            var connection = GetDbConnectionAsync();
            var result = await connection.Table<Category>()
                                        .Where(x => x.Name == categoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        public async Task<int> InsertExpenceAsync(Expense item)
        {
            var connection = GetDbConnectionAsync();
            var result = await connection.InsertAsync(item);
            return result;
        }

        public async Task<SubCategory> GetSubCategoriesAsync(string subCategoryName)
        {
            var connection = GetDbConnectionAsync();

            var result = await connection.Table<SubCategory>()
                                        .Where(x => x.Name == subCategoryName)
                                        .FirstOrDefaultAsync();
            return result;
        }

        public static async Task GetNotification(string text)
        {
            var peshoXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText03);
            var peshoElements = peshoXml.GetElementsByTagName("text");
            peshoElements[0].AppendChild(peshoXml.CreateTextNode(text));

            var toastNotification = new ToastNotification(peshoXml);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }

        public async Task<List<SubCategory>> GetSubCategoriesByIdAsync(int categoryId)
        {
            var connection = GetDbConnectionAsync();
            var result = await connection.Table<SubCategory>()
                                        .Where(x => x.CategoryId == categoryId)
                                        .ToListAsync();
            return result;
        }

        private void AmountNotifier(object sender, KeyRoutedEventArgs e)
        {
            this.currentText = this.tbAmount.Text;
            lastSender = this.tbAmount;
            double pesho;
            if (double.TryParse(this.currentText, out pesho))
            {
                if (pesho >= double.MaxValue)
                {
                    this.SetBorder(false);
                    reasonForFailMessage = NotificationMessages.AmmountTooBig;
                    this.validAmount = false;
                    return;
                }
                else if (pesho <= 0)
                {
                    this.SetBorder(false);
                    reasonForFailMessage = NotificationMessages.AmmountTooSmall;
                    this.validAmount = false;
                    return;
                }

                this.validAmount = true;
                this.SetBorder(true);
                return;
            }
            this.validAmount = false;
            this.reasonForFailMessage = NotificationMessages.AmmountShouldBeNumber;
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
                this.reasonForFailMessage = NotificationMessages.CannotContainBadWords;
            }
            else if (text.ToLower().Contains(BadWords.FWord))
            {
                this.validDescription = false;
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.BadName;
            }
            else if (text.Length >= DefaultValues.MaximumLengthOfCategoryName)
            {
                this.validDescription = false;
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.TooLongName;
            }
            else if (text.Length <= DefaultValues.MinimumLengthOfCategoryName)
            {
                this.validDescription = false;
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NameTooShort;
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

        private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.tbCategory.SelectedValue == null)
            {
                reasonForFailMessage = NotificationMessages.MustSelectCategory;
                await GetNotification(reasonForFailMessage);
                return;
            }

            if (this.tbSubCategory.SelectedValue == null)
            {
                reasonForFailMessage = NotificationMessages.MustSelectSubCategory;
                await GetNotification(reasonForFailMessage);
                return;
            }

            if (!validAmount)
            {
                reasonForFailMessage = NotificationMessages.AmmountTooSmall;
                await GetNotification(reasonForFailMessage);
                return;
            }
            else if (!validDescription)
            {
                reasonForFailMessage = NotificationMessages.NameTooShort;
                await GetNotification(reasonForFailMessage);
                return;
            }

            var connection = this.GetDbConnectionAsync();

            var amount = 0.0;
            double.TryParse(this.tbAmount.Text, out amount);
            var date = this.dpDate.Date.UtcDateTime;
            var category = await this.GetCategoriesAsync(this.tbCategory.SelectedValue.ToString());
            var subCategory = await this.GetSubCategoriesAsync(this.tbSubCategory.SelectedValue.ToString());

            if (category == null || subCategory == null)
            {
                return;
            }

            var item = new Expense
            {
                CategoryId = category.Id,
                SubCategoryId = subCategory.Id,
                Date = date,
                Amount = amount,
                Description = this.tbDescription.Text
            };

            await this.InsertExpenceAsync(item);
            var message = "Category " + category.Name + " updated! :)";
            await GetNotification(message);
            this.Frame.Navigate(typeof(MainPage));
        }

        private void OnBackToMainPageClick(object sender, RoutedEventArgs e)
        {
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

        private void OnBackToMainPage(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}