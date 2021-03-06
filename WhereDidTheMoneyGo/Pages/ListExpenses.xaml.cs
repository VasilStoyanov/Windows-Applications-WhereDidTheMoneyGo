﻿namespace WhereDidTheMoneyGo.Pages
{
    using System;
    using System.IO;

    using SQLite.Net;
    using SQLite.Net.Async;
    using SQLite.Net.Platform.WinRT;
    using SQLiteNetExtensionsAsync.Extensions;
    using WhereDidTheMoneyGo.DataModels;
    using WhereDidTheMoneyGo.ViewModels;
    using Windows.Foundation;
    using Windows.Storage;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using Common;
    public sealed partial class ListExpenses : Page
    {
        private Point initialpoint;

        public ListExpenses()
        {
            this.InitializeComponent();
            this.ViewModel = new ListExpensesViewModel();
            this.ManipulationStarted += Page_ManipulationStarted;
            this.ManipulationDelta += Page_ManipulationDelta;
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
                        Date = item.Date.ToString("dd MMMM yyyy"),
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameters = e.Parameter as ListExpensesParametersViewModel;
            this.GetAllData(parameters);
            this.ViewModel.SelectedParameters = $"Cateory '{parameters.SelectedCategory}', {parameters.SelectedMonth}.{parameters.SelectedYear}";
        }

        private void OnBackToMainPage(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void ListBox_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var item = sender as Grid;
            //var itemToDelete = item.DataContext.Id;
        }

        private void Page_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
                initialpoint = e.Position;
        }

        private void Page_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                Point currentpoint = e.Position;
                if (initialpoint.X - currentpoint.X >= DefaultValues.ThresholdValue)
                {
                    e.Complete();
                    
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
        }

        private void OnNewExpenceAddClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddNewExpence));
        }
    }
}