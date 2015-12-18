using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        // Temporary
        private ObservableCollection<CategoryItemViewModel> categories = new ObservableCollection<CategoryItemViewModel>();
        private ObservableCollection<SubCategoryItemViewModel> categoryItems = new ObservableCollection<SubCategoryItemViewModel>();

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

            var category1 = new CategoryItemViewModel() { Category = new Category("Food"), Items = categoryItems, Amount = 1000 };
            var category2 = new CategoryItemViewModel() { Category = new Category("Housing"), Items = categoryItems, Amount = 938 };

            categories.Add(category1);
            categories.Add(category2);

            var contentViewModel = new CategoryViewModel();
            contentViewModel.Categories = categories;

            this.DataContext = contentViewModel;
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
            if(categoryNameIsValid)
            {
                var newCategoryName = this.nameOfCategory.Text;
                var newCategory = new CategoryItemViewModel()
                {
                    Category = new Category(newCategoryName),
                    Items = new ObservableCollection<SubCategoryItemViewModel>(),
                    Amount = DefaultValues.DefaultCategoryValue
                };

                this.categories.Add(newCategory);
            }
        }

        private void ValidateText(object sender, KeyRoutedEventArgs e)
        {
            var correct = true;
            // Part of developer's job, lol
            if(this.nameOfCategory.Text.ToLower().Contains(BadWords.Naughty))
            {
                SetBorder(!correct);
            }
            else if(this.nameOfCategory.Text.ToLower().Contains(BadWords.FWord))
            {
                SetBorder(!correct);
            }
            else if(this.nameOfCategory.Text.Length >= 50 || this.nameOfCategory.Text.Length == 0)
            {
                SetBorder(!correct);
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
    }
}
