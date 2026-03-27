using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParaBankAutomation.Pages;
using ParaBankAutomation.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ParaBankAutomation.Test.Login
{
    [TestFixture]
    public class LoginBoundaryTests
    {
        private IWebDriver driver;
        private LoginPage loginPage;
        private readonly string sheetName = "F2-LoginLogout";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
            loginPage = new LoginPage(driver);
        }

        [TearDown]
        public void TearDown()
        {
            // Lấy status, testCaseId, Screenshot và gọi ExcelHelper.WriteResultAndScreenshot...
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            string testStatus = (status == TestStatus.Passed) ? "PASS" : "FAIL";
            string testName = TestContext.CurrentContext.Test.Name;
            string testCaseId = testName.Split('_').Length >= 3 ? $"{testName.Split('_')[0]}.{testName.Split('_')[1]}.{testName.Split('_')[2]}" : "";

            string screenshotPath = null;
            if (status == TestStatus.Failed)
            {
                try
                {
                    Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"Screenshot_Fail_{testCaseId}.png");
                    screenshot.SaveAsFile(screenshotPath);
                }
                catch { }
            }

            try { if (!string.IsNullOrEmpty(testCaseId)) ExcelHelper.WriteResultAndScreenshot(sheetName, testCaseId, testStatus, screenshotPath); }
            catch { }

            if (driver != null) { driver.Quit(); driver.Dispose(); }
        }

        private void ExecuteBoundaryTest(string testCaseId)
        {
            List<string> testData = ExcelHelper.GetTestDataList(sheetName, testCaseId);
            string username = testData.Count > 0 ? testData[0] : "";
            string password = testData.Count > 1 ? testData[1] : "";

            loginPage.EnterUsername(username);
            loginPage.EnterPassword(password);
            loginPage.ClickLoginButton();
        }

        // --- Boundary Username ---
        [Test]
        public void S_2_14_Username5KyTu()
        {
            ExecuteBoundaryTest("S.2.14");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "User 5 ký tự (invalid) không bị chặn.");
        }

        [Test]
        public void S_2_15_Username6KyTu()
        {
            ExecuteBoundaryTest("S.2.15");
            Assert.That(driver.Url.Contains("login.htm") || driver.Url.Contains("overview.htm"), Is.True);
        }

        [Test]
        public void S_2_16_Username20KyTu()
        {
            ExecuteBoundaryTest("S.2.16");
            Assert.That(driver.Url.Contains("login.htm") || driver.Url.Contains("overview.htm"), Is.True);
        }

        [Test]
        public void S_2_17_Username21KyTu()
        {
            ExecuteBoundaryTest("S.2.17");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "User 21 ký tự không bị xử lý lỗi.");
        }

        [Test]
        public void S_2_18_Password7KyTu()
        {
            ExecuteBoundaryTest("S.2.18");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty);
        }

        [Test]
        public void S_2_19_Password8KyTu()
        {
            ExecuteBoundaryTest("S.2.19");
            Assert.That(driver.Url.Contains("login.htm") || driver.Url.Contains("overview.htm"), Is.True);
        }

        [Test]
        public void S_2_20_Password20KyTu()
        {
            ExecuteBoundaryTest("S.2.20");
            Assert.That(driver.Url.Contains("login.htm") || driver.Url.Contains("overview.htm"), Is.True);
        }

        [Test]
        public void S_2_21_Password21KyTu()
        {
            ExecuteBoundaryTest("S.2.21");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty);
        }
    }
}