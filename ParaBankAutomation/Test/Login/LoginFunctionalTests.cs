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
    public class LoginFunctionalTests
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
                catch (Exception e) { Console.WriteLine($"[Lỗi chụp màn hình]: {e.Message}"); }
            }

            try
            {
                if (!string.IsNullOrEmpty(testCaseId))
                {
                    ExcelHelper.WriteResultAndScreenshot(sheetName, testCaseId, testStatus, screenshotPath);
                }
            }
            catch (Exception ex) { Console.WriteLine($"[Lỗi ExcelHelper]: {ex.Message}"); }

            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        private void ExecuteLoginFlow(string testCaseId)
        {
            List<string> testData = ExcelHelper.GetTestDataList(sheetName, testCaseId);
            // Xử lý an toàn nếu file excel để trống ô (null/empty)
            string username = testData.Count > 0 ? testData[0] : "";
            string password = testData.Count > 1 ? testData[1] : "";

            if (!string.IsNullOrEmpty(username)) loginPage.EnterUsername(username);
            if (!string.IsNullOrEmpty(password)) loginPage.EnterPassword(password);

            loginPage.ClickLoginButton();
        }

        [Test]
        public void S_2_7_LoginThanhCong()
        {
            ExecuteLoginFlow("S.2.7");
            Assert.That(driver.Url.Contains("overview.htm"), Is.True, "Đăng nhập hợp lệ nhưng không chuyển đến Account Overview.");
        }

        [Test]
        public void S_2_8_LoginUserDungPassSai()
        {
            ExecuteLoginFlow("S.2.8");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "Không hiển thị lỗi khi Pass sai.");
        }

        [Test]
        public void S_2_9_LoginUserSaiPassDung()
        {
            ExecuteLoginFlow("S.2.9");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "Không hiển thị lỗi khi User sai.");
        }

        [Test]
        public void S_2_10_LoginUserVaPassDeuSai()
        {
            ExecuteLoginFlow("S.2.10");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "Không hiển thị lỗi khi User và Pass đều sai.");
        }

        [Test]
        public void S_2_11_LoginKhiUserRong()
        {
            ExecuteLoginFlow("S.2.11");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "Hệ thống không chặn khi User rỗng.");
        }

        [Test]
        public void S_2_12_LoginKhiPassRong()
        {
            ExecuteLoginFlow("S.2.12");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "Hệ thống không chặn khi Pass rỗng.");
        }

        [Test]
        public void S_2_13_LoginKhiUserVaPassRong()
        {
            ExecuteLoginFlow("S.2.13");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "Hệ thống không chặn khi cả 2 trường rỗng.");
        }
    }
}