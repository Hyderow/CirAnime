using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CirAnime.Models
{
  public enum ProcessingType { PreProcessing, Encode}
  public enum ProcessingStatus { Pending, InProgress, Finished, Failed}
  public class ProcessingJob
  {
    public int ID { get; set; }
    public string OriginalFile { get; set; }
    public UploadEntry UploadEntry { get; set; }
    public ProcessingType Type { get; set; }
    public ProcessingStatus Status { get; set; }
    public DateTime CreationDate { get; set; }
    public int Progress { get; set; }

    
  }
}
