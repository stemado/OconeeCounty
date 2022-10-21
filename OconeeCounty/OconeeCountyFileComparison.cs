using Flows;
using Serilog;

namespace OconeeCounty;

public class OconeeCountyFileComparison
{
    private readonly IOconeeCounty _comparison;
    private readonly FlowsDbContext _context;

    private readonly Guid _flowId = Guid.Parse("3fa85f64-8717-4562-b3fc-2c963f66afa6");

    public OconeeCountyFileComparison()
    {
    }

    public OconeeCountyFileComparison(IOconeeCounty comparison, FlowsDbContext context)
    {
        _comparison = comparison;
        _context = context;
    }

    public async Task Execute(bool encrypt, bool sendNotification = true)
    {
        Log.Information($"{nameof(OconeeCountyFileComparison)} Process Started");
    }
}

public class OconeeCountyCompareConsumer
{
    private readonly IOconeeCounty _comparison;
    private readonly FlowsDbContext _context;

    public OconeeCountyCompareConsumer(IOconeeCounty comparison, FlowsDbContext context)
    {
        _comparison = comparison;
        _context = context;
    }

    // public async Task Consume(ConsumeContext<FlowComplete> context)
    // {
    //     // This prevents the wrong FlowId from being processed.
    //     if (context.Message.FlowId != OconeeConstants.OconeeCountyFlowJobId) return;
    //
    //     var data = context.Message;
    //
    //     if (!File.Exists(@"C:\Temp\OconeeDeductionChangeFullFile.csv")) return;
    //
    //     // Move file from Temp Folder to Oconee_Payroll Folder
    //     try
    //     {
    //         File.Move(@"C:\Temp\OconeeDeductionChangeFullFile.csv",
    //             @"\\anf-fs01\EDI\_Carrier_Output_Files\Oconee_Payroll\OconeeDeductionChangeFullFile.csv", true);
    //     }
    //     catch (Exception e)
    //     {
    //         Log.Error("Unable to Archive Oconee County File: " + e.Message);
    //     }
    //
    //
    //     // Compare the file from PlanSource
    //     var compareResult = await _comparison.CompareFiles();
    //
    //     // Send Email
    //     var email = new EmailMessage
    //     (
    //         from: new[]
    //         {
    //             "dexchange@antfarmservices.com"
    //         },
    //         to: _context.FlowNotifications.Where(x => x.FlowId == data.FlowId || x.IsDefault).Select(x => x.Email)
    //             .ToArray(),
    //         subject: $"Oconee – Weekly Changes {DateTime.Now:MM.dd.yyyy}",
    //         body: $@"Please see the attached comparison file.",
    //         attachmentFilePaths: new[]
    //         {
    //             compareResult.ZipFilePath
    //         }
    //     );
    //
    //     BackgroundJob.Enqueue(() => EWSEmail.SendMail(email, true, true));
    //
    //     await Task.CompletedTask;
    // }
}