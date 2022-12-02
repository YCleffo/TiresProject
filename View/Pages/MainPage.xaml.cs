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
        
        int countElement = 10;
        int page = 1;
      
        List<ProductType> productTypes;
        bool reverseType;
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
            DisplayPagination(1);
            UpdateUI();
        }


        private void UpdateUI()
        {
            List<Product> displayProduct = GetRows();
           
          
           
            if (GetRows().Count > 10)
            {
                DisplayPagination(page);
               
                displayProduct = GetRows().Skip((page - 1) * countElement).Take(countElement).ToList();
                foreach(var item in displayProduct){
                    Console.WriteLine(item.ID);
                }
                PaginationListView.Visibility = Visibility.Visible;
            }
            else
            {
              
                PaginationListView.Visibility = Visibility.Collapsed; 
            }
            ProductListView.ItemsSource = displayProduct;
            CountRowsTextBlock.Text = $"Количество {displayProduct.Count()}/ {db.context.Product.ToList().Count()}";
        }

   

        private List<Product> GetRows()
        {
            List<Product> arrayProduct = db.context.Product.ToList();
            string searchData = FindTextBox.Text.ToUpper();
            if (!String.IsNullOrEmpty(FindTextBox.Text))
            {
                 arrayProduct = arrayProduct.Where(x => x.Title.ToUpper().Contains(searchData)).ToList();
                 arrayProduct = arrayProduct.Where(x => LevenshteinDistance(x.Title.ToUpper(), searchData)<=12).ToList();
            }

            if (ComboBoxFiltr.ItemsSource != null & ComboBoxFiltr.SelectedIndex == 0)
            {
                arrayProduct = arrayProduct.ToList();
            }
            else
            {
                arrayProduct = arrayProduct.Where(x => x.ProductTypeID == ComboBoxFiltr.SelectedIndex).ToList();
            }
            int value = ComboBoxSort.SelectedIndex;
            if (value== 0)
            {
                arrayProduct = arrayProduct.OrderBy(p => p.Title).ToList();
            }
            else if(value == 1)
            {
                arrayProduct = arrayProduct.OrderBy(p => p.ProductionWorkshopNumber).ToList();
            }
            else if (value == 2)
            {
                arrayProduct = arrayProduct.OrderBy(p => p.CostProduct).ToList();
            }
            if (reverseType)
            {
                arrayProduct.Reverse();
            }
            return arrayProduct;
        }

        private int GetPagesCount()
        {
            int countPage = 0;
            
            int count = GetRows().Count();
            if (count > countElement)
            {
                countPage = Convert.ToInt32(Math.Ceiling(count * 1.0 / countElement));
            }
            return countPage;
        }

        private void DisplayPagination(int page)
        {
            List<PageItem> source = new List<PageItem>();
            for(int i=1; i<= GetPagesCount(); i++)
            {
                source.Add(new PageItem(i, i == page));
            }
            PaginationListView.ItemsSource = source;
            PrevTextBlock.Visibility = (page <= 1 ? Visibility.Hidden : Visibility.Visible);
            NextTextBlock.Visibility = (page >= GetPagesCount() ? Visibility.Hidden : Visibility.Visible);
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

        private void ComboBoxSortSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }


        private void ReverseButtonClick(object sender, RoutedEventArgs e)
        {
            reverseType = !reverseType;
            UpdateUI();
        }


        private void TextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock activePage = (TextBlock)sender;
            page = Convert.ToInt32(activePage.Text);
            UpdateUI();
        }



        private void PrevTextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (page <= 1)
            {
                page = 1;
                PrevTextBlock.Visibility = Visibility.Visible;
                UpdateUI();
            }
            else
            {
                page -= 1;
                PrevTextBlock.Visibility = Visibility.Visible;
                UpdateUI();
            }
        }


        private void NextTextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (page >= GetPagesCount())
            {
                page =GetPagesCount();
                NextTextBlock.Visibility = Visibility.Hidden;
                UpdateUI();
            }
            else
            {
                page += 1;
                PrevTextBlock.Visibility = Visibility.Visible;
                UpdateUI();
            }
        }
    }
}
