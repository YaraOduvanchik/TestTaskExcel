using Microsoft.AspNetCore.Mvc;
using MoscowWeatherArchives.Application;
using MoscowWeatherArchives.Models;

namespace MoscowWeatherArchives.Controllers;

[Route("{controller}")]
public class WeatherController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        [FromServices] GetAllWeatherArchivesWithPaginationHandler handler,
        GetAllWeatherArchivesWithPaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);

        return View(result);
    }

    [HttpGet("Upload")]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(
        List<IFormFile> files,
        [FromServices] UploadWeatherArchiveHandler handler,
        CancellationToken cancellationToken)
    {
        if (files == null || files.Count == 0)
            return BadRequest("Файлы не выбраны");

        var statusMessages = new List<string>();

        foreach (var file in files)
        {
            if (file.Length == 0)
            {
                statusMessages.Add($"Файл {file.FileName} пуст");
                continue;
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                byte[] fileBytes = memoryStream.ToArray();
                using var newMemoryStream = new MemoryStream(fileBytes);
                
                var result = await handler.Handle(memoryStream, file.FileName, cancellationToken);

                if (result.IsSuccess) statusMessages.Add($"Файл {file.FileName} успешно загружен");
                else statusMessages.Add($"Ошибка: {result.Error}");
            }
            catch (Exception ex)
            {
                statusMessages.Add($"Неизвестная ошибка при загрузке файла {file.FileName}");
            }
        }

        ViewBag.UploadStatus = string.Join("<br>", statusMessages);
        return View();
    }
}