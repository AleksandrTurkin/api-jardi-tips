using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace JardiTips.Application.Base;

public record PagedCursor(DateTime DateTime, Guid LastId)
{
    public static string Encode(DateTime date, Guid lastId)
    {
        var cursor = new PagedCursor(date, lastId);
        var json = JsonSerializer.Serialize(cursor);
        
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(json));
    }

    public static PagedCursor? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;
        
        try
        {
            var json = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(cursor));
            return JsonSerializer.Deserialize<PagedCursor>(json);
        }
        catch
        {
            return null;
        }
    }
}

