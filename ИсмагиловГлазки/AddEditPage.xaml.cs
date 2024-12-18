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
using Microsoft.Win32;
using ИсмагиловГлазки;


namespace ИсмагиловГлазки
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    /// 


    public partial class AddEditPage : Page
    {
        private Agent currentAgent = new Agent();

        public AddEditPage(Agent selectedAgent = null)
        {
            InitializeComponent();

            if (selectedAgent != null)
            {
                currentAgent = selectedAgent; // Редактирование существующего агента
            }
            else
            {
                currentAgent = new Agent(); // Создание нового агента
            }

            // Установите DataContext для привязки данных
            DataContext = currentAgent;

        }


        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                //относительный путь
                currentAgent.Logo=myOpenFileDialog.FileName;
                //загружаем в элемент картинку
                LogoImage.Source=new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Валидация данных
            if (string.IsNullOrWhiteSpace(currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()) || currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Сохранение данных
            try
            {
                if (currentAgent.ID == 0)
                {
                    // Новый агент
                    ИсмагиловГлазкиSaveEntities.GetContext().Agent.Add(currentAgent);
                }
                else
                {
                    // Обновление существующего агента
                    var existingAgent = ИсмагиловГлазкиSaveEntities.GetContext().Agent
                        .FirstOrDefault(a => a.ID == currentAgent.ID);

                    if (existingAgent != null)
                    {
                        existingAgent.Title = currentAgent.Title;
                        existingAgent.Address = currentAgent.Address;
                        existingAgent.DirectorName = currentAgent.DirectorName;
                        existingAgent.AgentTypeID = currentAgent.AgentTypeID;
                        existingAgent.Priority = currentAgent.Priority;
                        existingAgent.INN = currentAgent.INN;
                        existingAgent.KPP = currentAgent.KPP;
                        existingAgent.Phone = currentAgent.Phone;
                        existingAgent.Email = currentAgent.Email;
                        existingAgent.Logo = currentAgent.Logo;
                    }
                }

                ИсмагиловГлазкиSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }



        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем наличие информации о реализации продукции
            if (currentAgent.ProductSale.Any())
            {
                MessageBox.Show("Удаление невозможно: у агента есть информация о реализации продукции.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Подтверждение удаления
            var result = MessageBox.Show($"Вы уверены, что хотите удалить агента {currentAgent.Title}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем связанные данные
                    ИсмагиловГлазкиSaveEntities.GetContext().AgentPriorityHistory.RemoveRange(currentAgent.AgentPriorityHistory);
                    ИсмагиловГлазкиSaveEntities.GetContext().Shop.RemoveRange(currentAgent.Shop);

                    // Удаляем агента
                    ИсмагиловГлазкиSaveEntities.GetContext().Agent.Remove(currentAgent);
                    ИсмагиловГлазкиSaveEntities.GetContext().SaveChanges();

                    MessageBox.Show("Агент успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Возвращаемся к списку агентов
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении агента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}

