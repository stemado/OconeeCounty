using CsvHelper.Configuration.Attributes;
using OfficeOpenXml.Attributes;

namespace OconeeCounty.Models;

public class IhrFullFile
{
    [EpplusIgnore]
    public string UniqueId => EmployeeSSN + EmployeeID + BenefitName + BenefitPlanName;

    public string EmployeeSSN { get; set; }

    public string EmployeeID { get; set; }

    public string EmployeeStatus { get; set; }

    public string FullNameLastFirstMiddle { get; set; }

    public string HireDate { get; set; }

    public string BenefitName { get; set; }

    public string BenefitPlanName { get; set; }

    public string DeductionCode { get; set; }

    public string EffectiveDate { get; set; }

    public string ExpirationDate { get; set; }

    public string ExportReason { get; set; }

    public double? PostTaxDeductionPerPay { get; set; }
    public double? PreTaxDeductionPerPay { get; set; }
    public double? EmployeeCostPerPay { get; set; }

    public string SORTTHISCOLUMN { get; set; }

    public string ElectionConfirmedDate { get; set; }
}