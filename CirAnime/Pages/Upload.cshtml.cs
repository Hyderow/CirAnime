using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace CirAnime.Pages
{
  [Authorize]
  [RequestFormLimits(ValueLengthLimit = 1000_000_000, MultipartBodyLengthLimit = 1000_000_000)]
  [RequestSizeLimit(1000_000_000)]
  public class UploadModel : PageModel
  {
    private readonly ILogger<UploadModel> _logger;
    private readonly string uploadPath;


    public UploadModel(ILogger<UploadModel> logger, IConfiguration config)
    {
      _logger = logger;
      uploadPath = config.GetValue<string>("FileUploadPath", "");
      uploadPathName = uploadPath;
    }

    public IActionResult OnGet()
    {
      if (!HttpContext.User.Identity.IsAuthenticated)
        return Redirect("/");
      return Page();
    }

    [BindProperty]
    public FileUploadPhysical FileUpload { get; set; }
    public string uploadPathName { get; set; }

    [HttpPost]
    public async Task<IActionResult> OnPostUploadAsync()
    {
      storeFile();
      var filename = FileUpload.FormFile.FileName;
      var len = System.IO.Directory.GetFiles(uploadPath).Length;
      filename += len;

      using (var fileStream = System.IO.File.Create(uploadPath + filename))
      {

        var memStream = new MemoryStream();

        await FileUpload.FormFile.CopyToAsync(memStream);




        byte[] filecontent = new byte[0];


        await fileStream.WriteAsync(memStream.ToArray());
        memStream.Close();
      }
      return RedirectToPage("./Privacy/uploaded");
      return Page();
    }

    private bool storeFile()
    {
      return false;
    }
    


    public class FileUploadPhysical
    {
      [Required]
      [Display(Name="File")]
      public IFormFile FormFile { get; set; }
    }


  }
}
