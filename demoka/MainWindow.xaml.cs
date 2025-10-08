using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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

namespace demoka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            using (var context = new demka_dimkiEntities())
            {
                var user = await context.Users
                    .Where(u => u.username == username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    MessageBox.Show("Неправильный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.IsLocked.HasValue && user.IsLocked.Value)
                {
                    MessageBox.Show("Вы заблокированы, обратитесь к администратору.", "Доступ запрещен.", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.LastLoginDate.HasValue && (DateTime.Now - user.LastLoginDate.Value).TotalDays > 30 && user.role != "Admin")
                {
                    user.IsLocked = true;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Ваша учетная запись заблокирована из-за длительного отсутствия", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.password == password)
                {
                    user.LastLoginDate = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Вы успешно авторизировались", "Добро пожаловать!", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (user.IsFirstLogin.HasValue && user.IsFirstLogin.Value)
                    {
                        ChangePassword changePasswordWindow = new ChangePassword(user.id);
                        changePasswordWindow.Owner = this;
                        changePasswordWindow.ShowDialog();
                    }
                    else
                    {
                        if (user.role == "Admin")
                        {
                            Admin adminWindow = new Admin();
                            adminWindow.Show();
                        }
                        else
                        {
                            MainWindow userWindow = new MainWindow();
                            userWindow.Show();
                        }
                        this.Close();
                    }
                }
                else
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts == 3)
                    {
                        user.IsLocked = true;
                        MessageBox.Show("Вы заблокированы после 3 неудачных попыток входа.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        int attemptsLeft = 3 - (user.FailedLoginAttempts ?? 0);
                        MessageBox.Show($"Неправильный логин и пароль. Осталось попыток: {attemptsLeft}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    await context.SaveChangesAsync();
                }
            }
        }

    }
}
