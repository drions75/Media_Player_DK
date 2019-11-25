using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Film_Player
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        bool PlaybackSliderDragging = false;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += new EventHandler(Timer_Tick);
        }

        void PlaybackTimer()
        {
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if(!PlaybackSliderDragging)
            {
                PlaybackSlider.Value = Me.Position.TotalMilliseconds;
                if (Me.NaturalDuration.HasTimeSpan)
                    TimePlay.Content = String.Format("{0} / {1}", Me.Position.ToString(@"mm\:ss"), Me.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            //Me.SpeedRatio = SpeedSlider.Value;
            Me.Volume = VolumeSlider.Value;
            PlaybackTimer();
            //TimePlay.Content ;
            Me.Play();
            
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Me.Stop();
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            Me.Pause();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Me.Volume = VolumeSlider.Value;
        }

        private void PlaybackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(PlaybackSliderDragging)
            {
                Me.Position = TimeSpan.FromMilliseconds(PlaybackSlider.Value);
            }
        }

        private void Me_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlaybackSlider.Maximum = Me.NaturalDuration.TimeSpan.TotalMilliseconds;
            //SpeedSlider.Value =1;
        }

        private void PlaybackSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            PlaybackSliderDragging = false;
            PlaybackTimer();
            Me.Play();
        }

        private void PlaybackSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PlaybackSliderDragging = true;
            Me.Stop();
        }

       
    }
}
