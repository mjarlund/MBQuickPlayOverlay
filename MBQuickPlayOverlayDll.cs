using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.ComponentModel;
using MusicBeePlugin.Views;
using MusicBeePlugin.ViewModels;
using System.Linq;
using System.Collections.ObjectModel;
using MusicBeePlugin.Models;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        private SearchWindowViewModel swViewModel = new SearchWindowViewModel();

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "QuickPlay Overlay";
            about.Description = "Bind to a global hotkey to quickly search for and play tracks in your NowPlayingList without having to open the MusicBee UI.";
            about.Author = "Mikkel Jarlund";
            about.TargetApplication = "";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = 1;  // your plugin version
            about.VersionMinor = 0;
            about.Revision = 3;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
            createMenuItem();

            return about;
        }

        private void InitialisePlugin()
        {
            swViewModel.NowPlayingList = GetNowPlayingList();
            swViewModel.PlaySelectedSong = new RelayCommand(x =>
            {
                PlayNow(swViewModel.SelectedSong);
            }, x => swViewModel.SelectedSong != null);

        }

        private void createMenuItem()
        {
            mbApiInterface.MB_AddMenuItem("mnuTools/QuickPlay Overlay", "QuickPlay Overlay", openSearchWindow);
        }

        private void openSearchWindow(object sender, EventArgs args)
        {
            SearchWindow window = new SearchWindow();
            window.DataContext = swViewModel;
            WindowInteropHelper helper = new WindowInteropHelper(window);
            ElementHost.EnableModelessKeyboardInterop(window);
            
            window.Show();
            window.Activate();
            window.Deactivated += closeSearchWindow;
        }

        private void closeSearchWindow(object sender, EventArgs e)
        {
            var window = sender as SearchWindow;
            window.Close();
        }

        public bool Configure(IntPtr panelHandle)
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            // panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            // keep in mind the panel width is scaled according to the font the user has selected
            // if about.ConfigurationPanelHeight is set to 0, you can display your own popup window
            if (panelHandle != IntPtr.Zero)
            {
                Panel configPanel = (Panel)Panel.FromHandle(panelHandle);
                Label prompt = new Label();
                prompt.AutoSize = true;
                prompt.Location = new Point(0, 0);
                prompt.Text = "prompt:";
                TextBox textBox = new TextBox();
                textBox.Bounds = new Rectangle(60, 0, 100, textBox.Height);
                configPanel.Controls.AddRange(new Control[] { prompt, textBox });
            }
            return false;
        }
       
        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall()
        {
        }

        // receive event notifications from MusicBee
        // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // perform some action depending on the notification type
            switch (type)
            {
                case NotificationType.PluginStartup:
                    // perform startup initialisation
                    InitialisePlugin();
                    break;
                case NotificationType.NowPlayingListChanged:
                    swViewModel.NowPlayingList = GetNowPlayingList();
                    break;
            }
        }

        private ObservableCollection<Song> GetNowPlayingList()
        {
            ObservableCollection<Song> instance = new ObservableCollection<Song>();

            string[] fileUrls = new string[0];
            mbApiInterface.NowPlayingList_QueryFilesEx(null, ref fileUrls);

            foreach (var url in fileUrls)
            {
                Song song = new Song();
                song.Url = url;
                song.Name = mbApiInterface.Library_GetFileTag(url, MetaDataType.TrackTitle);
                instance.Add(song);
            }

            return new ObservableCollection<Song>(instance.OrderBy(x => x.Name));
        }

        private void PlayNow(Song target)
        {
            NowPlayingList_FileActionDelegate FileAction = mbApiInterface.NowPlayingList_PlayNow;
            FileAction(target.Url);
        }

    }
}