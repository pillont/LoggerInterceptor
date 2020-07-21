using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pillont.LoggerInterceptors.Factory;
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

        public List<string> Errors { get; }
        public LoggerProxyFactory Factory { get; private set; }
        public List<string> Logged { get; }
        private ISuperObject SuperObj { get; set; }

        public AsyncFunctionsTests()
        {
            Logged = new List<string>();
            Errors = new List<string>();
            Factory = new LoggerProxyFactory();
            Factory.OnLog += (str) => Logged.Add(str);
            Factory.OnError += (str, e) => Errors.Add($"{str} : {e}");

            SuperObj = Factory.CreateForInterface<ISuperObject>(new SuperObject());
        }

        [Fact]
        public async Task ErrorOnAsyncTestAsync()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                                await SuperObj.SuperFunctionWithErrorAsync(42));

            // waiting async log
            await Task.Delay(1000);

            Assert.Single(Logged);
            Assert.Equal("SuperFunctionWithErrorAsync was called with value : [ value : 42 ]", Logged[0]);
            Assert.Single(Errors);
            Assert.StartsWith("SuperFunctionWithErrorAsync an error applied : System.AggregateException: One or more errors occurred. (c est le drame ptain !)\r\n ---> System.InvalidOperationException: c est le drame ptain !\r\n   at pillont.LoggerInterceptors.UnitTest.AsyncFunctionsTests.SuperObject.SuperFunctionWithErrorAsync(Int32 value) in", Errors[0]);
        }

        [Fact]
        public async Task SimpleAsyncTestAsync()
        {
            await SuperObj.SuperFunctionAsync(42);
            // waiting async log
            await Task.Delay(1000);

            Assert.Equal(2, Logged.Count);
            Assert.Equal("SuperFunctionAsync was called with value : [ value : 42 ]", Logged[0]);
            Assert.Equal("SuperFunctionAsync return result : \"super param is : 42\"", Logged[1]);
        }

        [Fact]
        public async Task SimpleAsyncVoidTestAsync()
        {
            await SuperObj.SuperVoidFunctionAsync(42);

            Assert.Single(Logged);
            Assert.Equal("SuperVoidFunctionAsync was called with value : [ value : 42 ]", Logged[0]);
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