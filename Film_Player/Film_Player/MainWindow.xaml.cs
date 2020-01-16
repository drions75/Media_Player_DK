using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

//Ilosc klatek
//Przewijanie strzalkami o 5 sekund

namespace Film_Player
{
    
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        bool PlaybackSliderDragging = false;
        String trackPath = "";
        public bool FlagaPlayStop = true;
        public bool FlagaPlaylistaVisability = false;
        public bool FlagaWczytanoPlik = false;
        public double speedRatioValue=1;
        

        //RenderTargetBitmap bmp = new RenderTargetBitmap(180, 180, 120, 96, PixelFormats.Pbgra32);

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
            if (FlagaWczytanoPlik)
            {
                Me.SpeedRatio = speedRatioValue;
                Me.Volume = VolumeSlider.Value;
                PrzelacznikStop_Play();

                PlaybackTimer();
                
            }
            

            //PlayTrack();
            //Me.Play();
            //Me.Stop();

            //TimePlay.Content ;
            //Me.Play();

        }
        private void Load_Playlist_Double_Click(object sender, MouseButtonEventArgs e)
        {
            if (playlistBox.Items.Count > 0)
            {
               PlayPlaylist();
               Me.Play();
               PrzelacznikStop_Play();
            }
        }
        private void PlayPlaylist()
        {
            int selectedItemIndex = -1;
            if (playlistBox.Items.Count > 0)
            {
                selectedItemIndex = playlistBox.SelectedIndex;
                if (selectedItemIndex > -1)
                {
                    trackPath = playlistBox.Items[selectedItemIndex].ToString();
                    TrackPlay.Content = trackPath;
                    PlayTrack();
                }
            }

        }
        private void PlayTrack()
        {
            bool ok = true;
            FileInfo fi = null;
            Uri src;
            try
            {
                fi = new FileInfo(trackPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ok = false;
            }

            if (ok)
            {
                //Sprawdzam czy plik istnieje
                if (!fi.Exists)
                {
                    MessageBox.Show("Nie odnaleziono " + trackPath);
                }
                else
                {
                    src = new Uri(trackPath);
                    Me.Source = src;
                    // assign the defaults (from slider positions) when a track starts playing
                    Me.SpeedRatio = speedRatioValue;
                    Me.Volume = VolumeSlider.Value;

                    Me.Play();
                    timer.Start();
                    PrzelacznikStop_Play();
                    Me.Pause();

                }
            }
        }
        public void PrzelacznikStop_Play(bool status = true)
        {
            
            FlagaPlayStop = !FlagaPlayStop;
            if (FlagaPlayStop)
            {
                Me.Play();
                StartBtn.Content = "Pause";
            }
            else
            {
                StartBtn.Content = "Start";
                Me.Pause();
            }
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = ".mov";
            dlg.Filter = ".mov|*.mov|.wmv|*.wmv|All files (*.*)|*.*";
            dlg.CheckFileExists = true;
            result = dlg.ShowDialog();
            if (result == true)
            {
                playlistBox.Items.Clear();
                playlistBox.Visibility = Visibility.Hidden;
                trackPath = dlg.FileName;
                TrackPlay.Content = trackPath;
                PlayTrack();
                Me.Play();
                Me.Stop();
                //StartBtn.Content = "Start";
            }
            FlagaWczytanoPlik = true;
        }
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            String folderpath = "";
            string[] files;

            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folderpath = fd.SelectedPath;
            }
            if (folderpath != "")
            {
                playlistBox.Items.Clear();
                playlistBox.Visibility = Visibility.Visible;
                FlagaPlaylistaVisability = true;
                files = Directory.GetFiles(folderpath, "*.mp4");
                int i = 1;
                foreach (string fn in files)
                {
                    
                    playlistBox.Items.Add(fn);
                    //i++ + ". " +
                }
                
                playlistBox.SelectedIndex = 0;
                FlagaWczytanoPlik = true;
            }
        }
       
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FlagaWczytanoPlik)
            {
                //Restart
                Me.Stop();
                Me.Play();
                FlagaPlayStop = false;
                StartBtn.Content = "Start";
                Me.Stop();
            }
            

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
            speedRatioValue = 1;
        }

        private void PlaybackSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            PlaybackSliderDragging = false;
            PlaybackTimer();
            if (FlagaPlayStop)
            {
                Me.Play();
            }
            
        }

        private void PlaybackSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var thumb = sender as Thumb;

                Point pos = e.GetPosition(PlaybackSlider);
                double d = 1.0d / PlaybackSlider.ActualWidth * pos.X;
                PlaybackSlider.Value = PlaybackSlider.Maximum * d;

            }
            PlaybackSliderDragging = true;
            Me.Stop();
        }
      
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void volume_Wheel(object sender, MouseWheelEventArgs e)
        {
            Me.Volume += (e.Delta > 0) ? 0.1 : -0.1;
            VolumeSlider.Value = Me.Volume;
        }

        private void Scaling_Click(object sender, RoutedEventArgs e)
        {
            RenderOptions.SetBitmapScalingMode(Me, BitmapScalingMode.LowQuality);
        }

        private void resetSpeed_Click(object sender, RoutedEventArgs e)
        {
            speedRatioValue = 1;
            SpeedPlay.Content = speedRatioValue;
            Me.SpeedRatio = speedRatioValue;
        }

        private void halfSpeed_Click(object sender, RoutedEventArgs e)
        {
            speedRatioValue = 0.5;
            SpeedPlay.Content = speedRatioValue;
            Me.SpeedRatio = speedRatioValue;
        }

        private void OneHalfSpeed_Click(object sender, RoutedEventArgs e)
        {
            speedRatioValue = 1.5;
            SpeedPlay.Content = speedRatioValue;
            Me.SpeedRatio = speedRatioValue;
        }

        private void twoSpeed_Click(object sender, RoutedEventArgs e)
        {
            speedRatioValue = 2.0;
            SpeedPlay.Content = speedRatioValue;
            Me.SpeedRatio = speedRatioValue;
        }
       
        private void Me_MediaEnded(object sender, RoutedEventArgs e)
        {
            int nextVideoIndex = -1;
            int numberOfVideos = -1;

            Me.Stop();
            PrzelacznikStop_Play();

            numberOfVideos = playlistBox.Items.Count;
            if (numberOfVideos > 0)
            {
                nextVideoIndex = playlistBox.SelectedIndex + 1;
                if (nextVideoIndex >= numberOfVideos)
                {
                    nextVideoIndex = 0;
                }

                playlistBox.SelectedIndex = nextVideoIndex;
                PlayPlaylist();
            }
            
            PrzelacznikStop_Play();
            Me.Play();
            PrzelacznikStop_Play();
        }

        private void playlistButton(object sender, RoutedEventArgs e)
        {
           
                FlagaPlaylistaVisability = !FlagaPlaylistaVisability;
                if (FlagaPlaylistaVisability)
                {
                    playlistBox.Visibility = Visibility.Visible;
                }
                else
                {
                    playlistBox.Visibility = Visibility.Hidden;
                }
            
            
        }

        private void Me_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Coś poszło nie tak, nie można odtworzyć pliku " + trackPath + " [" +
                            e.ErrorException.Message + "]");
        }
        private bool IsValidTrack(string aTrack)
        {
            return (aTrack.EndsWith(".mp4"));
        }
        private void Files_Drop(object sender, DragEventArgs e)
        {
            string x;
            string[] trackpaths = e.Data.GetData(DataFormats.FileDrop) as string[];
            foreach (string s in trackpaths)
            {
                if (IsValidTrack(s))
                {
                    x = Path.GetFileName(s);
                    playlistBox.Items.Add(x);
                    FlagaWczytanoPlik = true;
                }
            }
            if (playlistBox.Items.Count > 0)
            {
                FlagaPlaylistaVisability = true;
                playlistBox.Visibility = Visibility.Visible;
                playlistBox.SelectedIndex = 0;
                trackPath = trackpaths[0];
                TrackPlay.Content = trackPath;
                PlayTrack();
                PrzelacznikStop_Play();
            }
        }

        private void Me_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("ContextMenu") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }
    }
}
