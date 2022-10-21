namespace OconeeCounty.Models;

public class AuditDifference
{
    public string PropertyName { get; set; }
    public string Object1Value { get; set; }
    public string Object2Value { get; set; }
    public IhrFullFile CurrentObjectOne { get; set; }
    public IhrFullFile PreviousObjectTwo { get; set; }
}