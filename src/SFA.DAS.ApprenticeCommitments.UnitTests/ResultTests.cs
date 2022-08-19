using FluentAssertions;
using NUnit.Framework;
using System;

namespace SFA.DAS.ApprenticeCommitments.UnitTests
{
    public class ResultToStringTests
    {
        [Test]
        public void SuccessToString() => new SuccessResult().ToString().Should().Be("Success");

        [Test]
        public void SuccessOfStringToString()
            => new SuccessResult<string>("bob").ToString().Should().Be("bob");

        [Test]
        public void SuccessOfIntToString()
            => new SuccessResult<int>(12).ToString().Should().Be("12");

        [Test]
        public void ErrorToString()
            => new ErrorResult().ToString().Should().Be("Error");

        [Test]
        public void ErrorOfTToString()
            => new ErrorResult<string, string>("bob").ToString().Should().Be("Error (bob)");

        [Test]
        public void ExceptionToString()
            => new ExceptionResult<Exception>(new Exception("exceptional bob")).ToString()
            .Should().Be(new Exception("exceptional bob").ToString());

        [Test]
        public void SuccessStatusToString()
            => new SuccessStatusResult<string>("successful bob").ToString()
            .Should().Be("successful bob").ToString();

        [Test]
        public void SuccessStatusNullToString()
            => new SuccessStatusResult<string>(null).ToString()
            .Should().Be("Success<String>").ToString();

        [Test]
        public void ExceptionStatusNullToString()
            => new ExceptionStatusResult<string>("failed status", new Exception("exceptional alice")).ToString()
            .Should().Be("failed status (System.Exception: exceptional alice)").ToString();
    }
}