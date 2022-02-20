using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions.Events;

// ReSharper disable PossibleNullReferenceException

namespace Vostok.Logging.Hercules.Tests
{
    internal class HerculesTagsBuilderExtensions_Tests
    {
        private HerculesTagsBuilder builder;

        [SetUp]
        public void Setup()
        {
            builder = new HerculesEventBuilder();
        }

        [Test]
        public void Should_serialize_dates_in_ISO_format()
        {
            var dtoValue = DateTimeOffset.Now;
            var dtValue = DateTime.Now;

            var properties = new Dictionary<string, object>
            {
                ["DateTimeOffset"] = dtoValue,
                ["DateTime"] = dtValue
            };

            builder.AddProperties(properties, null, null);

            var tags = builder.BuildTags();

            tags["DateTimeOffset"]?.AsString.Should().Be(dtoValue.ToString("O"));
            tags["DateTime"]?.AsString.Should().Be(dtValue.ToString("O"));
        }

        [Test]
        public void Should_filter_properties()
        {
            var properties = new Dictionary<string, object>
            {
                ["p1"] = "v1",
                ["p2"] = "v2"
            };

            builder.AddProperties(properties, new[] {"p1"}, null);

            var tags = builder.BuildTags();

            tags.ContainsKey("p1").Should().BeFalse();
            tags["p2"]?.AsString.Should().Be("v2");
        }
        
        [Test]
        public void Should_serialize_complex_objects()
        {
            var properties = new Dictionary<string, object>
            {
                ["p1"] = new Class1(),
                ["p2"] = new Class2()
            };

            builder.AddProperties(properties, null, null);

            var tags = builder.BuildTags();

            tags.Keys.Should().BeEquivalentTo("p1", "p2");

            #region p1 checking

            EnsureEqual(tags["p1"].AsContainer);

            #endregion

            #region p2 checking

            var p2 = tags["p2"].AsContainer;
            
            p2["SimpleValue"].AsInt.Should().Be(42);
            EnsureEqual(p2["ObjectValue"].AsContainer);

            p2["SimpleArray"].AsVector.AsStringList.Should().BeEquivalentTo("who", "is", "it");
            EnsureEqual(p2["ObjectsArray"].AsVector.AsContainerList[0], 1);
            EnsureEqual(p2["ObjectsArray"].AsVector.AsContainerList[1], 2);
            
            p2["SimpleDictionary"].AsContainer["aa"].AsInt.Should().Be(55);
            p2["SimpleDictionary"].AsContainer["bb"].AsInt.Should().Be(66);
            EnsureEqual(p2["ObjectsDictionary"].AsContainer["123"].AsContainer, b:"123key");
            EnsureEqual(p2["ObjectsDictionary"].AsContainer["124"].AsContainer, b:"124key");
            
            #endregion

            void EnsureEqual(HerculesTags value, int a = 11, string b = "12asdf")
            {
                value.Keys.Should().BeEquivalentTo("A", "B");
                value["A"].AsInt.Should().Be(a);
                value["B"].AsString.Should().Be(b);
            } 
        }
        
        public class Class1
        {
            public int A { get; set; } = 11;
            public string B { get; set; } = "12asdf";
        }
    
        public class Class2
        {
            public int SimpleValue { get; set; } = 42;
            public Class1 ObjectValue { get; set; } = new();

            public List<string> SimpleArray { get; set; } = new() {"who", "is", "it"};
            public Class1[] ObjectsArray { get; set; } = new[] {new Class1 {A = 1}, new Class1 {A = 2}};

            public Dictionary<string, int> SimpleDictionary { get; set; } = new()
            {
                ["aa"] = 55,
                ["bb"] = 66
            };
            public Dictionary<int, Class1> ObjectsDictionary { get; set; } = new()
            {
                [123] = new Class1 {B = "123key"},
                [124] = new Class1 {B = "124key"}
            };
        }
    }
}