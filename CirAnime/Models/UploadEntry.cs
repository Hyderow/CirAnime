using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace CirAnime.Models
{
  public class UploadEntry
  {
    public UploadEntry() { }
    public UploadEntry(string title, string originalFileName, User user, DateTime time)
    {
      Title = title;
      OriginalFileName = originalFileName;
      User = user;
      UploadDate = time;
    }
    public int ID { get; set; }
    public string Title { get; set; }
    public string OriginalFileName { get; set; }
    //public int UserID { get; set; }
    public User User { get; set; }

    [DataType(DataType.Date)]
    public DateTime UploadDate { get ; set; }
    public MediaInfo MediaInfo { get; set; }

    public List<ProcessingJob> ProcessingJobs { get; } = new List<ProcessingJob>();
  }
}
