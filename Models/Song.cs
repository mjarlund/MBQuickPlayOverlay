using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBeePlugin.Models
{
    public class Song : Plugin
    {
        private MusicBeeApiInterface mbApiInterface
        {
            get { return ApiInterface.mbApiInterface; }
        }

        public Song(string sourceFileUrl)
        {
            Url = sourceFileUrl;
            TrackTitle = mbApiInterface.Library_GetFileTag(Url, MetaDataType.TrackTitle);
        }

        public string Url { get; }
        public string TrackTitle { get; }
    }
}
