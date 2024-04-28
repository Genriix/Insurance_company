﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Insurance_сompany
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private CAPTCHA captcha = new CAPTCHA(); // Инициализируем экземпляр класса капча

        public int user_id; // Создаём Публичную переменную с id пользователя 

        public LoginPage()
        {
            InitializeComponent();
            CapOut.IsEnabled = false; // Делаем текстбокс не активным
            captcha.CaptchaIsGenerate = false; // Мы не проходили капчу
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder(); // Инициализируем экземпляр класса стрингБилдер

            /// Создаём свой цвет для посветки текстбоксов

            var converter = new System.Windows.Media.BrushConverter();
            var lightGray = (Brush)converter.ConvertFromString("#FFABADB3");


            /// Приравниваем публичные поля КапИн и КапАут и 
            /// нашим КапИн и КУапАут (текстбоксы вход и выход капчи)

            captcha.CapOut = CapOut.Text.ToString(); 
            captcha.CapIn = CapIn.Text.ToString();


            string login = Login.Text, password = Password.Password; // Создаём переменные логина и пароля

            /// Ищем пользователя по совпадению логина и пароля в таблице User

            var user = InsuranceCompanyEntities.GetContext().User.FirstOrDefault(u => u.Login == login && u.Password == password); 

            /// Заполняем эклемпляр ошибками если есть

            if (user == null) // Мы не нашли пользователя
            {
                Login.BorderBrush = System.Windows.Media.Brushes.Red;
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                errors.AppendLine("Неправильное имя пользователя или пароль");
            }
            else 
            { 
                Login.BorderBrush = lightGray; 
                Password.BorderBrush = lightGray;
            }

            if (captcha.CaptchaIsGenerate == false) // Капча не была сгенерирована
            { 
                CapIn.BorderBrush = System.Windows.Media.Brushes.Red;
                errors.AppendLine("Пройдите тест CAPTCHA"); 
            }
            else if (captcha.CheckSequence() != true) // Или капча была пройдена не верно
            {
                CapIn.BorderBrush = System.Windows.Media.Brushes.Red;
                errors.AppendLine("Повторите тест CAPTCHA"); 
            }
            else { CapIn.BorderBrush = lightGray; }

            if (errors.Length > 0) //Выводи ошибки если есть
            {
                MessageBox.Show(errors.ToString(), "Ошибка входа" );
                CapOut.Text = "";
                CapIn.Text = "";
                return; // Завершаем исполнение метода и дальше по коду не идём
            }

            /// Если всё ок, и мы не попались на ловушку ошибок, то отчищаем поля и переходим на следующую страницу

            user_id = user.id;
            Manager.MainFrame.Navigate(new UserPage()); 
            Login.Text = "";
            Password.Password = "";
            CapOut.Text = "";
            CapIn.Text = "";
            captcha.CaptchaIsGenerate = false;
        }

        private void GenerateRandomSequence(object sender, RoutedEventArgs e)
        {
            CapOut.Text = captcha.GenerateRandomSequence(); //Записываем в наш текстбокс то, что скажет капча из экземпляра класса
        }
    }
}
