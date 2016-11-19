using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using MusicBeePlugin;
using System.Collections.ObjectModel;

namespace MusicBeePlugin.ViewModels
{
    public class SearchWindowViewModel : Plugin, INotifyPropertyChanged
    {
        private MusicBeeApiInterface mbApiInterface;
        public ObservableCollection<Song> NowPlayingList { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public RelayCommand PlaySelectedSongCommand { get; }

        private void PlayNow(Song target)
        {
            NowPlayingList_FileActionDelegate FileAction = mbApiInterface.NowPlayingList_PlayNow;
            FileAction(target.Url);
        }
        public ObservableCollection<Song> SearchList
        {
            get { return GetSongsMatchingKey(TextBoxSearch); }
        }

        public Song SelectedSong
        {
            get
            {
                if(SearchList.Count != 0)
                {
                    return SearchList.First();
                }

                return null;
            }
        }

        public SearchWindowViewModel(MusicBeeApiInterface pApi)
        {
            mbApiInterface = pApi;
            NowPlayingList = LoadNowPlayingQueue();

            PlaySelectedSongCommand = new RelayCommand(x =>
            {
                if (SelectedSong != null) PlayNow(SelectedSong);
            }, x => this.SelectedSong != null);
        }

        private ObservableCollection<Song> LoadNowPlayingQueue()
        {
            ObservableCollection<Song> instance = new ObservableCollection<Song>();

            string[] fileUrls = new string[0];
            mbApiInterface.NowPlayingList_QueryFilesEx(null, ref fileUrls);

            foreach (var url in fileUrls)
            {
                instance.Add(new Song(url, mbApiInterface));
            }

            return instance;
        }

        private ObservableCollection<Song> GetSongsMatchingKey(string key)
        {
            ObservableCollection<Song> instance = new ObservableCollection<Song>();

            if(NowPlayingList.Count() == 0)
            {
                return instance;
            }

            try
            {
                NowPlayingList.Any(x => x.TrackTitle.StartsWith(key, StringComparison.CurrentCultureIgnoreCase));   
            }
            catch(ArgumentNullException)
            {
                return instance;
            }

            foreach(Song song in NowPlayingList)
            {
                if (song.TrackTitle.StartsWith(key, StringComparison.CurrentCultureIgnoreCase))
                    instance.Add(song);
            }

            return instance;
        }
        
        private string _TextBoxSearch;
        public string TextBoxSearch
        {
            get { return _TextBoxSearch; }
            set
            {
                _TextBoxSearch = value;
                OnPropertyChanged();
                OnPropertyChanged("SearchList");
                OnPropertyChanged("SelectedSong");
            }
        }
        
    }
}
