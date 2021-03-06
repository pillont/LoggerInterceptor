using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using pillont.LoggerInterceptors.Factory;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;
using Xunit;

namespace pillont.LoggerInterceptors.UnitTest
{
    public class SimpleFunctionsTests
    {
        public interface ISuperObject
        {
            [Log(Action = LogAction.OnCall)]
            string SuperFunction(int value);

            [Log]
            public string SuperFunctionWithError(int value);

            [Log(Action = LogAction.OnCall | LogAction.OnEnd)]
            string SuperFunctionWithResult(int value);

            [Log]
            public void SuperVoidFunction(int value);
        }

        public List<BaseLogContext> ContextShared { get; }
        public Subject<BaseLogContext> Subject { get; }
        public ISuperObject SuperObj { get; }

        public SimpleFunctionsTests()
        {
            ContextShared = new List<BaseLogContext>();
            Subject = new Subject<BaseLogContext>();
            Subject.Subscribe(val => ContextShared.Add(val));

            var factory = new LoggerProxyFactory(Subject);
            SuperObj = factory.CreateForInterface<ISuperObject>(new SuperObject());
        }

        [Fact]
        public void SimpleEnterAndResultTest()
        {
            SuperObj.SuperFunctionWithResult(42);

            Assert.Equal(2, ContextShared.Count);
            var start = ContextShared[0] as StartLogContext;
            Assert.NotNull(start);
            Assert.NotNull(start.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionWithResult), start.Method.Name);
            Assert.Single(start.Arguments);
            Assert.Equal(42, start.Arguments[0]);
            Assert.Equal(SuperObj, start.CalledObject);

            var result = ContextShared[1] as ResultLogContext;
            Assert.NotNull(result);
            Assert.NotNull(result.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionWithResult), result.Method.Name);
            Assert.Equal("super param is : 42", result.Result);
            Assert.Equal(SuperObj, result.CalledObject);
        }

        [Fact]
        public void SimpleEnterTest()
        {
            SuperObj.SuperFunction(42);

            Assert.Single(ContextShared);
            var start = ContextShared[0] as StartLogContext;
            Assert.NotNull(start);
            Assert.NotNull(start.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunction), start.Method.Name);
            Assert.Single(start.Arguments);
            Assert.Equal(42, start.Arguments[0]);
            Assert.Equal(SuperObj, start.CalledObject);
        }

        [Fact]
        public void SimpleErrorTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
                                     SuperObj.SuperFunctionWithError(42));

            Assert.Equal(2, ContextShared.Count);
            var start = ContextShared[0] as StartLogContext;
            Assert.NotNull(start);
            Assert.NotNull(start.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionWithError), start.Method.Name);
            Assert.Single(start.Arguments);
            Assert.Equal(42, start.Arguments[0]);
            Assert.Equal(SuperObj, start.CalledObject);

            Assert.IsType<ErrorLogContext>(ContextShared[1]);
            var error = ContextShared[1] as ErrorLogContext;
            Assert.NotNull(error.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionWithError), error.Method.Name);
            Assert.IsType<InvalidOperationException>(error.Exception);
            Assert.Equal(SuperObj, error.CalledObject);
        }

        [Fact]
        public void SimpleVoidTest()
        {
            SuperObj.SuperVoidFunction(42);

            Assert.Equal(2, ContextShared.Count);

            var start = ContextShared[0] as StartLogContext;
            Assert.NotNull(start);
            Assert.NotNull(start.Attribute);
            Assert.Equal(nameof(SuperObj.SuperVoidFunction), start.Method.Name);
            Assert.Single(start.Arguments);
            Assert.Equal(42, start.Arguments[0]);
            Assert.Equal(SuperObj, start.CalledObject);

            Assert.IsType<VoidResultLogContext>(ContextShared[1]);
            var result = ContextShared[1] as VoidResultLogContext;
            Assert.NotNull(result.Attribute);
            Assert.Equal(nameof(SuperObj.SuperVoidFunction), result.Method.Name);
            Assert.Equal(SuperObj, result.CalledObject);
        }

        public class SuperObject : ISuperObject
        {
            public string SuperFunction(int value)
            {
                return $"super param is : {value}";
            }

            public string SuperFunctionWithError(int value)
            {
                throw new InvalidOperationException("ben was here and BOOM !");
            }

            public string SuperFunctionWithResult(int value)
            {
                return $"super param is : {value}";
            }

            public void SuperVoidFunction(int value)
            { }
        }
    }
}