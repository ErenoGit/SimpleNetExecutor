using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;

namespace SimpleNetExecutor.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class ModulesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ModulesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("moduleMd5")]
        public async Task<IActionResult> GetModuleMd5(string endpointId)
        {
            var endpoint = await _db.Endpoints.FirstOrDefaultAsync(e => e.EndpointId == endpointId);

            if (endpoint == null)
            {
                endpoint = new Endpoint
                {
                    EndpointId = endpointId,
                    LastEndpointHeartbeat = DateTime.UtcNow
                };

                _db.Endpoints.Add(endpoint);
            }
            else
            {
                endpoint.LastEndpointHeartbeat = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            var latest = await _db.Modules.OrderByDescending(x => x.Id).FirstOrDefaultAsync();

            return Ok(latest?.ModuleMd5 ?? "");
        }

        [HttpGet("moduleDll")]
        public async Task<IActionResult> GetModuleDll(string moduleMd5)
        {
            var dll = await _db.Modules.FirstOrDefaultAsync(x => x.ModuleMd5 == moduleMd5);

            if (dll == null)
                return NotFound();

            return File(dll.ModuleDll, "application/octet-stream");
        }

        [HttpPost("moduleDll")]
        public async Task<IActionResult> UploadDll(string secretKey, IFormFile file)
        {
            if (secretKey != HttpContext.RequestServices.GetRequiredService<IConfiguration>()["MySettings:Password"])
                return Unauthorized();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var moduleMd5 = ComputeMd5Hex(ms);

            if(_db.Modules.Any(x => x.ModuleMd5 == moduleMd5))
                return Ok();

            var dll = new Module
            {
                ModuleMd5 = moduleMd5,
                ModuleDll = ms.ToArray()
            };

            _db.Modules.Add(dll);
            await _db.SaveChangesAsync();

            return Ok();
        }

        private string ComputeMd5Hex(MemoryStream ms)
        {
            ms.Position = 0;

            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(ms);

            ms.Position = 0;

            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}