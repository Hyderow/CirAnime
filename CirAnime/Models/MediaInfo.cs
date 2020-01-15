using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CirAnime.Models
{
  public class MediaInfo
  {
    public int ID { get; set; }
    public string title { get; set; }
    public int duration { get; set; }
    public bool live { get; set; }
    public string thumbnail { get; set; }
    public virtual List<Source> sources { get; } = new List<Source>();
    public virtual List<TextTrack> textTracks { get; } = new List<TextTrack>();
  }
  public class Source
  {
    public int ID { get; set; }
    public string url { get; set; }
    public string contentType { get; set; }
    public int quality { get; set; }
    public int bitrate { get; set; }

   // public int MediaInfoId { get; set; }
   // public MediaInfo MediaInfo { get; set; }
  }

  public class TextTrack
  {
    public int ID { get; set; }
    public string url { get; set; }
    public string contentType { get; set; }
    public string name { get; set; }
    //public int MediaInfoId { get; set; }
    //public MediaInfo MediaInfo { get; set; }
  }
}
