using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using MusicBeePlugin;
using System.Collections.ObjectModel;
using MusicBeePlugin.Models;

namespace MusicBeePlugin.ViewModels
{
    public class SearchWindowViewModel : Plugin, INotifyPropertyChanged
    {
        #region Properties
        private MusicBeeApiInterface mbApiInterface
        {
            get { return ApiInterface.mbApiInterface; }
        }
        public ObservableCollection<Song> NowPlayingList { get; set; }
        
        public ObservableCollection<Song> SearchList
        {
            get
            {
                return GetSongsMatchingKey();
            }
        }
        
        public Song SelectedSong
        {
            get; set;
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
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Commands
        public RelayCommand PlaySelectedSongCommand { get; }
        public RelayCommand GetNextSearchResult { get; }
        #endregion


        public SearchWindowViewModel()
        {
            NowPlayingList = LoadNowPlayingQueue();
            
            PlaySelectedSongCommand = new RelayCommand(x =>
            {
                PlayNow(SelectedSong);
            }, x => this.SelectedSong != null);

            GetNextSearchResult = new RelayCommand(x =>
            {
                SearchList.GetEnumerator().MoveNext();
            }, x => SearchList.Count() != 0);
        }

        #region Methods
        private void PlayNow(Song target)
        {
            NowPlayingList_FileActionDelegate FileAction = mbApiInterface.NowPlayingList_PlayNow;
            FileAction(target.Url);
        }

        private ObservableCollection<Song> LoadNowPlayingQueue()
        {
            ObservableCollection<Song> instance = new ObservableCollection<Song>();

            string[] fileUrls = new string[0];
            mbApiInterface.NowPlayingList_QueryFilesEx(null, ref fileUrls);

            foreach (var url in fileUrls)
            {
                instance.Add(new Song(url));
            }

            return instance;
        }
        

        private ObservableCollection<Song> GetSongsMatchingKey()
        {
            ObservableCollection<Song> instance = new ObservableCollection<Song>();
            var query = TextBoxSearch;

            if (NowPlayingList.Count() == 0 || string.IsNullOrEmpty(query))
            {
                return instance;
            }

            try
            {
                NowPlayingList.Any(x => x.TrackTitle.StartsWith(query, StringComparison.CurrentCultureIgnoreCase));
            }
            catch(ArgumentNullException)
            {
                return instance;
            }

            foreach(Song song in NowPlayingList)
            {
                if (song.TrackTitle.StartsWith(query, StringComparison.CurrentCultureIgnoreCase))
                    instance.Add(song);

            }

            return instance;
        }

        #endregion
        
    }
}
