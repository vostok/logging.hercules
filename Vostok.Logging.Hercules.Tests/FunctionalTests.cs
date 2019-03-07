using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Topology;
using Vostok.Commons.Testing;
using Vostok.Hercules.Client;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Queries;
using Vostok.Hercules.Client.Management;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.Hercules.Configuration;
using Vostok.Logging.Hercules.Parsing;

namespace Vostok.Logging.Hercules.Tests
{
    internal class FunctionalTests
    {
        private ConsoleLog consoleLog;
        private HerculesSink sink;
        private string streamName;
        private HerculesManagementClient managementClient;

        private string apiKey = "elk_adapter_key";

        [SetUp]
        public void Setup()
        {
            consoleLog = new ConsoleLog();

            sink = new HerculesSink(
                new HerculesSinkSettings(new FixedClusterProvider(
                        new Uri("http://vm-hercules04:6306")),
                    () => apiKey),
                consoleLog);

            streamName = "dotnet_test_" + Guid.NewGuid().ToString().Substring(0, 8);
            streamName = "elk_adapter_test_0";// + Guid.NewGuid().ToString().Substring(0, 8);

            managementClient = new HerculesManagementClient(
                new HerculesManagementClientSettings(new FixedClusterProvider(new Uri("http://vm-hercules05:6507")), () => apiKey),
                consoleLog);

            // managementClient.CreateStream(
            //     new CreateStreamQuery(
            //         new StreamDescription(streamName)
            //         {
            //             Type = StreamType.Base,
            //             Partitions = 1,
            //             TTL = 1.Minutes()
            //         }),
            //     10.Seconds());
        }

        [TearDown]
        public void TearDown()
        {
            // managementClient.DeleteStream(streamName, 10.Seconds());
        }
        
        [Test, Explicit]
        public void Test()
        {
            var log = new HerculesLog(new HerculesLogSettings(sink, streamName))
                .WithProperty("elk-index", () => "hercules-test-dotnet");
            
            log.Error(GetException(), "lol {A} {C} {B}", new{A = 1, B = 2, C = 3});
            log.Error(GetException(), "lol {A} {C} {B}", new{A = 3, B = 4, C = 5});
            
            new Action(() => sink.GetStatistics().SentRecordsCount.Should().Be(2)).ShouldPassIn(5.Seconds());
            
            var streamClient = new HerculesStreamClient(
                new HerculesStreamClientSettings(
                    new FixedClusterProvider(
                        new Uri("http://vm-hercules05:6407")),
                    () => apiKey),
                    new ConsoleLog());

            var readStreamResult = streamClient.Read(new ReadStreamQuery(streamName)
            {
                Limit = 100,
                ClientShard = 0,
                ClientShardCount = 1
            }, 20.Seconds());

            foreach (var @event in readStreamResult.Payload.Events)
            {
                var logEvent = new LogEventData(@event);
                System.Console.WriteLine(logEvent.Message);
                System.Console.WriteLine(logEvent.Exception?.Message);
            }
        }

        private static Exception GetException()
        {
            try
            {
                throw new AggregateException(
                    new InvalidCastException(),
                    new AggregateException(
                        new InvalidCastException(),
                        new InvalidCastException()));
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}