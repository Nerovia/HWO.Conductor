using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareOrchestra.Models.Orchestra
{
    [Serializable]
    public sealed class Sheetmusic
    {
        public Sheetmusic(string path = null, string title = null, List<string> tags = null, bool isFavourite = false, bool isSavedInternally = false)
        {
            Path = path;
            Title = title;
            Tags = tags;
            IsFavourite = isFavourite;
            IsSavedInternally = isSavedInternally;
        }

        public string Path { get; set; }

        public string Title { get; set; }

        public List<string> Tags { get; set; }

        public bool IsFavourite { get; set; }

        public bool IsSavedInternally { get; set; }
    }
}
