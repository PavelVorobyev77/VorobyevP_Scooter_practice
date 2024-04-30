using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using VorobyevP_Scooter_practice.db;

namespace VorobyevP_Scooter_practice.windows
{
    public partial class StationStatWindow : Window
    {
        private station_usage_statistics selectedStat;

        public StationStatWindow()
        {
            InitializeComponent();

            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnEdit.Click += BtnEdit_Click;

            using (var db = new VorobyevP_Scooter_practiceEntities())
            {
                station_usage_statistics.ItemsSource = db.station_usage_statistics.Include("charging_station").OrderBy(x=>x.id_statistics).ToList();
            }

            station_usage_statistics.SelectionChanged += Station_usage_statistics_SelectionChanged;
        }

        private void Station_usage_statistics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStat = (station_usage_statistics)station_usage_statistics.SelectedItem;
            if (selectedStat != null)
            {
                tbIDStation.Text = selectedStat.id_station.ToString();
                dpDate.SelectedDate = selectedStat.date;
                tbTotalUses.Text = selectedStat.total_uses.ToString();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStat == null)
            {
                MessageBox.Show("Статистика не выбрана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Вы уверены, что хотите изменить данные для статистики {selectedStat.id_statistics}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = new VorobyevP_Scooter_practiceEntities())
                {
                    var s = db.station_usage_statistics.Where(x => x.id_statistics == selectedStat.id_statistics).First();

                    s.id_station = int.Parse(tbIDStation.Text);
                    s.date = dpDate.SelectedDate.Value;
                    s.total_uses = int.Parse(tbTotalUses.Text);

                    db.SaveChanges();
                    Station_usage_statistics_SelectionChanged(null, null);
                    MessageBox.Show("Статистика обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Невозможно изменить статистику {selectedStat.id_statistics}, возможно, дублируется ID станции\n\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы уверены, что хотите добавить новую статистику?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = new VorobyevP_Scooter_practiceEntities())
                {
                    var s = new station_usage_statistics();

                    s.id_station = int.Parse(tbIDStation.Text);
                    s.date = dpDate.SelectedDate.Value;
                    s.total_uses = int.Parse(tbTotalUses.Text);

                    db.station_usage_statistics.Add(s);

                    db.SaveChanges();
                    Station_usage_statistics_SelectionChanged(null, null);
                    MessageBox.Show("Добавлена новая статистика", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Невозможно добавить статистику {selectedStat?.id_statistics}, возможно, дублируется ID станции\n\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStat == null)
            {
                MessageBox.Show("Статистика не выбрана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить статистику с ID {selectedStat.id_statistics}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                using (var db = new VorobyevP_Scooter_practiceEntities())
                {
                    var stat = db.station_usage_statistics.Where(x => x.id_statistics == selectedStat.id_statistics).First();
                    db.station_usage_statistics.Remove(stat);
                    db.SaveChanges();

                    MessageBox.Show("Статистика успешно удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Station_usage_statistics_SelectionChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Невозможно удалить статистику, возможно, имеются связанные с ней записи.\n\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            StationStatWindow newstatWindow = new StationStatWindow();
            newstatWindow.Show();
            Close();
        }
    }
}