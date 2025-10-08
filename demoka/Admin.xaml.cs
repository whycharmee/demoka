using demoka;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
using System.Windows.Shapes;

namespace demoka
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            using (var context = new demka_dimkiEntities())
            {
                var users = await context.Users.ToListAsync();
                Users.ItemsSource = users;
            }

        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var newUserWindow = new AddUserWindow();
            if (newUserWindow.ShowDialog() == true && newUserWindow.NewUser != null)
            {
                var newUser = newUserWindow.NewUser;

                using (var context = new demka_dimkiEntities())
                {
                    if (await context.Users.AnyAsync(u => u.username == newUser.username))
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        try
                        {
                            context.Users.Add(newUser);
                            await context.SaveChangesAsync();
                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении данных: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        LoadUsers();
                        MessageBox.Show("Пользователь успешно добавлен}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    }

                }
            }
            else
            {
                MessageBox.Show("Добавление пользователя отменено.", "Отмена", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}