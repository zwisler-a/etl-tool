namespace EtlApp.Domain.Dto;

public enum UpdateStrategy
{
    ReplaceComplete,
    ArchiveInTable,
    MergeByUnique,
    Append
}