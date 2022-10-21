using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Core.Comparison.Comparisons;
using Core.Shared.DTOs;
using OconeeCounty.Helpers;

namespace OconeeCounty.Tests;

public class OconeeCountyTest
{
    [Fact(Skip = "Internal Tests")]
    public async Task Test()
    {
        var oconee = new OconeeCounty(new FileCompare());
        var currentFilePath = @"\\anf-fs01\edi\_Carrier_Output_Files\Oconee_Payroll\OconeeDeductionChangeFullFile.csv";
        var previousFilePath =
            @"\\anf-fs01\edi\_Carrier_Output_Files\Oconee_Payroll\Report Archive\OconeeDeductionChangeFullFile_08.19.2022_05.00.44.csv";

        await oconee.CompareFiles(currentFilePath, previousFilePath);
    }
    
    [Fact]
    public void Given_CompletedChangesOnlyOutPutFile_When_FileSaved_Then_UniqueIdNotIncludedInFileOutput()
    {
        var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "Dummy_Output_File.csv");
        var dummyRecords = new List<IhrFullFile>()
        {
            new()
            {
                BenefitName = "Test", EmployeeSSN = "123456789", EmployeeID = "99999", BenefitPlanName = "Test Name"
            }
        };

        ExcelFileHelper.SaveAs(dummyRecords, ExcelFileHelper.OutPutType.Xlsx, filePath,
            new List<string>() { "UniqueId" });

        var text = File.ReadAllText(filePath);

        Assert.True(!text.Contains("UniqueId"));
    }

    [Fact(Skip = "Used for Local Testing")]
    public void Compare_0708_VS_0701()
    {
        // var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,"Dummy_Output_File.csv");
        var filePath = "C:\\Users\\sdoherty\\Desktop\\test_output_oconee_county.xlsx";

        var currentFilePath =
            @"\\anf-fs01\edi\_Carrier_Output_Files\Oconee_Payroll\Audits\07.01 And 07.08 File Duplicates\OconeeDeductionChangeFullFile_07.08.2022.csv";

        var priorFilePath =
            @"\\anf-fs01\edi\_Carrier_Output_Files\Oconee_Payroll\Audits\07.01 And 07.08 File Duplicates\OconeeDeductionChangeFullFile_07.01.2022.csv";

        var fileCompare = new FileCompare();

        var results = new List<IhrFullFile>()
        {
            new()
            {
                BenefitName = "Test", EmployeeSSN = "123456789", EmployeeID = "99999", BenefitPlanName = "Test Name"
            }
        };

        ExcelFileHelper.SaveAs(results, ExcelFileHelper.OutPutType.Xlsx, filePath);

        Assert.True(results.Any());
    }

    [Fact(Skip = "Used for Local Testing")]
    public void Compare_0701_VS_0624()
    {
        var currentFilePath =
            @"\\anf-fs01\edi\_Carrier_Output_Files\Oconee_Payroll\Audit\07.01 And 07.08 File Duplicates\07.08.2022\OconeeDeductionChangeFullFile_07.01.2022.csv";

        var priorFilePath =
            @"\\anf-fs01\edi\_Carrier_Output_Files\Oconee_Payroll\Audit\07.01 And 07.08 File Duplicates\07.08.2022\OconeeDeductionChangeFullFile_06.24.2022.csv";

        var fileCompare = new FileCompare();

        var results = fileCompare.Compare<IhrFullFile>(currentFilePath, priorFilePath,
            OconeeConstants.PremiumPropertyName, OconeeConstants.PropertiesToIgnore);

        var addisonErikaLyn = results.Differences.FirstOrDefault(x => x.UniqueId.Contains("62703652"));


        Assert.True(results.Differences.Any());
    }
}