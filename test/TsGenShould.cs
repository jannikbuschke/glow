using System;
using System.Collections.Generic;
using FluentAssertions;
using Glow.Core.Typescript;
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
            var collection = builder.Generate(null);
            Render.ToDisk(collection,  $"./models/{nameof(MarkNonCyclicDependencies)}/");
            foreach (var value in collection.Types.Values)
            {
                value.HasCyclicDependency.Should().BeFalse();
            }
        }

        [Fact]
        public void HandleCyclicDependencies()
        {
            var builder = new TypeCollectionBuilder();
            builder.Add<Parent>();
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection,  $"./models/{nameof(HandleCyclicDependencies)}/");
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
            TypeCollection collection = builder.Generate(null);
            Render.ToDisk(collection, $"./models/{nameof(HandleCyclicDependenciesWithDepthGreater1)}/");
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

            builder.Add<Generic<Foo>>();

            builder.Add<string>();
            builder.Add<Bar>();
            builder.Add<FooBar>();

            builder.Add<KeyValuePair<string, int>>();
            builder.Add<KeyValuePair<string, string>>();
            builder.Add<KeyValuePair<string, Foo>>();

            builder.Add<ReferenceToOtherModule>();

            var result = builder.Generate(null);

            Render.ToDisk(result, $"./models/{nameof(NotCrash)}/");
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
    }

    public class D
    {
        public F F { get; set; }
    }

    public class F
    {

    }
}

namespace Two
{
    public class OtherNamespaceClass
    {
        public string DisplayName { get; set; }
    }
}
