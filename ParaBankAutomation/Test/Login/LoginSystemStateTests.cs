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
    public class LoginSystemStateTests
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
                catch { }
            }
            try { if (!string.IsNullOrEmpty(testCaseId)) ExcelHelper.WriteResultAndScreenshot(sheetName, testCaseId, testStatus, screenshotPath); }
            catch { }
            if (driver != null) { driver.Quit(); driver.Dispose(); }
        }

        private void LoginThanhCongHienTai(string tcId)
        {
            List<string> data = ExcelHelper.GetTestDataList(sheetName, tcId);
            loginPage.EnterUsername(data[0]);
            loginPage.EnterPassword(data[1]);
            loginPage.ClickLoginButton();
        }

        [Test]
        public void S_2_22_ChuyenHuongSauLoginThanhCong()
        {
            LoginThanhCongHienTai("S.2.22");
            Assert.That(driver.Url.Contains("overview.htm"), Is.True, "URL không chuyển sang Account Overview.");
        }

        [Test]
        public void S_2_23_GiuNguyenTrangKhiLoginThatBai()
        {
            loginPage.EnterUsername("sai_user");
            loginPage.EnterPassword("sai_pass");
            loginPage.ClickLoginButton();
            Assert.That(driver.Url.Contains("login.htm"), Is.True, "Hệ thống văng sang trang khác khi lỗi login.");
        }

        [Test]
        public void S_2_24_QuayLaiTrangLoginSauKhiLogout()
        {
            LoginThanhCongHienTai("S.2.24");
            loginPage.ClickLogout(); 
            Assert.That(driver.Url.Contains("index.htm") || loginPage.IsCustomerLoginTitleDisplayed(), Is.True, "Không quay lại trang chủ / trang login sau khi Logout.");
        }

        [Test]
        public void S_2_25_ChanTruyCapProtectedUrlSauLogout()
        {
            LoginThanhCongHienTai("S.2.25");
            string protectedUrl = driver.Url; // Lưu URL của Account Overview
            loginPage.ClickLogout();

            driver.Navigate().GoToUrl(protectedUrl);

            Assert.That(!driver.PageSource.Contains("Accounts Overview"), Is.True, "Lỗi bảo mật: Vẫn xem được Account Overview sau khi Logout.");
        }

        [Test]
        public void S_2_26_DangNhap1TaiKhoanTren2Tab()
        {
            LoginThanhCongHienTai("S.2.26");
            string overviewUrl = driver.Url;

            driver.SwitchTo().NewWindow(WindowType.Tab);
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            Assert.That(!loginPage.IsCustomerLoginTitleDisplayed(), Is.True, "Tab 2 không nhận diện được Session (Cookie) từ Tab 1.");
        }
    }
}