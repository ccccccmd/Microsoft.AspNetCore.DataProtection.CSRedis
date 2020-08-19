using CSRedis;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.DataProtection.CSRedis.Test
{
    public class CSRedisDataProtectionBuilderExtensionsTest
    {
        [Fact]
        public void PersistKeysToRedis_UsesRedisXmlRepository()
        {
            // Arrange
        
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddDataProtection();

            // Act
            builder.PersistKeysToCSRedis(new CSRedisClient(""));
            var services = serviceCollection.BuildServiceProvider();

            // Assert
            var options = services.GetRequiredService<IOptions<KeyManagementOptions>>();
            Assert.IsType<CSRedisXmlRepository>(options.Value.XmlRepository);
        }
    }
}