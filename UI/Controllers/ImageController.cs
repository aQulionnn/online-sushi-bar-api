using Application.Delegates;
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DAL.Enums;
using DAL.Parameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly CacheServiceResolver  _cacheServiceResolver;

        public ImageController(IOptions<CloudStorageParameters> config, CacheServiceResolver cacheServiceResolver)
        {
            _cacheServiceResolver = cacheServiceResolver;
            var account = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length == 0)
                return BadRequest("No file uploaded");

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(uniqueFileName, stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                return Ok(uniqueFileName);
            else
                return StatusCode(500, "Something went wrong when uploading file");
        }

        [HttpGet]
        [Route("get/{publicId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string publicId)
        {
            var redisCacheService = _cacheServiceResolver(CachingType.Redis);
            
            var cachedImage = await redisCacheService.GetDataAsync<byte[]>($"image:{publicId}");
            if (cachedImage != null)
                return File(cachedImage, "image/jpeg");

            var image = await _cloudinary.GetResourceAsync(publicId);
            if (image == null)
                return NotFound();

            var imageUrl = image.SecureUrl.ToString();
            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

            await redisCacheService.SetDataAsync($"image:{publicId}", imageBytes);
            Response.Headers["Cache-Control"] = "public, max-age=3060";
            Response.Headers["ETag"] = $"\"{publicId}\"";

            return File(imageBytes, "image/jpeg");
        }
    }
}
