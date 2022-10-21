using Core.Comparison.Comparisons;
using Core.Shared.Responses;
using Ionic.Zip;
using OconeeCounty.Helpers;
using OconeeCounty.Models;
using Serilog;
using File = System.IO.File;

namespace OconeeCounty;

public interface IOconeeCounty
{
    Task<CompareFileResponse> CompareFiles();


    Task<string> CompareFiles(string currentFilePath, string previousFilePath);
}

public class OconeeCounty : IOconeeCounty
{
    private FileCompare _fileCompare { get; set; }

    private string _archiveFilePath;
    private string _previousFilePath;

    private string ZipDirectoryFilePath => Path.GetDirectoryName(OconeeConstants.OutputFilePath) + ".zip";

    public OconeeCounty(FileCompare fileCompare)
    {
        _fileCompare = fileCompare;
    }

    public virtual Task<string> CompareFiles(string currentFilePath, string previousFilePath)
    {
        var results =
            _fileCompare.Compare<IhrFullFile>(currentFilePath, previousFilePath, OconeeConstants.PremiumPropertyName,
                    OconeeConstants.PropertiesToIgnore)
                .ChangedRecords ??
            throw new ArgumentNullException(
                "ChangedRecords");

        if (!results.Any()) return Task.FromResult("No Changes Found");

        var currentRecords = ExcelFileHelper.ReadCsvFile<IhrFullFile>(currentFilePath);

        var changedRecords = currentRecords.Where(x => results.Any(y => y == x.UniqueId)).Select(x => x);


        // Save the final output file with Oconee's requested payroll file data
        ExcelFileHelper.SaveAs(changedRecords, ExcelFileHelper.OutPutType.Xlsx, OconeeConstants.OutputFilePath);

        // Zip the file and password protect zip archive
        var newZipDirectoryFilePath = ZipFileWithPassword();

        return Task.FromResult(newZipDirectoryFilePath);
    }

    public virtual Task<CompareFileResponse> CompareFiles()
    {
        _previousFilePath = new DirectoryInfo(OconeeConstants.PreviousFileDirectory).GetFiles("*.csv")
            .OrderByDescending(x => x.LastWriteTime).Select(x => x.FullName).First();

        var results = _fileCompare
            .Compare<IhrFullFile>(OconeeConstants.CurrentFilePath, _previousFilePath,
                OconeeConstants.PremiumPropertyName,
                OconeeConstants.PropertiesToIgnore).ChangedRecords;

        // Save the final output file with Oconee's requested payroll file data
        if (!results.Any())
            return Task.FromResult(new CompareFileResponse("", ComparisonStatus.CompletedWithOutChanges));

        var currentRecords = ExcelFileHelper.ReadCsvFile<IhrFullFile>(OconeeConstants.CurrentFilePath);

        var changedRecords = currentRecords.Where(x => results.Any(y => y == x.UniqueId)).Select(x => x);

        // Save the final output file with Oconee's requested payroll file data
        ExcelFileHelper.SaveAs(changedRecords, ExcelFileHelper.OutPutType.Xlsx, OconeeConstants.OutputFilePath);

        // Zip the file and password protect zip archive
        var newZipDirectoryFilePath = ZipFileWithPassword();

        return Task.FromResult(new CompareFileResponse(newZipDirectoryFilePath, ComparisonStatus.CompletedWithChanges));
    }

    private string ZipFileWithPassword()
    {
        var newZipDirectory = Path.GetDirectoryName(ZipDirectoryFilePath) + "\\" +
                              Path.GetFileNameWithoutExtension(OconeeConstants.OutputFilePath) + ".zip";


        using (var zip = new ZipFile())
        {
            zip.Password = OconeeConstants.OconeeZipPassword;
            // Note: string.Empty is there by instruction of DotNetZip library which prevents any parent directories form being included when zipping
            //		 If you use 'null'  it will include all parent directories.
            zip.AddFile(OconeeConstants.OutputFilePath, string.Empty);
            zip.Save(newZipDirectory);
        }

        return newZipDirectory;
    }
}