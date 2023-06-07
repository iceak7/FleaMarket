using FleaMarket.Infrastructure;
using FleaMarket.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace FleaMarket.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class GenerateContentController : Controller
    {
        private readonly IOpenApiService _openApiService;
        private readonly IGoogleService _googleService;
        private readonly IUnitOfWork _unitOfWork;
        public GenerateContentController(IOpenApiService openApiService, IUnitOfWork unitOfWork, IGoogleService googleService)
        {
            _openApiService = openApiService;
            _unitOfWork = unitOfWork;
            _googleService = googleService;
        }

        [HttpPost]
        public async Task<JsonResult> TranslateToEnglish(string text)
        {
            try
            {
                var res = await _googleService.TranslateToEn(text);

                if(!string.IsNullOrWhiteSpace(res))
                {
                    return Json(new { success = true, message = res });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false});
            }
        }

        [HttpPost]
        public async Task<JsonResult> GenerateTitle(string description)
        {
            try
            {
                var titles = await _unitOfWork.MarketItems.GetAllTitles();

                var res = await _openApiService.GenerateTitle(description, titles);

                if (!string.IsNullOrWhiteSpace(res))
                {
                    return Json(new { success = true, message = res });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }

}
