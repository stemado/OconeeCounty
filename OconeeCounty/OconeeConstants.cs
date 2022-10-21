namespace OconeeCounty;

public class OconeeConstants
{
    public static readonly Guid OconeeCountyFlowJobId = Guid.Parse("3fa85f64-8717-4562-b3fc-2c963f66afa6");

    public static readonly string CurrentFilePath =
        @"\\anf-fs01\EDI\_Carrier_Output_Files\Oconee_Payroll\OconeeDeductionChangeFullFile.csv";

    public static readonly string PreviousFileDirectory =
        @"\\anf-fs01\EDI\_Carrier_Output_Files\Oconee_Payroll\Report Archive";

    public static readonly string OutputFilePath =
        $@"\\anf-fs01\EDI\_Carrier_Output_Files\Oconee_Payroll\Final File Archive\OconeeDeductionChange {DateTime.Now.Month}.{DateTime.Now.Day}.{DateTime.Now.Year}.xlsx";

    public static string PremiumPropertyName = "EmployeeCostPerPay";


    public static string OconeeCountyFileName = "OconeeDeductionChangeFullFile.csv";
    public static string OconeeZipPassword = "OconeeCountySchools";

    public static List<string> PropertiesToIgnore = new()
    {
        "SORTTHISCOLUMN",
        "ElectionConfirmedDate",
        "ExportReason",
        "DeductionCode",
        "EmployeeSSN",
        "EmployeeID",
        "EmployeeStatus",
        "FullNameLastFirstMiddle",
        "HireDate",
        "BenefitName",
        "BenefitPlanName",
        "EffectiveDate",
        "ExpirationDate",
        "PostTaxDeductionPerPay",
        "PreTaxDeductionPerPay",
    };
}