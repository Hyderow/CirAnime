using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CirAnime.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
        {
          _logger = logger;
          _logger.LogWarning(config.GetValue<string>("Discord:AppId"));
        }
        public void OnGet()
        {

        }
    }
}