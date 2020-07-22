using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using pillont.LoggerInterceptors.Factory;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;
using Xunit;

namespace pillont.LoggerInterceptors.UnitTest
{
    public class AsyncFunctionsTests
    {
        public interface ISuperObject
        {
            [Log]
            public Task<string> SuperFunctionAsync(int value);

            [Log]
            public Task<string> SuperFunctionWithErrorAsync(int value);

            [Log]
            public Task SuperVoidFunctionAsync(int value);
        }

        public List<BaseLogContext> ContextShared { get; }
        public Subject<BaseLogContext> Subject { get; }
        private ISuperObject SuperObj { get; set; }

        public AsyncFunctionsTests()
        {
            ContextShared = new List<BaseLogContext>();
            Subject = new Subject<BaseLogContext>();
            Subject.Subscribe(val => ContextShared.Add(val));

            var factory = new LoggerProxyFactory(Subject);
            SuperObj = factory.CreateForInterface<ISuperObject>(new SuperObject());
        }

        [Fact]
        public async Task ErrorOnAsyncTestAsync()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                                await SuperObj.SuperFunctionWithErrorAsync(42));

            // waiting async log
            await Task.Delay(1000);

            Assert.Equal(2, ContextShared.Count);
            var start = ContextShared[0] as StartLogContext;
            Assert.NotNull(start);
            Assert.NotNull(start.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionWithErrorAsync), start.Method.Name);
            Assert.Single(start.Parameters);
            Assert.Equal("value", start.Parameters[0].Name);
            Assert.Single(start.Values);
            Assert.Equal(42, start.Values[0]);

            var error = ContextShared[1] as ErrorLogContext;
            Assert.NotNull(error);
            Assert.NotNull(error.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionWithErrorAsync), error.Method.Name);
            Assert.IsType<AggregateException>(error.Exception);
            Assert.IsType<InvalidOperationException>((error.Exception as AggregateException).InnerException);
        }

        [Fact]
        public async Task SimpleAsyncTestAsync()
        {
            await SuperObj.SuperFunctionAsync(42);
            // waiting async log
            await Task.Delay(1000);

            Assert.Equal(2, ContextShared.Count);
            var start = ContextShared[0] as StartLogContext;
            Assert.NotNull(start);
            Assert.NotNull(start.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionAsync), start.Method.Name);
            Assert.Single(start.Parameters);
            Assert.Equal("value", start.Parameters[0].Name);
            Assert.Single(start.Values);
            Assert.Equal(42, start.Values[0]);

            var result = ContextShared[1] as ResultLogContext;
            Assert.NotNull(result);
            Assert.NotNull(result.Attribute);
            Assert.Equal(nameof(SuperObj.SuperFunctionAsync), result.Method.Name);
            Assert.Equal("super param is : 42", result.Result);
        }

        [Fact]
        public async Task SimpleAsyncVoidTestAsync()
        {
            await SuperObj.SuperVoidFunctionAsync(42);

            // waiting async log
            await Task.Delay(1000);

            Assert.Equal(2, ContextShared.Count);
            var start = ContextShared[0] as StartLogContext;
            Assert.NotNull(start);
            Assert.NotNull(start.Attribute);
            Assert.Equal(nameof(SuperObj.SuperVoidFunctionAsync), start.Method.Name);
            Assert.Single(start.Parameters);
            Assert.Equal("value", start.Parameters[0].Name);
            Assert.Single(start.Values);
            Assert.Equal(42, start.Values[0]);

            Assert.IsType<VoidResultLogContext>(ContextShared[1]);
            var result = ContextShared[1] as VoidResultLogContext;
            Assert.NotNull(result.Attribute);
            Assert.Equal(nameof(SuperObj.SuperVoidFunctionAsync), result.Method.Name);
        }

        public class SuperObject : ISuperObject
        {
            public async Task<string> SuperFunctionAsync(int value)
            {
                await Task.Delay(500);
                return $"super param is : {value}";
            }

            public async Task<string> SuperFunctionWithErrorAsync(int value)
            {
                await Task.Delay(500);
                throw new InvalidOperationException("c est le drame ptain !");
            }

            public async Task SuperVoidFunctionAsync(int value)
            {
                await Task.Delay(500);
            }
        }
    }
}