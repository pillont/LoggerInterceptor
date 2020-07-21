using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pillont.LoggerInterceptors.Factory;
using Xunit;

namespace pillont.LoggerInterceptors.UnitTest
{
    public class SimpleFunctionsTests
    {
        //TODO: [LogHeader()]
        public interface ISuperObject
        {
            [Log(LogAction.OnCall)]
            string SuperFunction(int value);

            [Log]
            public string SuperFunctionWithError(int value);

            [Log(LogAction.OnCall | LogAction.OnEnd)]
            string SuperFunctionWithResult(int value);

            [Log]
            public void SuperVoidFunction(int value);
        }

        public List<string> Errors { get; }
        public LoggerProxyFactory Factory { get; private set; }
        public List<string> Logged { get; }
        public ISuperObject Superobj { get; private set; }

        public SimpleFunctionsTests()
        {
            Logged = new List<string>();
            Errors = new List<string>();
            Factory = new LoggerProxyFactory();
            Factory.OnLog += (str) => Logged.Add(str);
            Factory.OnError += (str, e) => Errors.Add($"{str} : {e}");

            Superobj = Factory.CreateForInterface<ISuperObject>(new SuperObject());
        }

        [Fact]
        public void NoEventTest()
        {
            Factory = new LoggerProxyFactory();
            Superobj = Factory.CreateForInterface<ISuperObject>(new SuperObject());

            Assert.Throws<InvalidOperationException>(() =>
                                         Superobj.SuperFunction(42));
        }

        [Fact]
        public void SimpleEnterAndResultTest()
        {
            Superobj.SuperFunctionWithResult(42);

            Assert.Equal(2, Logged.Count);
            Assert.Equal("SuperFunctionWithResult was called with value : [ value : 42 ]", Logged[0]);
            Assert.Equal("SuperFunctionWithResult return result : \"super param is : 42\"", Logged[1]);
        }

        [Fact]
        public void SimpleEnterTest()
        {
            Superobj.SuperFunction(42);

            Assert.Single(Logged);
            Assert.Equal("SuperFunction was called with value : [ value : 42 ]", Logged[0]);
        }

        [Fact]
        public void SimpleErrorTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
                                     Superobj.SuperFunctionWithError(42));

            Assert.Single(Logged);
            Assert.Equal("SuperFunctionWithError was called with value : [ value : 42 ]", Logged[0]);

            Assert.Single(Errors);
            Assert.StartsWith("SuperFunctionWithError an error applied : System.InvalidOperationException: ben was here and BOOM !\r\n   at pillont.LoggerInterceptors.UnitTest.SimpleFunctionsTests.SuperObject.SuperFunctionWithError(Int32 value) in ", Errors[0]);
        }

        [Fact]
        public void SimpleVoidTest()
        {
            Superobj.SuperVoidFunction(42);

            Assert.Single(Logged);
            Assert.Equal("SuperVoidFunction was called with value : [ value : 42 ]", Logged[0]);
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