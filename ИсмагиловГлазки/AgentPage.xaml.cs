using System;
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

namespace ИсмагиловГлазки
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        public AgentPage()
        {
            InitializeComponent();
            //добавляем строки
            //загрузить в список из бд
            var currentAgents = ИсмагиловГлазкиSaveEntities.GetContext().Agent.ToList();
            //связать с нашим листвью
            AgentListView.ItemsSource = currentAgents;
            //добавили строки

            ///*ComboType*/.SelectedIndex = 0;
            //FilterComboBox.SelectedIndex = 0;

            //вызываем UpdateAgents();
            UpdateAgents();

        }
        private void UpdateAgents()
        {

            // Получаем всех агентов из базы данных
            var currentAgents = ИсмагиловГлазкиSaveEntities.GetContext().Agent.ToList();
            currentAgents = currentAgents.Where(p => p.Title.ToLower().Contains(SearchTextBox.Text.ToLower()) || p.Email.ToLower().Contains(SearchTextBox.Text.ToLower()) ||
            p.Phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("+", "")
            .Contains(SearchTextBox.Text.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("+", ""))).ToList();

            switch (ComboType.SelectedIndex)
            {
                case 1:
                    currentAgents = currentAgents.OrderBy(agent => agent.Title).ToList();
                    break;
                case 2:
                    currentAgents = currentAgents.OrderByDescending(agent => agent.Title).ToList();
                    break;
                case 5:
                    currentAgents = currentAgents.OrderBy(agent => agent.Priority).ToList();
                    break;
                case 6:
                    currentAgents = currentAgents.OrderByDescending(agent => agent.Priority).ToList();
                    break;
                default:
                    break;
            }

            if (FilterComboBox != null)
            switch (FilterComboBox.SelectedIndex)
            {
                case 1:
                    currentAgents=currentAgents.ToList();
                        break;
                case 2:
                    currentAgents = currentAgents.Where(agent => agent.AgentTypeTitle == "ЗАО").ToList();
                    break;
                case 3:
                    currentAgents = currentAgents.Where(agent => agent.AgentTypeTitle == "МКК").ToList();
                    break;
                case 4:
                    currentAgents = currentAgents.Where(agent => agent.AgentTypeTitle == "МФО").ToList();
                    break;
                case 5:
                    currentAgents = currentAgents.Where(agent => agent.AgentTypeTitle == "ОАО").ToList();
                    break;
                case 6:
                    currentAgents = currentAgents.Where(agent => agent.AgentTypeTitle == "ООО").ToList();
                    break;
                case 7:
                    currentAgents = currentAgents.Where(agent => agent.AgentTypeTitle == "ПАО").ToList();
                    break;
                default:
                    break;
            }


            // Сортировка по ComboType
            var selectedSort = (ComboType.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedSort == "Наименование по возрастанию")
            {
                currentAgents = currentAgents.OrderBy(agent => agent.Title).ToList();
            }
            else if (selectedSort == "Наименование по убыванию")
            {
                currentAgents = currentAgents.OrderByDescending(agent => agent.Title).ToList();
            }


            if (selectedSort == "Приоритет по возрастанию")
            {
                currentAgents = currentAgents.OrderBy(agent => agent.Priority).ToList();
            }
            else if (selectedSort == "Приоритет по убыванию")
            {
                currentAgents = currentAgents.OrderByDescending(agent => agent.Priority).ToList();
            }

            // Обновляем источник данных для ListView
            if (AgentListView != null)
            AgentListView.ItemsSource = currentAgents;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
            {
                Manager.MainFrame.Navigate(new AddEditPage());
            }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();

        }

       

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();

        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();

        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();

        }

        
    }
}
