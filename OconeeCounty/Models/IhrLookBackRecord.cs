namespace OconeeCounty.Models;

public record IhrLookBackRecord
{
    public string? EmployeeSSN { get; set; }

    public string? EmployeeID { get; set; }

    public string? EmployeeStatus { get; set; }

    public string? FullNameLastFirstMiddle { get; set; }

    public string? HireDate { get; set; }

    public string? BenefitName { get; set; }

    public string? BenefitPlanName { get; set; }


    public string? EffectiveDate { get; set; }

    public string? ExpirationDate { get; set; }

    public string? ElectionChangeDate { get; set; }

    public string? ExportReason { get; set; }

    public double? PostTaxDeductionPerPay { get; set; }
    public double? PreTaxDeductionPerPay { get; set; }
    public double? EmployeeCostPerPay { get; set; }
}