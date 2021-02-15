using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Glow.Core.Linq;
using Glow.Core.Typescript;
using Glow.Core.Utils;
using One;
using Two;
using Xunit;

namespace Glow.Test
{
    public class TsGenShould
    {
        [Fact]
        public void MarkNonCyclicDependencies()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<D>();
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection,  $"./models/{this.GetMethodName(getFullName: false)}/");
            foreach (TsType value in collection.Types.Values)
            {
                value.HasCyclicDependency.Should().BeFalse();
            }
        }

        [Fact]
        public void Foo()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<G>();
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection,  $"./models/{this.GetMethodName(getFullName: false)}/");
            foreach (TsType value in collection.Types.Values)
            {
                value.HasCyclicDependency.Should().BeTrue();
            }
        }

        [Fact]
        public void TopologicalSortShouldDetectCycles()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<G>();
            // builder.Add<K>();
            TypeCollection collection = builder.Generate(null);

            IEnumerable<TsType> t =
                collection.All()
                    .Where(v => v.IsT0)
                    .Select(v => v.AsT0)
                    .Where(v => !v.IsPrimitive)
                    .DistinctBy(v => v.Id)
                    .ToList();

            IList<TsType> result = t.TopologicalSort(
                v => v.Properties?.Where(v => v.TsType.IsT0).Select(v => v.TsType.AsT0));

            t.Should().OnlyContain(v => v.HasCyclicDependency);
        }

        [Fact]
        public void DetectDoubleCycles()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<H>();
            // builder.Add<K>();
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection,  $"./models/{this.GetMethodName(getFullName: false)}/");
            foreach (TsType value in collection.Types.Values)
            {
                value.HasCyclicDependency.Should().BeTrue();
            }
        }

        [Fact]
        public void MarkCyclicDependenciesOtherDependencies()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<G>();
            // builder.Add<K>();
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection,  $"./models/{this.GetMethodName(getFullName: false)}/");
            foreach (TsType value in collection.Types.Values)
            {
                value.HasCyclicDependency.Should().BeTrue();
            }
        }

        [Fact]
        public void HandleCyclicDependencies()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<Parent>();
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection,  $"./models/{this.GetMethodName(getFullName: false)}/");
            foreach (TsType value in collection.Types.Values)
            {
                value.HasCyclicDependency.Should().BeTrue();
            }
        }

        [Fact]
        public void HandleCyclicDependenciesWithDepthGreater1()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<A>();
            builder.Add<K>();
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection, $"./models/{this.GetMethodName(getFullName: false)}/");
            foreach (TsType value in collection.Types.Values)
            {
                value.HasCyclicDependency.Should().BeTrue();
            }
        }

        [Fact]
        public void NotCrash()
        {
            var builder = new TypeCollectionBuilder();

            builder.Add<IEnumerable<string>>();
            builder.Add<Dictionary<string, object>>();

            // builder.Add<Generic<Foo>>();

            builder.Add<string>();
            builder.Add<Bar>();
            builder.Add<FooBar>();

            builder.Add<KeyValuePair<string, int>>();
            builder.Add<KeyValuePair<string, string>>();
            builder.Add<KeyValuePair<string, Foo>>();

            builder.Add<ReferenceToOtherModule>();

            builder.Add<One.Enum>();
            builder.Add<One.Enum2>();

            TypeCollection result = builder.Generate(null);

            Render.ToDisk(result, $"./models/{this.GetMethodName(getFullName: false)}/");
        }

        // var typeWithConcreteArgument = typeof(List<string>);
        // var args1 = typeWithConcreteArgument.GenericTypeArguments; // [string]
        // var genericArguments = typeWithConcreteArgument.GetGenericArguments(); // [string]
        // var typeWithoutConcreteArgument = typeof(List<>);
        // var args2 = typeWithoutConcreteArgument.GenericTypeArguments;// empty
        // var genericArguments2 = typeWithoutConcreteArgument.GetGenericArguments();// [T], IsGeneric = true
    }
}

namespace One
{
    public enum Enum
    {
        One, Two, Three
    }

    public enum Enum2
    {
        Two=2, Three=3
    }

    public class Bar
    {
        public string Foo { get; set; }
        public int Value { get; set; }
        public FooBar FooBar { get; set; }
    }

    public class FooBar
    {
        public decimal Value { get; set; }
        public double Value2 { get; set; }
        // public IEnumerable<double> Value3 { get; set; }
        public Dictionary<string, decimal> MyProperty { get; set; }
        public Dictionary<string, string> MyProperty2 { get; set; }
        public Dictionary<string, Foo> MyProperty3 { get; set; }
    }

    public class Foo {
        public string DisplayName { get; set; }
    }

    public class Generic<T>
    {
        public IEnumerable<T> GenericValues { get; set; }
    }

    public class ReferenceToOtherModule
    {
        public OtherNamespaceClass OtherNamespaceClass { get; set; }
    }

    public class Parent
    {
        public Child Child { get; set; }
    }
    public class Child
    {
        public Parent Parent { get; set; }
    }
    public class A
    {
        public string DisplayName { get; set; }
        public DateTime Date { get; set; }
        public B B { get; set; }
    }
    public class B
    {
        public Guid Id { get; set; }
        public C C { get; set; }
    }
    public class C
    {
        public A A { get; set; }
        public CyclicOtherNamespace CyclicOtherNamespace { get; set; }
    }

    public class D
    {
        public F F { get; set; }
    }

    public class F
    {

    }

    public class G
    {
        public CyclicOtherNamespace CyclicOtherNamespace { get; set; }
    }

    public class H
    {
        public I I { get; set; }
    }

    public class I
    {
        public H H { get; set; }
        public L L { get; set; }
    }

    public class L
    {
        public I I { get; set; }
    }
}

namespace Two
{
    public class OtherNamespaceClass
    {
        public string DisplayName { get; set; }
    }

    public class CyclicOtherNamespace
    {
        public G G { get; set; }
        public K K { get; set; }
    }

    public class K
    {
        public CyclicOtherNamespace CyclicOtherNamespace { get; set; }
    }
}
