using System;
using WpfLaba1.Entities;
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
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects;

namespace WpfLaba1
{
    /// <summary>
    /// Логика взаимодействия для Sotrudniki.xaml
    /// </summary>
    public partial class Sotrudniki : Page
    {
        public static TitlePersonalEntities DataEntitiesEmployee { get; set; }

        public static ObservableCollection<Employee> ListEmployee;
        public Sotrudniki()
        {

            DataEntitiesEmployee = new TitlePersonalEntities();
            InitializeComponent();
            ListEmployee = new ObservableCollection<Employee>();

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetEmployees();
            DataGridEmployee.SelectedIndex = 0;

        }

        private void GetEmployees()
        {
            var queryEmployee = DataEntitiesEmployee.Employees.OrderBy(Employee => Employee.Surname);
            foreach (Employee emp in queryEmployee)
            {
                ListEmployee.Add(emp);
            }
            DataGridEmployee.ItemsSource = ListEmployee;
        }

        private void RewriteEmployee()
        {
            DataEntitiesEmployee = new TitlePersonalEntities();
            ListEmployee.Clear();
            GetEmployees();
        }







        private bool isDirty = true;

        private void UndoCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RewriteEmployee();
            DataGridEmployee.IsReadOnly = true;
            isDirty = true;
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataEntitiesEmployee.SaveChanges();
            isDirty = true;
            DataGridEmployee.IsReadOnly = true;
        }
        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Employee emp = DataGridEmployee.SelectedItem as Employee;
            if (emp != null)
            {
                MessageBoxResult result = MessageBox.Show("Удалить сотрудника: " +  emp.Surname + " " + emp.Name + " " + emp.Patronymic, "Предупреждение", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataEntitiesEmployee.DeleteObject(emp);
                    DataGridEmployee.SelectedIndex = DataGridEmployee.SelectedIndex == 0 ? 1 : DataGridEmployee.SelectedIndex - 1;
                    ListEmployee.Remove(emp);
                    DataEntitiesEmployee.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления");
            }
        }
        private void EditCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataGridEmployee.IsReadOnly = false;
            DataGridEmployee.BeginEdit();
            isDirty = false;
        }
        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Employee employee = Employee.CreateEmployee(-1, "не задано", "не задано", "не задано", 0);
            employee.Telephone = "не задано";
            employee.Email = "не задано";
            try
            {
                DataEntitiesEmployee.Employees.Add(employee);
                ListEmployee.Add(employee);
                DataGridEmployee.ScrollIntoView(employee);
                DataGridEmployee.SelectedIndex = DataGridEmployee.Items.Count - 1;
                DataGridEmployee.Focus();
                DataGridEmployee.IsReadOnly = false;
                isDirty = false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка добавления нового сотрудника в контекст данных" + ex.ToString());
            }

        }
        private void FindCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Найти");
            BorderFind.Visibility = System.Windows.Visibility.Visible;
        }


        private void EditCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }
        private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        private void TextBoxSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonFindSurname.IsEnabled = true;
            ButtonFindTitle.IsEnabled = false;
            ComboBoxTitle.SelectedIndex = -1;
        }

        private void ButtonFindSurname_Click(object sender, RoutedEventArgs e)
        {
            string surname = TextBoxSurname.Text;
            DataEntitiesEmployee = new TitlePersonalEntities();
            ListEmployee.Clear();
            var queryEmployee = from employee in DataEntitiesEmployee.Employees
                                where employee.Surname == surname
                                select employee;
            foreach (Employee emp in queryEmployee)
            {
                ListEmployee.Add(emp);
            }
            if (ListEmployee.Count > 0)
            {
                DataGridEmployee.ItemsSource = ListEmployee;
                ButtonFindSurname.IsEnabled = true;
                ButtonFindTitle.IsEnabled = false;
            }
            else
                MessageBox.Show("Сотрудник с фамилией \n" + surname + "\n не найдан", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ComboBoxTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonFindTitle.IsEnabled = true;
            ButtonFindSurname.IsEnabled = false;
            TextBoxSurname.Text = "";
        }

        private void ButtonFindTitle_Click(object sender, RoutedEventArgs e)
        {
            DataEntitiesEmployee = new TitlePersonalEntities();
            ListEmployee.Clear();

            Title title = ComboBoxTitle.SelectedItem as Title;
            var queryEmployee = from employee in DataEntitiesEmployee.Employees
                                where employee.TitleID == title.ID
                                orderby employee.Surname
                                select employee;
            foreach (Employee emp in queryEmployee)
            {
                ListEmployee.Add(emp);
            }
            DataGridEmployee.ItemsSource = ListEmployee;
        }





        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
