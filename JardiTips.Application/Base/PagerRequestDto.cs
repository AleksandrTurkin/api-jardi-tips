
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JardiTips.Application.Base;

public class PagerRequestDto : IPagerParameters
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    [DefaultValue(1)]
    public int? Page { get; set; }

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    [DefaultValue(15)]
    public int? PageSize { get; set; }
}

