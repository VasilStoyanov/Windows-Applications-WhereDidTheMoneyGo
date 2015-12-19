using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using WhereDidTheMoneyGo.AttachedProperties;
using WhereDidTheMoneyGo.Common;
using WhereDidTheMoneyGo.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        // Temporary
        private ObservableCollection<CategoryItemViewModel> categories = new ObservableCollection<CategoryItemViewModel>();
        private ObservableCollection<SubCategoryItemViewModel> categoryItems = new ObservableCollection<SubCategoryItemViewModel>();
        private HashSet<string> categoryNames = new HashSet<string>();

        public MainPage()
        {
            this.InitializeComponent();

            // Sample data. Should be deleted when everything is ready
            var item1 = new SubCategoryItemViewModel() { Category = "Petrol", Amount = 100 };
            var item2 = new SubCategoryItemViewModel() { Category = "Heating", Amount = 300 };
            var item3 = new SubCategoryItemViewModel() { Category = "Clothes", Amount = 50 };


            categoryItems.Add(item1);
            categoryItems.Add(item2);
            categoryItems.Add(item3);

            var category1 = new CategoryItemViewModel() { Category = Category.Food, Items = categoryItems, Amount = 1000 };
            this.categoryNames.Add("food");
            this.categories.Add(category1);

            var category2 = new CategoryItemViewModel() { Category = Category.Housing, Items = categoryItems, Amount = 938 };
            this.categoryNames.Add("housing");
            this.categories.Add(category2);

            var contentViewModel = new CategoryViewModel();
            contentViewModel.Categories = categories;

            this.DataContext = contentViewModel;
            this.notificationBox.Visibility = Visibility.Collapsed;
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
            var newCategoryName = this.nameOfCategory.Text;
            if (categoryNameIsValid)
            {
                var newCategory = new CategoryItemViewModel()
                {
                    Category = Category.Food, // TODO - fix it later
                    Items = new ObservableCollection<SubCategoryItemViewModel>(),
                    Amount = DefaultValues.DefaultCategoryValue
                };

                this.categories.Add(newCategory);
                this.categoryNames.Add(newCategoryName.ToLower());
                NotifyUserMessage(true, newCategoryName);
                this.nameOfCategory.Text = String.Empty;
                this.categoryNameIsValid = false;
            }
            else
            {
                NotifyUserMessage(false, newCategoryName);
            }
        }

        private void ValidateText(object sender, KeyRoutedEventArgs e)
        {
            var correct = true;
            if (this.nameOfCategory.Text.ToLower().Contains(BadWords.Naughty))
            {
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageForBadName;
            }
            else if (this.nameOfCategory.Text.ToLower().Contains(BadWords.FWord))
            {
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageForBadName;
            }
            else if (this.nameOfCategory.Text.Length >= DefaultValues.MaximumLengthOfCategoryName)
            {
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageTooLongName;
            }
            else if (this.nameOfCategory.Text.Length <= DefaultValues.MinimumLengthOfCategoryName)
            {
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageTooShort;
            }
            else if(this.categoryNames.Contains(this.nameOfCategory.Text.ToLower()))
            {
                SetBorder(!correct);
                this.reasonForFailMessage = NotificationMessages.NotifyMessageNameAlreadyExist;
            }
            else
            {
                SetBorder(correct);
            }
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
    }
}
