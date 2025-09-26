using DemExam2.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DemExam2.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChangeProductName.xaml
    /// </summary>
    public partial class ChangeProductPage : Page
    {
        products selectedProduct;
        public ChangeProductPage(products product)
        {
            InitializeComponent();

            selectedProduct = product;
            var db = Wallpaper_FactoryEntities.GetContext();

            //Заполнение комбобокса
            cmbProductType.ItemsSource = db.product_types.Select(x => x.product_type_name).ToList();

            //Подгрузка из бд выбранного типа продукта
            cmbProductType.SelectedItem = db.product_types.First(x => x.product_type_id == product.product_type_id).product_type_name;

            tbProductName.Text = product.name_of_product;
            tbArticul.Text = product.article.ToString();
            tbMinPrice.Text = product.min_price.ToString();
            tbWide.Text = product.wide.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProductType.SelectedItem == null || tbProductName.Text == "" || tbArticul.Text == "" || tbMinPrice.Text == "" || tbWide.Text == "")
            {
                MessageBox.Show("Заполните все поля формы, прежде чем ее сохранить!");
                return;
            }

            var db = Wallpaper_FactoryEntities.GetContext();
            products product = db.products.First(x => x.product_id == selectedProduct.product_id);

            product.product_type_id = db.product_types.First(x => x.product_type_name == cmbProductType.SelectedItem.ToString()).product_type_id;
            product.name_of_product = tbProductName.Text;
            int out_new = -1;
            Int32.TryParse(tbArticul.Text, out out_new);
            if(out_new > 0)
            {
                product.article = out_new;
                out_new = -1;
            }
            else
            {
                MessageBox.Show("Значение артикула должно быть больше 0 и меньше 2147483647");
                return;
            }
            Int32.TryParse(tbMinPrice.Text, out out_new);
            if (out_new > 0)
            {
                product.min_price = out_new;
            }
            else
            {
                MessageBox.Show("Значение минимальной цены должно быть больше 0 и меньше 2147483647");
                return;
            }
            double new_out = -1;
            Double.TryParse(tbWide.Text, out new_out);
            if (new_out > 0.0)
            {
                product.wide = new_out;
            }
            else
            {
                MessageBox.Show("Значение ширины рулона должно быть больше 0 и меньше 2147483647");
                return;
            }

            //валидация
            var context = new ValidationContext(product);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var productValid = Validator.TryValidateObject(product, context, results, true);

            //вывод ошибок пользователя
            if (!productValid)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in results)
                    sb.AppendLine(error.ToString());

                MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Успешное изменение данных о продукте", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new ProductsPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}
