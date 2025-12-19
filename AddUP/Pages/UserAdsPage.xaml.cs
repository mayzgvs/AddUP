using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AddUP.Pages
{
    public partial class UserAdsPage : Page
    {
        private readonly Entities db = new Entities();

        public UserAdsPage()
        {
            InitializeComponent();
            LoadAds();
        }

        private void LoadAds()
        {
            var currentUser = App.CurrentUser as Users;

            if (currentUser == null)
            {
                NavigationService.GoBack();
                return;
            }

            var ads = db.Ads
                .Where(a => a.user_id == currentUser.user_id)
                .ToList();
            dgAds.ItemsSource = ads;
        }

        private void AddAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdEditPage(null, this)); 
        }

        private void dgAds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgAds.SelectedItem is Ads selectedAd)
            {
                NavigationService.Navigate(new AdEditPage(selectedAd, this));
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is int adId)) return;
            var ad = db.Ads.Find(adId);
            if (ad == null) return;
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить объявление «{ad.ad_title}»?\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    db.Ads.Remove(ad);
                    db.SaveChanges();
                    MessageBox.Show("Объявление удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAds();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainForm());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }

        private void GoToMyCompleted_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserCompletedAdsPage());
        }

        public void RefreshAds() => LoadAds();
    }
}