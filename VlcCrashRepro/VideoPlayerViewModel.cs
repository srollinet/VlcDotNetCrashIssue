using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VlcCrashRepro.Annotations;

namespace VlcCrashRepro
{
    public class VideoPlayerViewModel : INotifyPropertyChanged
    {
        private Uri _mediaUri;
        private bool _hasError;
        public int Index { get; }

        public Uri MediaUri
        {
            get => _mediaUri;
            private set
            {
                _mediaUri = value;
                OnPropertyChanged();
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }

        public VideoPlayerViewModel(int index)
        {
            Index = index;
        }

        public void AffectMedia(Uri mediaUri)
        {
            HasError = false;
            MediaUri = mediaUri;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}