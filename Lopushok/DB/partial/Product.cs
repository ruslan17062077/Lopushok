using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    MemoryStream byteStreame = new MemoryStream(Image);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = byteStreame;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
                else
                {
                    return null;
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
                return totalcost;
            }
        }
    }
}
