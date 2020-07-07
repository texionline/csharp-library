using System;
using NUnit.Framework;

namespace Texi
{
    [TestFixture]
    public class TexiTests
    {
        [Test]
        public void TestMethod()
        {
            string apiKey = "YOUR_APIKEY";
            var t = new Texi("http://texionline.scoollabs.com", apiKey);
            t.ResponseReceived += delegate(object sender, ResponseEventArgs e) { 
                Console.WriteLine(e.Response.Data);
            };
            string phone = "PHONE_NUMBER";
            t.Send(phone, "Hello from C# library");
        }
    }
}
