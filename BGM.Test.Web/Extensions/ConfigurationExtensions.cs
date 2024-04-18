using BGM.Test.Web.Models.Configuration;

namespace BGM.Test.Web.Extensions;

public static class ConfigurationExtensions
{
    public static IConfigurationSection ConfigureOptions(this WebApplicationBuilder webApplicationBuilder)
    {
        IConfigurationSection sftpSection = webApplicationBuilder.Configuration.GetSection("SFTP");
        webApplicationBuilder.Services.Configure<SftpOptions>(sftpSection);

        IConfigurationSection configurationSection = webApplicationBuilder.Configuration.GetSection("ImportWorker");
        webApplicationBuilder.Services.Configure<ImportWorkerOptions>(configurationSection);
        return configurationSection;
    }
}