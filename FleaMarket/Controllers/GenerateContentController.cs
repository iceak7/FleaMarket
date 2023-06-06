using FleaMarket.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FleaMarket.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class GenerateContentController : Controller
    {
        private readonly IOpenApiService _openApiService;
        public GenerateContentController(IOpenApiService openApiService)
        {
            _openApiService = openApiService;
        }

        [HttpPost]
        public async Task<JsonResult> TranslateToEnglish(string text)
        {
            try
            {
                var res = await _openApiService.TranslateToEn(text);

                return Json(new {success = true, message = res});
            }
            catch (Exception ex)
            {
                return Json(new { success = false});
            }
        }
    }

}
