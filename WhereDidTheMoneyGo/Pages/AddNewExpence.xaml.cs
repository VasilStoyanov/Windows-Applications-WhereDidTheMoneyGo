namespace WhereDidTheMoneyGo.Pages
{
    using Data;
    using System;
    using System.Threading.Tasks;
    using WhereDidTheMoneyGo.AttachedProperties;
    using WhereDidTheMoneyGo.Common;
    using WhereDidTheMoneyGo.DataModels;
    using WhereDidTheMoneyGo.ViewModels;
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
        private string reasonForFailMessage = NotificationMessages.NotifyMessageTooShort;
        private object lastSender;

        public AddNewExpence()
        {
            this.InitializeComponent();
            this.ViewModel = new AddExpenseViewModel();
            DatabaseConnections.ViewModel = this.ViewModel;
            DatabaseConnections.InitAsync();
            DatabaseConnections.PopulateCategoriesAsync();

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

        private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if(!validAmount || !validDescription)
            {
                return;
            }

            var connection = DatabaseConnections.GetDbConnectionAsync();

            var amount = 0.0;
            double.TryParse(this.tbAmount.Text, out amount);
            var date = this.dpDate.Date.UtcDateTime;
            var category = await DatabaseConnections.GetCategoriesAsync(this.tbCategory.SelectedValue.ToString()); 
            var subCategory = await DatabaseConnections.GetSubCategoriesAsync(this.tbSubCategory.SelectedValue.ToString());

            if(category == null || subCategory == null)
            {
                return;
            }

            var item = new Expense
            {
                CategoryId = category.Id,
                SubCategoryId = subCategory.Id,
                Date = date,
                Amount = amount,
                Description = this.tbDescription.Text,
                ImgUrl = this.tbImageUrl.Text
            };

            await DatabaseConnections.InsertExpenceAsync(item);
            var message = "Category " + category.Name + " updated! :)";
            await GetNotification(message);
            this.Frame.Navigate(typeof(MainPage));
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

        public static async Task GetNotification(string text)
        {
            var peshoXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText03);
            var peshoElements = peshoXml.GetElementsByTagName("text");
            peshoElements[0].AppendChild(peshoXml.CreateTextNode(text));

            var toastNotification = new ToastNotification(peshoXml);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
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
            var category = await DatabaseConnections.GetCategoriesAsync(this.ViewModel.CurrentCategory); 
            var allSubCategories = await DatabaseConnections.GetSubCategoriesByIdAsync(category.Id);
            foreach (var item in allSubCategories)
            {
                var newSubCategoryViewModel = new SubCategoryViewModel { Name = item.Name };
                this.ViewModel.SubCategories.Add(newSubCategoryViewModel);
            }
        }
    }
}
