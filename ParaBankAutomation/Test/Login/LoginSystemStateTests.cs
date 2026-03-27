using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
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

            if (data.Count < 2)
            {
                Assert.Fail($"LỖI DỮ LIỆU: File Excel thiếu dữ liệu cho Test Case {tcId}. Vui lòng bổ sung Username và Password vào cột Test Data.");
            }

            string user = data[0];
            string pass = data[1];

            loginPage.EnterUsername(user);
            loginPage.EnterPassword(pass);
            loginPage.ClickLoginButton();
        }

        [Test]
        public void S_2_18_ChuyenHuongSauLoginThanhCong()
        {
            LoginThanhCongHienTai("S.2.18");
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.Contains("overview.htm"));
            }
            catch { }
            Assert.That(driver.Url.Contains("overview.htm"), Is.True, "URL không chuyển sang Account Overview.");
        }

        [Test]
        public void S_2_19_GiuNguyenTrangKhiLoginThatBai()
        {
            List<string> data = ExcelHelper.GetTestDataList(sheetName, "S.2.19");

            if (data.Count < 2)
            {
                Assert.Fail("LỖI DỮ LIỆU: File Excel thiếu dữ liệu cho Test Case S.2.19. Yêu cầu 2 dòng: Wrong User, Pass.");
            }

            string wrongUser = data[0];
            string pass = data[1];

            loginPage.EnterUsername(wrongUser);
            loginPage.EnterPassword(pass);
            loginPage.ClickLoginButton();

            Assert.That(driver.Url.Contains("login.htm"), Is.True, "Hệ thống văng sang trang khác khi lỗi login.");
        }

        [Test]
        public void S_2_20_QuayLaiTrangLoginSauKhiLogout()
        {
            LoginThanhCongHienTai("S.2.20");
            loginPage.ClickLogout();
            Assert.That(driver.Url.Contains("index.htm") || loginPage.IsCustomerLoginTitleDisplayed(), Is.True, "Không quay lại trang chủ / trang login sau khi Logout.");
        }

        [Test]
        public void S_2_21_ChanTruyCapProtectedUrlSauLogout()
        {
            LoginThanhCongHienTai("S.2.21");
            try { new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(d => d.Url.Contains("overview.htm")); } catch { }

            string protectedUrl = driver.Url;
            loginPage.ClickLogout();
            driver.Navigate().GoToUrl(protectedUrl);

            Assert.That(!driver.PageSource.Contains("Accounts Overview"), Is.True, "Lỗi bảo mật: Vẫn xem được Account Overview sau khi Logout.");
        }


        [Test]
        public void S_2_22_DangNhapCung1TaiKhoanO2TabCungChrome()
        {
            LoginThanhCongHienTai("S.2.22");

            driver.SwitchTo().NewWindow(WindowType.Tab);
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            bool isLoggedOut = loginPage.IsCustomerLoginTitleDisplayed();

            if (!isLoggedOut)
            {
                Assert.Fail("BUG BẢO MẬT: Không được đăng nhập cùng lúc 1 tài khoản tại cùng 1 thời điểm. Hệ thống đã cho phép vào thẳng mà không yêu cầu đăng nhập lại.");
            }
        }

        [Test]
        public void S_2_23_DangNhap2TaiKhoanKhacNhauCung1LucO2Tab()
        {
            List<string> data = ExcelHelper.GetTestDataList(sheetName, "S.2.23");

            if (data.Count < 4)
            {
                Assert.Fail("LỖI DỮ LIỆU: Test case S.2.23 cần 4 dòng Test Data trong Excel (User A, Pass A, User B, Pass B). Vui lòng kiểm tra lại file Excel.");
            }

            string userA = data[0];
            string passA = data[1];
            string userB = data[2];
            string passB = data[3];

            // Login User A ở Tab 1
            loginPage.EnterUsername(userA);
            loginPage.EnterPassword(passA);
            loginPage.ClickLoginButton();

            driver.SwitchTo().NewWindow(WindowType.Tab);
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
            if (!loginPage.IsCustomerLoginTitleDisplayed()) { loginPage.ClickLogout(); }

            loginPage.EnterUsername(userB);
            loginPage.EnterPassword(passB);
            loginPage.ClickLoginButton();

            // Quay lại Tab 1 và F5
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.Navigate().Refresh();

            bool hasConflictMsg = driver.PageSource.Contains("Xung đột tài khoản") || driver.PageSource.Contains("tự động đăng xuất");
            bool isForcedLogout = loginPage.IsCustomerLoginTitleDisplayed();

            if (!hasConflictMsg && !isForcedLogout)
            {
                Assert.Fail("BUG BẢO MẬT: Cùng 1 ứng dụng không được đăng nhập 2 tài khoản 1 lúc. Hệ thống không xuất thông báo: 'Xung đột tài khoản, tự động đăng xuất và yêu cầu nhập lại thông tin'.");
            }
        }

        [Test]
        public void S_2_24_DangNhapCung1TaiKhoanO2TrinhDuyetKhacNhau()
        {
            // Trình duyệt 1 (Chrome 1) đăng nhập
            LoginThanhCongHienTai("S.2.24");

            IWebDriver driver2 = null;
            try
            {
                // Trình duyệt 2 đăng nhập cùng tài khoản
                driver2 = new ChromeDriver();
                driver2.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                driver2.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

                LoginPage driver2LoginPage = new LoginPage(driver2);

                List<string> data = ExcelHelper.GetTestDataList(sheetName, "S.2.24");
                if (data.Count < 2)
                {
                    Assert.Fail("LỖI DỮ LIỆU: File Excel thiếu dữ liệu cho Test Case S.2.24. Cần 2 dòng: Username, Password.");
                }

                string user = data[0];
                string pass = data[1];

                driver2LoginPage.EnterUsername(user);
                driver2LoginPage.EnterPassword(pass);
                driver2LoginPage.ClickLoginButton();

                // Quay lại Trình duyệt 1 và F5
                driver.Navigate().Refresh();
                bool isBrowser1LoggedOut = loginPage.IsCustomerLoginTitleDisplayed();

                if (!isBrowser1LoggedOut)
                {
                    Assert.Fail("BUG BẢO MẬT: Cùng 1 thiết bị không được đăng nhập cùng lúc 2 thiết bị. Hệ thống không tự động đăng xuất Trình duyệt 1 khi có thiết bị khác đăng nhập vào.");
                }
            }
            finally
            {
                if (driver2 != null) { driver2.Quit(); driver2.Dispose(); }
            }
        }

        [Test]
        public void S_2_25_DangNhap2TaiKhoanKhacNhauO2TrinhDuyet()
        {
            List<string> data = ExcelHelper.GetTestDataList(sheetName, "S.2.25");
            if (data.Count < 4)
            {
                Assert.Fail("LỖI DỮ LIỆU: Test case S.2.25 cần 4 dòng Test Data trong Excel (User A, Pass A, User B, Pass B).");
            }

            string userA = data[0];
            string passA = data[1];
            string userB = data[2];
            string passB = data[3];

            // Trình duyệt 1 đăng nhập User A
            loginPage.EnterUsername(userA);
            loginPage.EnterPassword(passA);
            loginPage.ClickLoginButton();

            IWebDriver driver2 = null;
            try
            {
                // Trình duyệt 2 đăng nhập User B
                driver2 = new ChromeDriver();
                driver2.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                driver2.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

                LoginPage driver2LoginPage = new LoginPage(driver2);
                driver2LoginPage.EnterUsername(userB);
                driver2LoginPage.EnterPassword(passB);
                driver2LoginPage.ClickLoginButton();

                bool hasConflictError = driver2.PageSource.Contains("Xung đột") || driver2LoginPage.GetErrorMessage() != "";

                if (!hasConflictError)
                {
                    Assert.Fail("BUG BẢO MẬT: Cùng 1 ứng dụng không được đăng nhập 2 tài khoản 1 lúc. Hệ thống đang cho phép mở 2 trình duyệt để login 2 tài khoản riêng biệt.");
                }
            }
            finally
            {
                if (driver2 != null) { driver2.Quit(); driver2.Dispose(); }
            }
        }
    }
}