using System;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using CSRedis;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.DataProtection.CSRedis.Test
{

    public class DataProtectionRedisTests
    {

        public DataProtectionRedisTests(ITestOutputHelper output)
        {
            _output = output;

            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("testconfig.json")
                .Build();


            _redisClient = new CSRedisClient(_config["Test:Redis:Server"]);
            _redisClient.Del("Key");
        }

        private readonly ITestOutputHelper _output;
        private readonly IConfigurationRoot _config;

        private readonly CSRedisClient _redisClient;

        [Fact]
        public void GetAllElements_ReturnsAllXmlValuesForGivenKey()
        {
            _redisClient.RPush("Key", "<Element1/>", "<Element2/>");


            var repo = new CSRedisXmlRepository(_redisClient, "Key");

            var elements = repo.GetAllElements().ToArray();

            Assert.Equal(new XElement("Element1").ToString(), elements[0].ToString());
            Assert.Equal(new XElement("Element2").ToString(), elements[1].ToString());
        }

        [Fact]
        public void GetAllElements_ThrowsParsingException()
        {
            _redisClient.RPush("Key", "<Element1/>", "<Element2>");


            var repo = new CSRedisXmlRepository(_redisClient, "Key");

            Assert.Throws<XmlException>(() => repo.GetAllElements());
        }

        [Fact]
        public void StoreElement_PushesValueToList()
        {
            _redisClient.RPush("Key", "<Element2 />");


            var repo = new CSRedisXmlRepository(_redisClient, "Key");

            repo.StoreElement(new XElement("Element2"), null);
        }

        [Fact]
        public void XmlRoundTripsToActualRedisServer()
        {
            var guid = Guid.NewGuid().ToString();
            var key = "Test:DP:Key" + guid;

            try
            {
                var repo = new CSRedisXmlRepository(_redisClient, key);
                var element = new XElement("HelloRedis", guid);
                repo.StoreElement(element, guid);

                Thread.Sleep(1000);

                var repo2 = new CSRedisXmlRepository(_redisClient, key);
                var elements = repo2.GetAllElements();

                Assert.Contains(elements, e => e.Name == "HelloRedis" && e.Value == guid);
            }
            finally
            {
                // cleanup
                _redisClient.Del(key);
            }
        }
    }


}