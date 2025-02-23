namespace EtlApp;

using CommandLine;

public class Options
{
    [Option('c', "config", Required = true, HelpText = "Configuration file.")]
    public required string ApplicationConfigPath { get; set; }

    [Option('i', "include", Default = "*", HelpText = "Regex which Piplines should be processed.")]
    public string? IncludePipeline { get; set; }

    [Option('v', "verbose", Default = false, HelpText = "More logging")]
    public bool Verbose { get; set; } = false;
}