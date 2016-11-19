using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBeePlugin
{
    public class Song : Plugin
    {
        private string[] _Data = new string[0];

        public Song(string sourceFileUrl, MusicBeeApiInterface pApi)
        {
            Url = sourceFileUrl;
            TrackTitle = pApi.Library_GetFileTag(Url, MetaDataType.TrackTitle);
        }

        public string Url { get; }
        public string TrackTitle { get; }
    }
}
