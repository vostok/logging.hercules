using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Hercules.Helpers;

namespace Vostok.Logging.Hercules.Tests.Helpers
{
    [TestFixture]
    internal class ExceptionsNormalizer_Tests
    {
        [TestCase(
            "<>org.apache.zookeeper.<6ba5cd9e-ba8a-4a9b-a221-d7912a7cdbe2>ClientCnxnSocketNIO",
            "<>org.apache.zookeeper.<~>ClientCnxnSocketNIO")]
        [TestCase(
            "<>org.apache.zookeeper.<6BA5CD9E-BA8A-4A9B-A221-D7912A7CDBE2>ClientCnxnSocketNIO",
            "<>org.apache.zookeeper.<~>ClientCnxnSocketNIO")]
        [TestCase(
            "EmittedProxyCommand_EmittedProxy_IIntegrationServiceClient_747b9339-fe3a-4253-9d33-940978cf037f_SaveOrganizationCommand_2b042abf-7e97-4b9d-8b6b-04ed4598c32e",
            "EmittedProxyCommand_EmittedProxy_IIntegrationServiceClient_~_SaveOrganizationCommand_~")]
        [TestCase("lambda_method285", "lambda_method~")]
        public void Normalize_should_cut_guids(string input, string expected)
        {
            var actual = ExceptionsNormalizer.Normalize(input);
            actual.Should().Be(expected);
        }
    }
}