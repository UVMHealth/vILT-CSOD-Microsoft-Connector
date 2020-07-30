using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace vILTConnector.API
    {
    /// <summary></summary>
    public class Program
    {
        /// <summary></summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //Replacing the default host builder in favor of one that can leverage the key vault.
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        /// <summary>
        /// Modified CreateHostBuilder to allow for KeyVault connection
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
                 Host.CreateDefaultBuilder(args)
                     .ConfigureAppConfiguration((context, config) =>
                     {
                         var builtConfig = config.Build();
                         var keyVaultEndpoint = builtConfig["KeyVaultEndpoint"];
                         if (!string.IsNullOrEmpty(keyVaultEndpoint))
                         {
                             var azureServiceTokenProvider = new AzureServiceTokenProvider();
                             var keyVaultClient = new KeyVaultClient(
                                 new KeyVaultClient.AuthenticationCallback(
                                     azureServiceTokenProvider.KeyVaultTokenCallback));
                             config.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                         }
                     })
                     .ConfigureWebHostDefaults(webBuilder =>
                     {
                         webBuilder.UseStartup<Startup>();
                     });
    }
}
