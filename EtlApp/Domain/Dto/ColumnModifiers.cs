namespace EtlApp.Domain.Dto;

public enum ColumnModifiers
{
    NonNull,
    Unique,
    Ignore,

    /// <summary>
    /// Identifies the column to be used as a primary key if necessary.
    /// Implies Uniqueness of the column 
    /// </summary>
    PrimaryKey,

    /// <summary>
    /// Marks a column to be used as an identifier for duplicate Entities.
    /// In case of a merge of tables, this is used to identify which rows are updated
    /// </summary>
    DuplicateIdentifier
}