using System.Diagnostics;
using System.Text.RegularExpressions;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;

namespace EtlApp.Adapter.BuildIn.Transformation;

public class RegexReplaceTransformerConfig : TransformerConfig
{
    public string SelectRegex { get; set; }
    public string ReplaceRegex { get; set; }
}

public class RegexReplaceTransformer(RegexReplaceTransformerConfig config) : ITransformer
{
    public object? Transform(object? value)
    {
        if (value == null || string.IsNullOrEmpty(config.SelectRegex))
        {
            return value;
        }

        var inputString = value.ToString();
        var regex = new Regex(config.SelectRegex);
        Debug.Assert(inputString != null, nameof(inputString) + " != null");
        return regex.Replace(inputString, config.ReplaceRegex);
    }
}