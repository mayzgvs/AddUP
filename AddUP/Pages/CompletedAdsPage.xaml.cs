// Pages/CompletedAdsPage.xaml.cs
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AddUP.Pages
{
    public partial class CompletedAdsPage : Page
    {
        private readonly Entities db = new Entities();

        public CompletedAdsPage()
        {
            InitializeComponent();

            var completedAds = db.Ads
                .Where(a => a.Statuses.status_desc == "Завершено")
                .ToList();

            LWcompleted.ItemsSource = completedAds;

            if (!completedAds.Any())
            {
                MessageBox.Show("Пока нет завершённых объявлений.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}