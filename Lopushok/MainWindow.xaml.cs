using Lopushok.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Lopushok
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

   
    public partial class MainWindow : Window
    {

        public List<Product> allProducts = App.db.Product.ToList();
        public List<Product> FilterProducts = App.db.Product.ToList();
        public int _currentPage = 1;
        
        public int _productsPerPage = 4;
        public int _totalPages;

        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
            List<ProductType> type = App.db.ProductType.ToList();
            type.Insert(0 ,new ProductType() { ID=1000000, Title = "Все типы", DefectedPercent = 0});
            FilterComboBox.ItemsSource = type;



        }

        private void LoadProducts()
        {
            ProductsItemsControl.Items.Clear();
            int a = 20 * _currentPage;
            int b = a - 20;
        
            for(int i =b; i <= a; i++)
            {
                if (FilterProducts.Count > i )
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
            if(SearchTextBox.Text.Length > 0)
            {
                string searchText = SearchTextBox.Text.ToLower();
                FilterProducts = allProducts.Where(x => x.Title.ToLower().Contains(searchText) || x.Description.ToLower().Contains(searchText)).ToList();
                _currentPage = 1;
                LoadProducts();

            }
            else
            {
                FilterProducts = allProducts;
                LoadProducts();
            }

            
            

        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (FilterComboBox.SelectedIndex != -1 && selfilter.Title != "Все")
            //{

            //    FilterProducts = allProducts.Where(x => x.ProductTypeID == selfilter.ID).ToList();
            //    _currentPage = 1;
            //    LoadProducts();

            //}
            //else
            //{
            //    FilterProducts = allProducts;
            //    LoadProducts();
            //}
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selfilter = FilterComboBox.SelectedItem as ProductType;
            if (FilterComboBox.SelectedIndex != -1 && selfilter.Title != "Все")
            {
                
                FilterProducts = allProducts.Where(x => x.ProductTypeID == selfilter.ID ).ToList();
                _currentPage = 1;
                LoadProducts();

            }
            else
            {
                FilterProducts = allProducts;
                LoadProducts();
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
            if(_currentPage != FilterProducts.Count / 20 )
           _currentPage++;
            LoadProducts();
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