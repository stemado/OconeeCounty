using Core.Comparison.Comparisons;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OconeeCounty.Models;

namespace OconeeCounty;

public static class Config
{
    public static IServiceCollection AddOconeeCountyModule(this IServiceCollection services, IConfiguration config) =>
        services
            .AddScoped<FileCompare>()
            .AddScoped<OconeeCounty>();
}