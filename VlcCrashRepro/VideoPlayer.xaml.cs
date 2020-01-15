using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace VlcCrashRepro
{
    public partial class VideoPlayer
    {
        private readonly LibVLC _libVLC;
        private VideoView _videoView;

        public VideoPlayer()
        {
            InitializeComponent();
            _libVLC = new LibVLC();
        }

        public void Play(Uri mediaUri)
        {
            Stop();
            if (mediaUri == null)
            {
                return;
            }

            var videoView = new VideoView
            {
                MediaPlayer = new MediaPlayer(_libVLC)
            };
            videoView.Loaded += (sender, args) =>
            {
                using (var media = new Media(_libVLC, mediaUri.AbsoluteUri, FromType.FromLocation))
                {
                    media.AddOption("no-audio");
                    videoView.MediaPlayer.Play(media);
                }
            };
            _videoView = videoView;
            Content = videoView;
        }

        public void Stop()
        {
            if (_videoView == null)
            {
                return;
            }

            var oldPlayer = _videoView.MediaPlayer;
            var oldVideoView = _videoView;
            _videoView = null;
            Content = null;

            //Must be done on UI thread
            oldVideoView.Dispose();

            Task.Run(() =>
            {
                oldPlayer.Dispose();
            });
        }
    }
}
