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
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SempahoreWithThreads
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Semaphore Semaphore { get; set; }
        public ObservableCollection<Thread> Threads { get; set; } = new ObservableCollection<Thread>();
        public Thread ThreadP { get; set; }
        private static int _serialNumThread = 0;
        private int _semaphoreCount = 0;
        private int _createdThreadCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            Semaphore = new Semaphore(_semaphoreCount, _semaphoreCount, "Semaphore");

            for (int i = 0; i < _createdThreadCount; i++)
            {

                ThreadP = new Thread(() => WorkingThreads(Semaphore));
                _serialNumThread++;
                ThreadP.Name = "Thread --> " + _serialNumThread;
                Threads.Add(ThreadP);
            }
        }


        private void WorkingThreads(Semaphore s)
        {
            bool st = false;

            while (!st)
            {
                if (s.WaitOne(1))
                {
                    try
                    {
                        var t = Thread.CurrentThread;
                        Dispatcher.Invoke(() => lbWorkingThreads.Items.Add(t));
                        Dispatcher.Invoke(() => lbWaitingThreads.Items.Remove(t));
                        Thread.Sleep(5000);
                    }
                    finally
                    {
                        st = true;
                        var t = Thread.CurrentThread;
                        Dispatcher.Invoke(() => lbWorkingThreads.Items.Remove(t));
                        Dispatcher.Invoke(() => lbWaitingThreads.Items.Remove(t));
                        s.Release();
                    }
                }
                else
                {
                    var t = Thread.CurrentThread;
                    Dispatcher.Invoke(() =>
                    {
                        if (!lbWaitingThreads.Items.Contains(t) && !lbWorkingThreads.Items.Contains(t))
                            lbWaitingThreads.Items.Add(t);
                    });

                }

            }
        }

        private void LbCreatedThreads_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var t = (lbCreatedThreads.SelectedItem as Thread);
                t.Start();
                lbWaitingThreads.Items.Add(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NudSemaphorePlaces_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _semaphoreCount = Convert.ToInt32(nudSemaphorePlaces.Value);
        }

        private void btnClearThreads_Click(object sender, RoutedEventArgs e)
        {
            if (!lbCreatedThreads.Items.IsEmpty)
            {
                _serialNumThread = 0;
            }
            Threads.Clear();
        }

        private void nudCreatedThreadCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _createdThreadCount = Convert.ToInt32(nudCreatedThreadCount.Value);
        }

    }
}

