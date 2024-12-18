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
        int CountRecords;//Кол-во записей в таблице
        int CountPage;//Общее кол-во страниц
        int CurrentPage = 0;//Текущая страница

        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;
        public AgentPage()
        {
            InitializeComponent();

            // Загрузка данных из базы
            TableList = ИсмагиловГлазкиSaveEntities.GetContext().Agent.ToList();

            // Установка ComboBox в начальное состояние
            ComboType.SelectedIndex = 0;
            FilterComboBox.SelectedIndex = 0;

            // Установка пагинации и отображение первой страницы
            ChangePage(0, 0); // Инициализация первой страницы
        }
        private void UpdateAgents()
        {

            // Получаем всех агентов из базы данных
            var currentAgents = ИсмагиловГлазкиSaveEntities.GetContext().Agent.ToList();
            TableList = currentAgents;
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
            TableList = currentAgents;
            ChangePage(0, 0);

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

        

        private void AddAgentButton_Click(object sender, RoutedEventArgs e)
        {
            // Открыть страницу добавления нового агента
            Manager.MainFrame.Navigate(new AddEditPage()); // null означает добавление нового агента
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));//кнопка редактирования
        }
       
        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0,Convert.ToInt32(PageListBox.SelectedItem.ToString())-1);
        }
        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }
        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);

        }
        private void ChangePage(int direction, int? selectedPage)//Функция отвечающая за разделение list'а
        {
            //direction - направление, 0 - начало, 1 - предыдущая страница, 2 - следующая страница
            //selectedPage - при нажатии на стрелочки передаётся null,
            //при выборе определённой страницы в этой переменной находится номер страницы
            

            CurrentPageList.Clear();//начальная очистка листа
            CountRecords = TableList.Count;//определение количества записей во всём списке
            //определение кол-ва страниц
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;
            //Проверка на правильность - если 
            //CurrentPage(номер текущей страницы) "правильный"

            int min;

            if (selectedPage.HasValue)//Проверка на значение не null (т.к. может быть null)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else//если нажата стрелка
            {
                switch (direction)
                {
                    case 1://нажата кнопка ""Предыдущая страница"
                        if (CurrentPage > 0)//то есть кнопка нажата правильно и "назад" можно идти
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                            //в случаях если CurrentPage попытается выйти из диапазона внесение данных не произойдёт
                        }
                        break;

                    case 2://нажата кнопка "следующая страница"
                        if (CurrentPage < CountPage - 1)//если вперёд идти можно
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)//если currentPage не вышел из диапазона, то
            {
                if (PageListBox != null)
                {
                    PageListBox.Items.Clear();
                    for (int i = 1; i <= CountPage; i++)
                    {
                        PageListBox.Items.Add(i);
                    }

                    PageListBox.SelectedIndex = CurrentPage;
                }



                //вывод количества записей на странице и общего количества
                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                if(TBCount!=null)
                TBCount.Text = min.ToString();
                if (TBallRecords != null)
                TBallRecords.Text = " из " + CountRecords.ToString();
                if (AgentListView != null)
                AgentListView.ItemsSource = CurrentPageList;
                //обновить отображение списка услуг
                if (AgentListView != null)

                AgentListView.Items.Refresh();
            }
        }
    }
}