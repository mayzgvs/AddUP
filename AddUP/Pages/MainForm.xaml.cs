using System;
using System.Collections.Generic;
using System.IO;
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

namespace AddUP.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : Page
    {
        public MainForm()
        {
            InitializeComponent();
            LWads.ItemsSource = Entities.GetContext().Ads.ToList();
            cmbCategory.ItemsSource = Entities.GetContext().Categories.ToList();
            cmbStatus.ItemsSource = Entities.GetContext().Statuses.ToList();
            cmbType.ItemsSource = Entities.GetContext().Types.ToList();
            cmbCity.ItemsSource = Entities.GetContext().Cities.ToList();
        }

        private void UpdateAds()
        {
            var currentAds = Entities.GetContext().Ads.ToList();

            if (!string.IsNullOrWhiteSpace(TBsearch.Text))
            {
                string searchText = TBsearch.Text.ToLower();
                currentAds = currentAds.Where(a =>
                    a.ad_title.ToLower().Contains(searchText) ||
                    a.ad_description.ToLower().Contains(searchText)
                ).ToList();
            }

            if (cmbCategory.SelectedItem != null)
            {
                currentAds = currentAds.Where(a =>
                    a.category_id == cmbCategory.SelectedIndex + 1
                ).ToList();
            }

            if (cmbCity.SelectedItem != null)
            {
                currentAds = currentAds.Where(a =>
                    a.city_id == cmbCity.SelectedIndex + 1
                ).ToList();
            }

            if (cmbStatus.SelectedItem != null)
            {
                currentAds = currentAds.Where(a =>
                    a.status_id == cmbStatus.SelectedIndex + 1
                ).ToList();
            }

            if (cmbType.SelectedItem != null)
            {
                currentAds = currentAds.Where(a =>
                    a.type_id == cmbType.SelectedIndex + 1
                ).ToList();
            }

            LWads.ItemsSource = currentAds;
        }

        private void TBsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAds();
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }
        private void GoToCompletedAds_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CompletedAdsPage());
        }
        private void BackupDatabase_Click(object sender, RoutedEventArgs e)
        {
            string studentNumber = "4";
            string lastName = "Ватаманюк";

            string fileName = $"{studentNumber}_РКБД_{lastName}.bak";

            string safeFileName = $"{studentNumber}_RKBD_{lastName}.bak";

            string backupPath = @"C:\DatabaseBackups";

            try
            {
                if (!System.IO.Directory.Exists(backupPath))
                {
                    System.IO.Directory.CreateDirectory(backupPath);
                }
            }
            catch
            {
                backupPath = System.IO.Path.GetTempPath();
            }

            string fullPath = System.IO.Path.Combine(backupPath, fileName);

            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                string databaseName = "";

                using (var db = new Entities())
                {
                    var connection = db.Database.Connection;
                    databaseName = connection.Database;

                    connection.Open();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $@"
                    BACKUP DATABASE [{databaseName}] 
                    TO DISK = @backupPath 
                    WITH INIT, 
                    NAME = 'Backup_{studentNumber}_{lastName}',
                    DESCRIPTION = 'Created by AdsApp',
                    COMPRESSION";

                        var param = cmd.CreateParameter();
                        param.ParameterName = "@backupPath";
                        param.Value = fullPath;
                        cmd.Parameters.Add(param);

                        cmd.ExecuteNonQuery();
                    }
                }

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullPath);
                    string message = $@"Резервная копия успешно создана!
         Папка: {backupPath}
         Файл: {fileName}
         Размер: {fileInfo.Length / 1024} KB
         Время: {DateTime.Now:HH:mm:ss}

          Файл готов для сдачи преподавателю";

                    MessageBox.Show(message, "Резервная копия создана",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", backupPath);
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("Файл не был создан. Проверьте права доступа к диску C:\\",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                string errorMessage = "";

                switch (sqlEx.Number)
                {
                    case 5:
                        errorMessage = "Отказано в доступе к папке.\n" +
                                      "Попробуйте запустить программу от имени администратора.";
                        break;
                    case 3201:
                        errorMessage = $"Не могу записать файл по пути:\n{fullPath}\n\n" +
                                      "Возможные решения:\n" +
                                      "1. Запустите программу от имени администратора\n" +
                                      "2. Выберите другую папку для сохранения";
                        break;
                    default:
                        errorMessage = $"Ошибка SQL #{sqlEx.Number}: {sqlEx.Message}";
                        break;
                }

                MessageBox.Show(errorMessage, "Ошибка SQL Server",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            TBsearch.Text = "";
            cmbStatus.SelectedItem = null;
            cmbCategory.SelectedItem = null;
            cmbType.SelectedItem = null;
            cmbCity.SelectedItem = null;
            LWads.ItemsSource = Entities.GetContext().Ads.ToList();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            
            NavigationService.Navigate(new LoginPage());
        }
    }
}
