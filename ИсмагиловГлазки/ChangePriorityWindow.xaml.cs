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
using System.Windows.Shapes;

namespace ИсмагиловГлазки
{
    /// <summary>
    /// Логика взаимодействия для ChangePriorityWindow.xaml
    /// </summary>
    public partial class ChangePriorityWindow : Window
    {
        private List<Agent> _selectedAgents;

        public ChangePriorityWindow(List<Agent> selectedAgents)
        {
            InitializeComponent();
            _selectedAgents = selectedAgents;

            // Устанавливаем максимальный приоритет в поле
            var maxPriority = _selectedAgents.Max(agent => agent.Priority);
            PriorityTextBox.Text = maxPriority.ToString();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PriorityTextBox.Text, out int newPriority))
            {
                if (newPriority < 0)
                {
                    MessageBox.Show("Приоритет не может быть отрицательным. Пожалуйста, введите значение больше или равно нулю.");
                    return;
                }

                // Изменяем приоритет выбранных агентов
                foreach (var agent in _selectedAgents)
                {
                    agent.Priority = newPriority;
                }

                // Сохраняем изменения в базе данных
                ИсмагиловГлазкиSaveEntities.GetContext().SaveChanges();

                // Обновляем интерфейс
                var agentPage = Application.Current.MainWindow.FindName("AgentPage") as AgentPage;
                agentPage?.UpdateAgents();

                // Закрываем окно
                this.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите правильное числовое значение для приоритета.");
            }
        }
    }
}
