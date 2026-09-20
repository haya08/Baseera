using System.Text.Json;
using Baseera.Infrastructure.Auth.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/auth/meta")]
public sealed class MetaAuthController : ControllerBase
{
    private const string OAuthStatePrefix = "MetaOAuth:State:";
    private const string ConnectionPrefix = "MetaOAuth:Connection:";

    private readonly IMetaOAuthService _metaOAuthService;
    private readonly IMemoryCache _memoryCache;

    public MetaAuthController(
        IMetaOAuthService metaOAuthService,
        IMemoryCache memoryCache)
    {
        _metaOAuthService = metaOAuthService;
        _memoryCache = memoryCache;
    }

    // =========================================================
    // 1) Start OAuth
    // =========================================================
    [HttpGet("start")]
    public IActionResult Start()
    {
        var requestId = Guid.NewGuid().ToString("N");

        // نسمح بالـ OAuth request لمدة 10 دقائق فقط
        _memoryCache.Set(
            OAuthStatePrefix + requestId,
            true,
            TimeSpan.FromMinutes(10));

        var authorizationUrl =
            _metaOAuthService.BuildAuthorizationUrl(requestId);

        return Ok(new
        {
            requestId,
            authorizationUrl,
            expiresInMinutes = 10
        });
    }

    // =========================================================
    // 2) Meta Callback
    // =========================================================
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(
        [FromQuery] string? code,
        [FromQuery] string? state,
        [FromQuery] string? error,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            return BadRequest(new
            {
                error,
                message = "Meta authorization was cancelled or failed."
            });
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest("Missing authorization code.");
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            return BadRequest("Missing OAuth state.");
        }

        // نتأكد إن الـ request لسه صالح
        var stateKey = OAuthStatePrefix + state;

        if (!_memoryCache.TryGetValue(stateKey, out _))
        {
            return BadRequest("Invalid or expired OAuth state.");
        }

        // بعد نجاح الـ validation نشيل الـ state
        _memoryCache.Remove(stateKey);

        try
        {
            var result =
                await _metaOAuthService.CompleteAuthorizationAsync(
                    code,
                    cancellationToken);

            // نخزن الـ connection مؤقتًا على السيرفر
            // ومش بنرجع أي Access Token للمتصفح
            _memoryCache.Set(
                ConnectionPrefix + state,
                result,
                TimeSpan.FromMinutes(15));

            return Content(
                """
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8" />
                    <title>Meta Connected</title>
                </head>
                <body style="font-family: Arial; padding: 40px;">
                    <h2>Meta connected successfully ✅</h2>
                    <p>Your authorization was completed successfully.</p>
                    <p>You can close this window now.</p>
                </body>
                </html>
                """,
                "text/html");
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                connected = false,
                message = ex.Message
            });
        }
    }

    // =========================================================
    // 3) Finalize connection into YOUR browser session
    // =========================================================
    [HttpGet("connection")]
    public IActionResult GetConnection(
        [FromQuery] string? requestId)
    {
        // -----------------------------------------
        // لو جاي RequestId:
        // هات الـ connection من MemoryCache
        // وحطها في Session بتاعت المتصفح الحالي
        // -----------------------------------------
        if (!string.IsNullOrWhiteSpace(requestId))
        {
            var connectionKey = ConnectionPrefix + requestId;

            if (!_memoryCache.TryGetValue(
                    connectionKey,
                    out MetaOAuthResult? result)
                || result is null)
            {
                return NotFound(new
                {
                    connected = false,
                    message =
                        "No completed Meta connection found for this request. " +
                        "It may have expired or already been used."
                });
            }

            // خزّن الـ connection في Session الخاصة بالمستخدم الحالي
            HttpContext.Session.SetString(
                "MetaConnection",
                JsonSerializer.Serialize(result));

            // One-time handoff
            _memoryCache.Remove(connectionKey);
        }

        // -----------------------------------------
        // بعد الـ Finalize أو لو Session موجودة بالفعل
        // -----------------------------------------
        var json =
            HttpContext.Session.GetString("MetaConnection");

        if (string.IsNullOrWhiteSpace(json))
        {
            return NotFound(new
            {
                connected = false,
                message = "No Meta connection found."
            });
        }

        using var document =
            JsonDocument.Parse(json);

        var pages =
            document.RootElement
                .GetProperty("Pages")
                .Clone();

        return Ok(new
        {
            connected = true,
            pages
        });
    }
}