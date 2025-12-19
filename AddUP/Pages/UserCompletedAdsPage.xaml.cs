using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AddUP.Pages
{
    public partial class UserCompletedAdsPage : Page
    {
        private readonly Entities db = new Entities();

        public UserCompletedAdsPage()
        {
            InitializeComponent();
            LoadCompletedAds();
        }

        private void LoadCompletedAds()
        {
            var currentUser = App.CurrentUser as Users; 

            if (currentUser == null)
            {
                NavigationService?.GoBack();
                return;
            }

            var completedAds = db.Ads
                .Where(a => a.user_id == currentUser.user_id
                          && a.status_id == 2) 
                .ToList();

            LWcompleted.ItemsSource = completedAds;

            if (!completedAds.Any())
            {
                MessageBox.Show("У вас пока нет завершённых объявлений.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new UserAdsPage());
        }
    }
}