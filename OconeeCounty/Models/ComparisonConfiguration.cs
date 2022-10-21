namespace OconeeCounty.Models;

public record ComparisonConfiguration(string? PreviousFilePath,
    string? CurrentFilePath,
    int NumberOfDaysAgoPreviousFileSent,
    int MaxDifferences = 10,
    bool TreatStringEmptyAndNullTheSame = true,
    bool CaseSensitive = false,
    List<string>? MembersToIgnore = null,
    List<string>? MembersToInclude = null);