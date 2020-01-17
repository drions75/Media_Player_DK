using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Media.Imaging;



//Ilosc klatek
//Przewijanie strzalkami o 5 sekund

namespace Film_Player
{
    public struct Film
    {

        public string nazwa;
        public string trackPath_1;
        
    }
    
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        bool PlaybackSliderDragging = false;
        String trackPath = "";
        public bool FlagaPlayStop = true;
        public bool FlagaPlaylistaVisability = false;
        public bool FlagaWczytanoPlik = false;
        public double speedRatioValue=1;
        public List<Film> l_filmy = new List<Film>();
        public int nowe = 0;
        

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
                
                int selectedItemIndex = -1;
                selectedItemIndex = playlistBox.SelectedIndex;
                if (selectedItemIndex > -1)
                {
                    TurnPath(selectedItemIndex);
                    PlayPlaylist();
                    Me.Play();
                    PrzelacznikStop_Play();
                }
                   
               
            }
            
        }

        void TurnPath(int selectedItemIndex)
        {
            string tmp_path;
            tmp_path = playlistBox.Items[selectedItemIndex].ToString();
            for (int i = 0; i < l_filmy.Count; i++)
            {
                if (l_filmy[i].nazwa == tmp_path)
                {
                    trackPath = l_filmy[i].trackPath_1;
                }
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
                    TurnPath(selectedItemIndex);//trackPath = playlistBox.Items[selectedItemIndex].ToString();
                    TrackPlay.Content = trackPath;
                    PlayTrack();
                }
            }

        }
        private void PlayTrack()
        {
            Starter.Visibility = Visibility.Hidden;
            Me.Visibility = Visibility.Visible;
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
        private string GetFileInfo()
        {
            
            string dirname;
            string filename;
            string header;
            string data;
            string info = "";
            Shell32.Shell shell = new Shell32.Shell();
            dirname = Path.GetDirectoryName(trackPath);
            filename = Path.GetFileName(trackPath);
            Shell32.Folder folder = shell.NameSpace(dirname);
            Shell32.FolderItem folderitem = folder.ParseName(filename);
            info = filename;
            for (int i = 10; i <= 340; i++)
            {
                header = folder.GetDetailsOf(null, i);
                data = folder.GetDetailsOf(folderitem, i);
                if (!(String.IsNullOrEmpty(header)) && !(String.IsNullOrEmpty(data)))
                {
                    info += $"{header}: {data}\r";
                }
            }
            return info;
        }
        private void MediaInformation_Click(object sender, RoutedEventArgs e)
        {
            string info = "";
            if (String.IsNullOrEmpty(trackPath))
            {
                MessageBox.Show("Nie odtwarzasz aktualnie żadnego wideo");
            }
            else
            {
                info = GetFileInfo();
                Info ip = new Info(info);
                ip.ShowDialog();
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
                Film tmp;
                foreach (string fn in files)
                {
                    
                    tmp.trackPath_1 = fn;
                    tmp.nazwa = Path.GetFileName(fn);
                    playlistBox.Items.Add(tmp.nazwa);
                    l_filmy.Add(tmp);
                    trackPath = tmp.trackPath_1;
                    //playlistBox.Items.Add(fn);
                    //i++ + ". " +
                }

                TrackPlay.Content = trackPath;
                playlistBox.SelectedIndex = 0;
                FlagaWczytanoPlik = true;
                PlayTrack();
                PrzelacznikStop_Play();
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
            if (l_filmy.Count >= 1)
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
              
            
            
        }

        private void Me_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Coś poszło nie tak, nie można odtworzyć pliku " + trackPath + " [" +
                            e.ErrorException.Message + "]");
        }
        private bool IsValidTrack(string aTrack)
        {
            return (aTrack.EndsWith(".mp4") || aTrack.EndsWith(".MP4") || aTrack.EndsWith(".mov"));
        }
        private void Files_Drop(object sender, DragEventArgs e)
        {
           
            string[] trackpaths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (trackpaths != null)
            {
                nowe = trackpaths.Length;
                Film tmp;
               
                for (int i = 0; i < trackpaths.Length; i++)
                {
                    if (IsValidTrack(trackpaths[i]))
                    {
                        
                        tmp.trackPath_1 = trackpaths[i];
                        tmp.nazwa = Path.GetFileName(trackpaths[i]);
                        playlistBox.Items.Add(tmp.nazwa);
                        l_filmy.Add(tmp);
                        
                    }

                    
                }

                //f.nazwa = Path.GetFileName(f.trackPath_1);
                //playlistBox.Items.Add(f.trackPath_1);

                FlagaWczytanoPlik = true;


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
        }

        private void Me_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("ContextMenu") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            l_filmy.Clear();
            playlistBox.Items.Clear();
            Me.Stop();
            Me.Source = null;
            Me.Close();
            FlagaWczytanoPlik = false;
            TrackPlay.Content = "Nie załadowano pliku...";
            TimePlay.Content = "00:00";
            if (StartBtn.Content == "Pause")
            {
                StartBtn.Content = "Start";
            }
            playlistBox.Visibility = Visibility.Hidden;
            Me.Visibility = Visibility.Hidden;
            Starter.Visibility = Visibility.Visible;
            
        }

        private void PlaylistBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("ContextMenu2") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }
    }
}
