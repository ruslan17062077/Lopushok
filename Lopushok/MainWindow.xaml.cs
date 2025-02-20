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

       

        public MainWindow()
        {
            InitializeComponent();
            App.main = this;    
            myframe.NavigationService.Navigate(new Pages.ProductList());


        }

       
    }
} 