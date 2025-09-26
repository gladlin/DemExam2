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
using DemExam2.Database;

namespace DemExam2.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private List<ProductStruct> productList;

        public ProductsPage()
        {
            InitializeComponent();

            productList = new List<ProductStruct>();
            AllProducts();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
           NavigationService.Navigate(new AddProductPage());
        }

        private void LViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Если выбран продукт, то показываем кнопку редактирования продукта
            if (LViewProduct.SelectedItem != null)
            {
                btnChangeProduct.Visibility = Visibility.Visible;
                btnCountMaterial.Visibility = Visibility.Visible;
            }
            else
            {
                btnChangeProduct.Visibility = Visibility.Hidden;
                btnCountMaterial.Visibility = Visibility.Hidden;
            }
        }
        private void btnChangeProduct_Click(object sender, RoutedEventArgs e)
        {
            // переход на страницу изменения продукта 
            if (LViewProduct.SelectedItem is ProductStruct selectedItem)
            {
                Wallpaper_FactoryEntities db = Wallpaper_FactoryEntities.GetContext();
                var product = db.products.First(x => x.article == selectedItem.Articule);
                NavigationService.Navigate(new ChangeProductPage(product));
            }
            else
                return;
        }

        private void btnCountMaterial_Click(object sender, RoutedEventArgs e)
        {
            // переход на страницу подсчета количества материала
            if (LViewProduct.SelectedItem is ProductStruct selectedItem)
            {
                Wallpaper_FactoryEntities db = Wallpaper_FactoryEntities.GetContext();
                var product = db.products.First(x => x.article == selectedItem.Articule);
                NavigationService.Navigate(new CountProducts(product));
            }
            else
                return;
        }

        public struct ProductStruct
        {
            public string ProductType { get; set; }
            public string ProductName { get; set; }
            public int Articule { get; set; }
            public double MinPrice { get; set; }
            public double Wide { get; set; }
            public double Price { get; set; }
        }

        /// <summary>
        /// Заполняет все карточки партнёров
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        /// <param name="partners">Массив всех работников в бд</param>
        private void AllProducts()
        {
            Wallpaper_FactoryEntities db = Wallpaper_FactoryEntities.GetContext();

            var products = db.products.ToList();
            foreach (var item in products) {
                var product = db.products.First(x => x.product_id == item.product_id);
                string product_type = db.product_types.First(x => x.product_type_id == item.product_type_id).product_type_name;
                var product_materials = db.product_materials.ToList();

                double price = 0;
                foreach (var item2 in product_materials)
                {
                    if (item2.product_id == item.product_id)
                    {
                        price += db.materials.First(x => x.material_id == item2.material_id).price * item2.amount_of_material;
                    }
                }

                ProductStruct ProductStruct = new ProductStruct()
                {
                    ProductType = product_type,
                    ProductName = item.name_of_product,
                    Articule = item.article,
                    MinPrice = item.min_price,
                    Wide = item.wide,
                    Price = price
                };
                productList.Add(ProductStruct);
            }
            LViewProduct.ItemsSource = productList;
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem is ProductStruct selectedItem)
            {
                Wallpaper_FactoryEntities db = Wallpaper_FactoryEntities.GetContext();
                var product = db.products.First(x => x.article == selectedItem.Articule);
                NavigationService.Navigate(new ChangeProductPage(product));
            }
            else
                return;
        }
    }
}
