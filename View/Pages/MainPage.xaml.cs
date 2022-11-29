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
using vehicle.Models;


namespace vehicle.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        Core db = new Core();
        List<ProductType> productTypes;
        public MainPage()
        {
            InitializeComponent();
            List<string> sortTypeList = new List<string>()
            {
                "наименование", "остаток на складе", "стоимость"
            };
            ComboBoxSort.ItemsSource = sortTypeList;
            productTypes = new List<ProductType>
            {
                new ProductType()
                {
                    ID = 0,
                    Title = "Все типы"
                }
            };
            productTypes.AddRange(db.context.ProductType.ToList());
            ComboBoxFiltr.ItemsSource = productTypes;
            UpdateUI();
        }

        private void UpdateUI()
        {
            List<Product> displayProduct = GetRows();
            //ProductListView.ItemsSource = db.context.Product.ToList();
            ProductListView.ItemsSource = displayProduct;
        }

        private List<Product> GetRows()
        {
            List<Product> arrayProduct = db.context.Product.ToList();
            string searchData = FindTextBox.Text.ToUpper();
            if (!String.IsNullOrEmpty(FindTextBox.Text))
            {
                 arrayProduct = arrayProduct.Where(x => x.Title.ToUpper().Contains(searchData)).ToList();
                // arrayProduct = arrayProduct.Where(x => LevenshteinDistance(x.Title.ToUpper(), searchData)<=6).ToList();
            }

            if (ComboBoxFiltr.ItemsSource != null & ComboBoxFiltr.SelectedIndex == 0)
            {
                arrayProduct = arrayProduct.ToList();
            }
            else
            {
                arrayProduct = arrayProduct.Where(x => x.ProductTypeID == ComboBoxFiltr.SelectedIndex).ToList();
            }
            
            return arrayProduct;
        }

        public static int LevenshteinDistance(string source1, string source2)
        {
     
            /// <summary>
            ///     Calculate the difference between 2 strings using the Levenshtein distance algorithm
            /// </summary>
            /// <param name="source1">First string</param>
            /// <param name="source2">Second string</param>
            /// <returns></returns>

                var source1Length = source1.Length;
                var source2Length = source2.Length;

                var matrix = new int[source1Length + 1, source2Length + 1];

                // First calculation, if one entry is empty return full length
                if (source1Length == 0)
                    return source2Length;

                if (source2Length == 0)
                    return source1Length;

                // Initialization of matrix with row size source1Length and columns size source2Length
                for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
                for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

                // Calculate rows and collumns distances
                for (var i = 1; i <= source1Length; i++)
                {
                    for (var j = 1; j <= source2Length; j++)
                    {
                        var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                        matrix[i, j] = Math.Min(
                            Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                            matrix[i - 1, j - 1] + cost);
                    }
                }
                // return result
                return matrix[source1Length, source2Length];
            
        
    }

        private void FindTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            FindTextBox.Text = "";
        }


        private void FindTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUI(); 
        }



        private void ComboBoxFiltrSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }
    }
}
