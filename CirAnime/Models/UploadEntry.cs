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
    public UploadEntry(string title, string originalFileName, string owner, DateTime time)
    {
      Title = title;
      OriginalFileName = originalFileName;
      Owner = owner;
      UploadDate = time;
    }
    public int ID { get; set; }
    public string Title { get; set; }
    public string OriginalFileName { get; set; }
    public string Owner { get; set; }

    [DataType(DataType.Date)]
    public DateTime UploadDate { get ; set; }
  }
}
