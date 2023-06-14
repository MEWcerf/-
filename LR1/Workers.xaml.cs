using LR1.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Data.Entity.Core.Objects;
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
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Utilities;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.Data.Entity;

namespace LR1
{
    /// <summary>
    /// Логика взаимодействия для Workers.xaml
    /// </summary>
    
    public partial class Workers : Page
    {
        public static WorkersEntities1 DataEntitiesEmployee { get; set; }
        public static ObservableCollection<Employee> ListEmployee;
        public Workers()
        {
            DataEntitiesEmployee = new WorkersEntities1();
            InitializeComponent();
            ListEmployee = new ObservableCollection<Employee>();
        }

        private bool isDirty = true;
        private bool isLoaded = false;
        private void UndoCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isLoaded;
        }

        private void UndoCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RewriteEmployee();
            DataGridEmployee.IsReadOnly = true;
            MessageBox.Show("Отмена");
            isDirty = true;
            isLoaded = false;
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Employee emp = DataGridEmployee.SelectedItem as Employee;
            if (emp !=null)
            {
                MessageBoxResult result = MessageBox.Show("Удалить сотрудника: " + emp.Surname + " " + emp.Patronymic, "Предупреждение", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataEntitiesEmployee.Employees.Remove(emp);
                    DataGridEmployee.SelectedIndex = DataGridEmployee.SelectedIndex == 0 ? 1 : DataGridEmployee.SelectedIndex - 1;
                    ListEmployee.Remove(emp);
                    DataEntitiesEmployee.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления");
            }
            MessageBox.Show("Удаление");
            isDirty = true;
            isLoaded = false;

        }

        private void AddCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void AddCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Entities.Employee employee = Employee.Add(DataGridEmployee.Items.Count + 14, "не задано", "не задано", "не задано", 0);
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

                MessageBox.Show("Создание");
                isDirty = false;
                isLoaded = true;
            }
            catch (DataServiceRequestException ex)
            {
                throw new ApplicationException("Ошибка добавления нового сотрудника в контекст данных" + ex.ToString());
            }
        }

        private void EditCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void EditCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Редактирование");
            DataGridEmployee.IsReadOnly = false;
            DataGridEmployee.BeginEdit();
            isDirty = false;
            isLoaded = true;
        }

        private void FindCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void FindCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Поиск");
            isDirty = false;
            isLoaded = true;
            BorderFind.Visibility = System.Windows.Visibility.Visible;
        }

        private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isLoaded;
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Сохранение");
            DataEntitiesEmployee.SaveChanges();
            isDirty = true;
            DataGridEmployee.IsReadOnly=true;
            isLoaded = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetEmployees();
            DataGridEmployee.ItemsSource = ListEmployee;   
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
            DataEntitiesEmployee = new WorkersEntities1();
            ListEmployee.Clear();
            GetEmployees();
        }

        private void TextBoxSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonFindTitle.IsEnabled = false;
            ButtonFindSurname.IsEnabled = true;
            ComboBoxTitle.Text = "";
        }

        private void ComboBoxTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonFindTitle.IsEnabled = true;
            ButtonFindSurname.IsEnabled = false;
            TextBoxSurname.Text = "";
        }

        private void ButtonFindSurname_Click(object sender, RoutedEventArgs e)
        {
            string surname = TextBoxSurname.Text;
            DataEntitiesEmployee = new WorkersEntities1();
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
                MessageBox.Show("сотрудник с фамилией \n" +  surname+" не найден", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ButtonFindTitle_Click(object sender, RoutedEventArgs e)
        {
            DataEntitiesEmployee = new WorkersEntities1();
            ListEmployee.Clear();
            Title title = ComboBoxTitle.SelectedItem as Title;
            //var queryEmployee = DataEntitiesEmployee.Employees.OrderBy(Employee => Employee.Surname);
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

        private void RefreshCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RewriteEmployee();
            DataGridEmployee.IsReadOnly = false;
        }
    }
}
