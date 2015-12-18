using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WhereDidTheMoneyGo.AttachedProperties;
using WhereDidTheMoneyGo.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public MainPage()
        {
            this.InitializeComponent();

            // Sample data. Should be deleted when everything is ready
            var item1 = new SubCategoryItemViewModel() { Category = "Petrol", Amount = 100 };
            var item2 = new SubCategoryItemViewModel() { Category = "Heating", Amount = 300 };
            var item3 = new SubCategoryItemViewModel() { Category = "Clothes", Amount = 50 };

            var items = new List<SubCategoryItemViewModel>();

            items.Add(item1);
            items.Add(item2);
            items.Add(item3);

            var category1 = new CategoryItemViewModel() { Category = Category.Food, Items = items, Amount = 1000 };
            var category2 = new CategoryItemViewModel() { Category = Category.Housing, Items = items, Amount = 938 };

            var categories = new List<CategoryItemViewModel>();

            categories.Add(category1);
            categories.Add(category2);

            var contentViewModel = new CategoryViewModel();
            contentViewModel.Categories = categories;

            this.DataContext = contentViewModel;
        }

        private void OnCreateNewCategoryClick(object sender, RoutedEventArgs e)
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
    }
}
