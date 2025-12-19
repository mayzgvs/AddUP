using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AddUP.Pages
{
    public partial class AdEditPage : Page
    {
        private readonly Entities db = new Entities();
        private readonly Ads _ad; 
        private readonly UserAdsPage _parentPage;

        public AdEditPage(Ads ad, UserAdsPage parent)
        {
            InitializeComponent();
            _ad = ad;
            _parentPage = parent;

            LoadComboboxes();

            if (_ad != null)
            {
                LoadAdData();
                this.Title = "Редактирование объявления";
            }
            else
            {
                this.Title = "Добавление объявления";
            }
        }

        private void LoadComboboxes()
        {
            cmbCity.ItemsSource = db.Cities.ToList();
            cmbCategory.ItemsSource = db.Categories.ToList();
            cmbType.ItemsSource = db.Types.ToList();
            cmbStatus.ItemsSource = db.Statuses.ToList();
        }

        private void LoadAdData()
        {
            txtTitle.Text = _ad.ad_title;
            txtDescription.Text = _ad.ad_description ?? "";
            //txtPrice.Text = _ad.price.ToString("0.00"); 
            cmbCity.SelectedValue = _ad.city_id;
            cmbCategory.SelectedValue = _ad.category_id;
            cmbType.SelectedValue = _ad.type_id;
            cmbStatus.SelectedValue = _ad.status_id;
        }

        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbStatus.SelectedItem is Statuses status && status.status_desc == "Завершено")
            {
                if (_ad == null) return; 

                var oldStatus = db.Statuses.FirstOrDefault(s => s.status_id == _ad.status_id);
                if (oldStatus != null && oldStatus.status_desc == "Активно")
                {
                    var res = MessageBox.Show(
                        "Объявление завершено!\n\nХотите указать полученную сумму?",
                        "Завершение объявления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (res == MessageBoxResult.Yes)
                    {
                        MessageBox.Show(
                            "Сумма зафиксирована как 0 ₽ (по умолчанию).\nПоздравляем с продажей!",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = App.CurrentUser as Users; 

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите заголовок объявления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (целое положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbCity.SelectedValue == null || cmbCategory.SelectedValue == null ||
                cmbType.SelectedValue == null || cmbStatus.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (currentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.GoBack();
                return;
            }

            try
            {
                if (_ad == null) 
                {
                    var newAd = new Ads
                    {
                        ad_title = txtTitle.Text.Trim(),
                        ad_description = txtDescription.Text.Trim(),
                        price = price,
                        city_id = (int)cmbCity.SelectedValue,
                        category_id = (int)cmbCategory.SelectedValue,
                        type_id = (int)cmbType.SelectedValue,
                        status_id = (int)cmbStatus.SelectedValue,
                        user_id = currentUser.user_id, 
                        ad_post_date = DateTime.Now,
                        photo = null
                    };
                    db.Ads.Add(newAd);
                }
                else 
                {
                    _ad.ad_title = txtTitle.Text.Trim();
                    _ad.ad_description = txtDescription.Text.Trim();
                    _ad.price = price;
                    _ad.city_id = (int)cmbCity.SelectedValue;
                    _ad.category_id = (int)cmbCategory.SelectedValue;
                    _ad.type_id = (int)cmbType.SelectedValue;
                    _ad.status_id = (int)cmbStatus.SelectedValue;
                }

                db.SaveChanges();

                MessageBox.Show("Объявление успешно сохранено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentPage?.RefreshAds();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}