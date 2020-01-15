using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using LibVLCSharp.Shared;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace VlcCrashRepro
{
    public partial class VideoPlayer
    {
        private readonly LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;

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

            var media = new Media(_libVLC, mediaUri.AbsoluteUri, FromType.FromLocation);
            media.AddOption("no-audio");
            _mediaPlayer = new MediaPlayer(_libVLC);
            VideoView.MediaPlayer = _mediaPlayer;
            _mediaPlayer.Play(media);
        }

        public void Stop()
        {
            VideoView.MediaPlayer = null;
            var toDispose = _mediaPlayer;
            _mediaPlayer = null;
            Task.Run(() =>
            {
                toDispose?.Dispose();
            });
        }
    }
}
