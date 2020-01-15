using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CirAnime.Models
{
  public class User
  {
    public int ID { get; set; }
    public UInt64 DiscordID { get; set; }
    public string Name { get; set; }

    public List<UploadEntry> UploadEntrys { get; } = new List<UploadEntry>();
  }
}
