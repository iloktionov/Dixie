using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class TimeSpanParser_Tests
	{
		[Test]
		public void Test_CorrectWork()
		{
			Assert.AreEqual(TimeSpan.FromMilliseconds(100), TimeSpanParser.Parse("100ms"));
			Assert.AreEqual(TimeSpan.FromMilliseconds(100), TimeSpanParser.Parse("100MS"));
			Assert.AreEqual(TimeSpan.FromMilliseconds(1.5), TimeSpanParser.Parse("1,5ms"));
			Assert.AreEqual(TimeSpan.FromSeconds(100), TimeSpanParser.Parse("100s"));
			Assert.AreEqual(TimeSpan.FromSeconds(0.1), TimeSpanParser.Parse("0.1s"));
			Assert.AreEqual(TimeSpan.FromMinutes(100), TimeSpanParser.Parse("100m"));
			Assert.AreEqual(TimeSpan.FromMinutes(0.1), TimeSpanParser.Parse("0.1m"));
			Assert.AreEqual(TimeSpan.FromHours(100), TimeSpanParser.Parse("100h"));
			Assert.AreEqual(TimeSpan.FromHours(0.1), TimeSpanParser.Parse("0.1h"));
			Assert.AreEqual(TimeSpan.FromHours(500), TimeSpanParser.Parse(TimeSpan.FromHours(500).ToString()));
			Assert.Throws<FormatException>(() => TimeSpanParser.Parse("fdgdfgdfgd"));
			Assert.Throws<FormatException>(() => TimeSpanParser.Parse("123d344ms"));
		}
	}
}