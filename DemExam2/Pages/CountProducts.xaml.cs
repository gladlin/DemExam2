using DemExam2.Database;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DemExam2.Pages
{
    /// <summary>
    /// Логика взаимодействия для CountProducts.xaml
    /// </summary>
    public partial class CountProducts : Page
    {
        products current_product;
        public CountProducts(products product)
        {
            InitializeComponent();
            var db = Wallpaper_FactoryEntities.GetContext();

            current_product = product;

            var materials_id = db.product_materials.Where(x => x.product_id == product.product_id).Select(x => x.material_id).ToList();
            if (materials_id.Count == 0)
            {
                MessageBox.Show("Для данного продукта не зафиксированы материалы, нечего рассчитывать");
                return;
            }            cmbMaterialName.ItemsSource = db.materials.Where(m => materials_id.Contains(m.material_id)).ToList();
            cmbMaterialName.DisplayMemberPath = "material_name";
            cmbMaterialName.SelectedValuePath = "material_id";
        }


        private void btnCount_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMaterialName.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать материал!");
                return;
            }

            if (!Int32.TryParse(tbNeedAmount.Text, out int need_product) || need_product <= 0)
            {
                MessageBox.Show("Количество продукции должно быть больше 0 и меньше 2147483647");
                return;
            }

            var db = Wallpaper_FactoryEntities.GetContext();

            var materialId = (int)cmbMaterialName.SelectedValue;
            var current_material = db.materials.First(x => x.material_id == materialId);

            double amount_per_product = db.product_materials.First(x => x.product_id == current_product.product_id && x.material_id == current_material.material_id).amount_of_material;

            tbNeedMaterial.Text = Count_Materials(current_product.product_id, current_material.material_id, need_product, amount_per_product).ToString();

        }

        private int Count_Materials(int productId, int materialId, int need_product, double amount_per_product)
        {
            var db = Wallpaper_FactoryEntities.GetContext();

            var current_material = db.materials.First(x => x.material_id == materialId);
            double need_material = need_product * amount_per_product;
            double percentage = db.material_types.First(x => x.material_type_id == current_material.material_type_id).percentage_of_fail;

            need_material += need_material * percentage;

            double need_buy = need_material - current_material.amount;
            if (need_buy < 0)
            {
                need_buy = 0;
            }

            int result = (int)Math.Ceiling(need_buy);

            return result;
        }

    }
}
