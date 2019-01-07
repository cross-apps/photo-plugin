using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace PhotoTaker.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void WelcomeTextIsDisplayed()
        {
            // AppResult[] results = app.WaitForElement(c => c.Marked("Welcome to Xamarin.Forms!"));

            Thread.Sleep(5000);
            app.Screenshot("Welcome screen.");


            try
            {
                app.Tap("OK");
            }
            catch (Exception ex)
            {

            }

            Thread.Sleep(2000);

            app.TapCoordinates(400, 800);

            Thread.Sleep(2000);

            app.Screenshot("aufter one tap");

            // app.Repl();
            // Assert.IsTrue(results.Any());
        }
    }
}
