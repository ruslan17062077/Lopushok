﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Schema;

namespace Lopushok.DB
{
    public partial class Product
    {
        public BitmapImage Images
        {
            get
            {
                if (Image != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream byteStream = new MemoryStream(Image))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = byteStream;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze(); // Предотвращает ошибки доступа из других потоков
                    }
                    return bitmapImage;
                }
                else
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("pack://application:,,,/Component/Images/picture.png", UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
                        return null;
                    }
                }
            }
        }


        public string Materials
        {
            get
            {
                string materials = "";
                foreach(var item in ProductMaterial)
                {
                    materials += $" { item.Material.Title} ";
                }
                return materials;
            }
        }
        public double TotalCost
        {
            get
            {
                double totalcost = 0;
                foreach(var item in ProductMaterial)
                {
                    if (item.Count != null)
                    {
                        totalcost += (double)(item.Count * item.Material.Cost);
                    }
                }
                if (totalcost != 0)
                {
                    return totalcost;
                }
                else
                {
                    return double.Parse(MinCostForAgent);
                }
            }
        }
        public SolidColorBrush BackgroundColor
        {
            get
            {
                if (ProductSale != null && ProductSale.Any())
                {
                    var lastSaleDate = ProductSale.Last().SaleDate;
                    if (lastSaleDate.Month != DateTime.Now.Month || lastSaleDate.Year != DateTime.Now.Year)
                    {
                        return new SolidColorBrush(Colors.LightCoral); 
                    }
                }
                else
                {
                    return new SolidColorBrush(Colors.LightCoral);
                }
                return new SolidColorBrush(Colors.White);
            }
        }
    }
}
