using System.Collections.ObjectModel;
using MusicBeePlugin.Models;

namespace MusicBeePlugin.ViewModels
{
    public class SearchWindowViewModel
    {
        public ObservableCollection<Song> NowPlayingList { get; set; }
        public Song SelectedSong { get; set; }
        public RelayCommand PlaySelectedSong { get; set; }
    }
}
