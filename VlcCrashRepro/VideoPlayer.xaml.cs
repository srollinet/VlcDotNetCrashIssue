using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Forms;

namespace VlcCrashRepro
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        private static readonly TimeSpan StreamWatchDogInterval = TimeSpan.FromSeconds(10);

        // ReSharper disable once AssignNullToNotNullAttribute
        // ReSharper disable once PossibleNullReferenceException
        private static readonly DirectoryInfo LibDirectory = new DirectoryInfo(Path.Combine(
            new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName,
            "libvlc",
            IntPtr.Size == 4 ? "win-x86" : "win-x64"));

        private VideoPlayerViewModel ViewModel { get; }

        private readonly DispatcherTimer _streamWatchdogTimer = new DispatcherTimer();
        private readonly VlcControl _vlcControl;

        private readonly object _vlcControlLock = new object();

        public VideoPlayer(VideoPlayerViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();

            _streamWatchdogTimer.Interval = StreamWatchDogInterval;
            _streamWatchdogTimer.Tick += (sender, args) =>
            {
                var mediaUri = ViewModel.MediaUri;
                if (mediaUri == null || _vlcControl.VlcMediaPlayer.IsPlaying())
                {
                    ViewModel.HasError = false;
                    return;
                }

                ViewModel.HasError = true;
                Console.WriteLine($"mediaPlayer {ViewModel.Index} is not playing stream {mediaUri}, attempting to restart it.");
                Play(mediaUri);
            };
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;

            _vlcControl = new VlcControl();
            PlayerContainer.Child = _vlcControl;
            _vlcControl.BeginInit();
            _vlcControl.VlcLibDirectory = LibDirectory;
            _vlcControl.EndInit();

            _vlcControl.VlcMediaPlayer.Log += MediaPlayerOnLog;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VideoPlayerViewModel.MediaUri):
                    Play(ViewModel.MediaUri);
                    break;
            }
        }

        private void Play(Uri mediaUri)
        {
            //If invoked from the main thread it deadlocks sometimes.
            Task.Run(() =>
            {
                lock (_vlcControlLock)
                {
                    _streamWatchdogTimer.Stop();
                    _vlcControl.Stop();
                    if (mediaUri == null)
                    {
                        return;
                    }

                    _vlcControl.Play(mediaUri, "no-audio");
                    _streamWatchdogTimer.Start();
                }
            });
        }

        private void Stop()
        {
            //If invoked from the main thread it deadlocks sometimes.
            Task.Run(() =>
            {
                lock (_vlcControlLock)
                {
                    _streamWatchdogTimer.Stop();
                    _vlcControl.Stop();
                }
            });
        }

        private void MediaPlayerOnLog(object sender, VlcMediaPlayerLogEventArgs e)
        {
            Console.WriteLine($"mediaPlayer {ViewModel.Index} {e.Module}: {e.Message}");
        }
    }
}
