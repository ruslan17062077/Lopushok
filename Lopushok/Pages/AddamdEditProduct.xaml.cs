using Lopushok.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Lopushok.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddamdEditProduct.xaml
    /// </summary>
    public partial class AddamdEditProduct : Page
    {
        Product product;
        private bool isEditMode = false;
        private List<Material> allMaterials;

        public AddamdEditProduct(Product _product)
        {
            InitializeComponent();
            ProdTypeCBox.ItemsSource = App.db.ProductType.ToList();

            // Если редактирование, используем переданный продукт, иначе создаём новый
            if (_product != null)
            {
                this.product = _product;
                isEditMode = true;
                ImageProduct.Source = product.Images;
            }
            else
            {
                this.product = new Product();
                // Если коллекция материалов не инициализирована, создаём её
                if (this.product.ProductMaterial == null)
                    this.product.ProductMaterial = new HashSet<ProductMaterial>();
            }

            // Загрузка всех материалов
            allMaterials = App.db.Material.ToList();

            // Если есть материалы у продукта, исключаем их из списка добавляемых
            if (product.ProductMaterial != null && product.ProductMaterial.Any())
            {
                foreach (var item in product.ProductMaterial)
                {
                    allMaterials.RemoveAll(m => m.ID == item.Material.ID);
                }
            }

            MaterialsComboBox.ItemsSource = allMaterials;
            UpdateMaterialsDataGrid();

            this.DataContext = this.product;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполненность полей
            if (string.IsNullOrWhiteSpace(product.ArticleNumber) ||
                string.IsNullOrWhiteSpace(product.Title) ||
                product.ProductType == null ||
                string.IsNullOrWhiteSpace(product.MinCostForAgent))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка, что минимальная стоимость - это число
            if (!decimal.TryParse(product.MinCostForAgent, out decimal minCost) || minCost <= 0)
            {
                MessageBox.Show("Минимальная стоимость должна быть числом больше 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка, что количество человек - целое число
            if (product.ProductionPersonCount.HasValue && product.ProductionPersonCount < 0)
            {
                MessageBox.Show("Количество людей должно быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка, что номер производственного цеха - целое число
            if (product.ProductionWorkshopNumber.HasValue && product.ProductionWorkshopNumber < 0)
            {
                MessageBox.Show("Номер цеха должен быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (App.db.Product.FirstOrDefault(x => x.ArticleNumber == product.ArticleNumber && x.ID != product.ID) != null)
            {
                MessageBox.Show("Артикул занят", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isEditMode)
                {
                    // Обновляем существующий продукт
                    var existingProduct = App.db.Product.FirstOrDefault(p => p.ID == product.ID);
                    if (existingProduct != null)
                    {
                        existingProduct.ArticleNumber = product.ArticleNumber;
                        existingProduct.Title = product.Title;
                        existingProduct.ProductTypeID = product.ProductTypeID;
                        existingProduct.ProductionPersonCount = product.ProductionPersonCount;
                        existingProduct.ProductionWorkshopNumber = product.ProductionWorkshopNumber;
                        existingProduct.MinCostForAgent = product.MinCostForAgent;
                        existingProduct.Description = product.Description;
                        existingProduct.Image = product.Image;
                        // Обновление материалов продукта происходит автоматически, если они привязаны к существующему объекту.
                    }
                }
                else
                {
                    // Добавляем новый продукт
                    App.db.Product.Add(product);
                }

                App.db.SaveChanges();
                MessageBox.Show("Продукт успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                App.main.myframe.NavigationService.Navigate(new Pages.ProductList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    product.Image = imageBytes;

                    // Отображаем изображение в интерфейсе
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    ImageProduct.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (product.ProductSale.Count != 0)
            {
                MessageBox.Show("Удаление невозможно, так как есть продажи");
            }
            else
            {
                App.db.Product.Remove(product);
                foreach (var item in App.db.ProductMaterial.Where(x => x.ProductID == product.ID).ToList())
                {
                    App.db.ProductMaterial.Remove(item);
                }
                App.db.SaveChanges();
                MessageBox.Show("Успешно удалено");
                App.main.myframe.NavigationService.Navigate(new Pages.ProductList());
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            App.main.myframe.NavigationService.Navigate(new Pages.ProductList());
        }

        // Метод для обновления источника данных для DataGrid
        private void UpdateMaterialsDataGrid()
        {
            MaterialsDataGrid.ItemsSource = product.ProductMaterial != null
                ? product.ProductMaterial.ToList()
                : new List<ProductMaterial>();
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsComboBox.SelectedItem is Material selectedMaterial)
            {
                if (int.TryParse(MaterialCountTextBox.Text, out int count) && count > 0)
                {
                    // Если коллекция материалов не инициализирована, создаём её
                    if (product.ProductMaterial == null)
                        product.ProductMaterial = new HashSet<ProductMaterial>();

                    var newMaterial = new ProductMaterial
                    {
                        Material = selectedMaterial,
                        Count = count
                    };

                    product.ProductMaterial.Add(newMaterial);
                    App.db.ProductMaterial.Add(newMaterial);
                    App.db.SaveChanges();

                    MessageBox.Show("Материал добавлен!");

                    // Обновляем таблицу материалов
                    UpdateMaterialsDataGrid();

                    // Удаляем добавленный материал из списка доступных
                    allMaterials.RemoveAll(m => m.ID == selectedMaterial.ID);
                    MaterialsComboBox.ItemsSource = null;
                    MaterialsComboBox.ItemsSource = allMaterials;

                    MaterialCountTextBox.Text = "0";
                    MaterialsComboBox.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите материал!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is ProductMaterial selectedMaterial)
            {
                product.ProductMaterial.Remove(selectedMaterial);
                App.db.ProductMaterial.Remove(selectedMaterial);
                App.db.SaveChanges();

                MessageBox.Show("Материал удален!");

                // Возвращаем удалённый материал обратно в список всех материалов
                allMaterials.Add(selectedMaterial.Material);
                MaterialsComboBox.ItemsSource = null;
                MaterialsComboBox.ItemsSource = allMaterials;

                MaterialCountTextBox.Text = "0";
                MaterialsComboBox.SelectedIndex = -1;

                UpdateMaterialsDataGrid();
            }
            else
            {
                MessageBox.Show("Выберите материал для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
