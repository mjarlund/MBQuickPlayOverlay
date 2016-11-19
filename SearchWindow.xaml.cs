using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MusicBeePlugin.ViewModels;


namespace MusicBeePlugin.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SearchWindow
    {
        public SearchWindow(Plugin.MusicBeeApiInterface pApi)
        {
            InitializeComponent();
            DataContext = new SearchWindowViewModel(pApi);
        }
    }
}
