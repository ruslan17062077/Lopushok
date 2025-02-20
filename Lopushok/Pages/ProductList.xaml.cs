using Lopushok.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lopushok.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductList.xaml
    /// </summary>
    public partial class ProductList : Page
    {
        public List<Product> allProducts = App.db.Product.ToList();
        public List<Product> FilterProducts = App.db.Product.ToList();
        public int _currentPage = 1;

        public int _productsPerPage = 4;
        public int _totalPages;
        public ProductList()
        {
            InitializeComponent();
            LoadProducts();
            List<ProductType> type = App.db.ProductType.ToList();
            type.Insert(0, new ProductType() { ID = 1000000, Title = "Все типы", DefectedPercent = 0 });
            FilterComboBox.ItemsSource = type;

        }
        private void LoadProducts()
        {
            ProductsItemsControl.Items.Clear();
            int a = 20 * _currentPage;
            int b = a - 20;

            for (int i = b; i <= a; i++)
            {
                if (FilterProducts.Count > i)
                {
                    ProductsItemsControl.Items.Add(FilterProducts[i]);
                }
            }


            PageInfoTextBlock.Text = $"{_currentPage} / {FilterProducts.Count / 20}";
        }

        private void DisplayProducts()
        {

        }




        private void UpdatePagination()
        {

        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAndSorting();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSorting();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSorting();
        }

        private void ApplyFiltersAndSorting()
        {
            string searchText = SearchTextBox.Text.ToLower();

            // Фильтрация по поисковому запросу
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                FilterProducts = allProducts
      .Where(x => x.Title.ToLower().Contains(searchText) ||
                  (x.Description ?? "").ToLower().Contains(searchText)) 
      .ToList();

            }
            else
            {
                FilterProducts = allProducts;
            }

            // Фильтрация по типу продукта
            var selectedFilter = FilterComboBox.SelectedItem as ProductType;
            if (selectedFilter != null && selectedFilter.Title != "Все типы")
            {
                FilterProducts = FilterProducts
                    .Where(x => x.ProductTypeID == selectedFilter.ID)
                    .ToList();
            }

            // Применение сортировки
            SortProducts();

            _currentPage = 1;
            LoadProducts();
        }

        private void SortProducts()
        {
            switch (SortComboBox.SelectedIndex)
            {
                case 0: // Наименование (А-Я)
                    FilterProducts = FilterProducts.OrderBy(p => p.Title).ToList();
                    break;
                case 1: // Наименование (Я-А)
                    FilterProducts = FilterProducts.OrderByDescending(p => p.Title).ToList();
                    break;
                case 2: // Цех (по возрастанию)
                    FilterProducts = FilterProducts.OrderBy(p => p.ProductionWorkshopNumber).ToList();
                    break;
                case 3: // Цех (по убыванию)
                    FilterProducts = FilterProducts.OrderByDescending(p => p.ProductionWorkshopNumber).ToList();
                    break;
                case 4: // Минимальная стоимость (по возрастанию)
                    FilterProducts = FilterProducts.OrderBy(p => Convert.ToDecimal(p.MinCostForAgent)).ToList();
                    break;
                case 5: // Минимальная стоимость (по убыванию)
                    FilterProducts = FilterProducts.OrderByDescending(p => Convert.ToDecimal(p.MinCostForAgent)).ToList();
                    break;
            }
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage != 1)
            {
                _currentPage--;
                LoadProducts();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage != FilterProducts.Count / 20)
                _currentPage++;
            LoadProducts();
        }

        private void ProductsItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Получаем элемент, по которому был произведен двойной клик
            var clickedElement = e.OriginalSource as DependencyObject;

            // Ищем родительский элемент типа FrameworkElement (Border, StackPanel и т.д.)
            while (clickedElement != null && !(clickedElement is ContentPresenter))
            {
                clickedElement = VisualTreeHelper.GetParent(clickedElement);
            }

            // Проверяем, что клик был сделан внутри ItemsControl
            if (clickedElement is ContentPresenter presenter)
            {
                // Получаем объект Product из DataContext
                if (presenter.DataContext is Product selectedProduct)
                {
                    // Открываем страницу редактирования с передачей объекта
                    App.main.myframe.NavigationService.Navigate(new Pages.AddamdEditProduct(selectedProduct));
                }
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            App.main.myframe.NavigationService.Navigate(new Pages.AddamdEditProduct(null));
        }

        //public void Photo()
        //{
        //    foreach(var item in App.db.Product)
        //    {
        //        if (item.Description != "")
        //        {
        //            string filepath = $"Component{item.Description}";
        //            byte[] image = File.ReadAllBytes(filepath);
        //            item.Image = image;

        //            Console.WriteLine("ok");
        //        }
        //    }
        //    App.db.SaveChanges();
        //}
    }
}
