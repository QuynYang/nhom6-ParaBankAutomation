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
    public class LoginTests
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
            string[] nameParts = testName.Split('_');
            string testCaseId = "";

            if (nameParts.Length >= 3)
            {
                testCaseId = $"{nameParts[0]}.{nameParts[1]}.{nameParts[2]}";
            }

            string screenshotPath = null;
            if (status == TestStatus.Failed)
            {
                try
                {
                    Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    string directory = TestContext.CurrentContext.WorkDirectory;
                    screenshotPath = Path.Combine(directory, $"Screenshot_Fail_{testCaseId}.png");
                    screenshot.SaveAsFile(screenshotPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Lỗi] Không thể chụp màn hình: {e.Message}");
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(testCaseId))
                {
                    ExcelHelper.WriteResultAndScreenshot(sheetName, testCaseId, testStatus, screenshotPath);
                    Console.WriteLine($"[Hoàn tất] Đã ghi kết quả cho Test Case {testCaseId}: {testStatus}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi ExcelHelper] Lỗi khi ghi kết quả vào file Excel: {ex.Message}");
            }

            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        [Test]
        public void S_2_1_KiemTraLayoutVaTieuDeTrangLogin()
        {
            bool isTitleDisplayed = loginPage.IsCustomerLoginTitleDisplayed();
            Assert.That(isTitleDisplayed, Is.True, "Tiêu đề 'Customer Login' không hiển thị trên trang.");
        }

        [Test]
        public void S_2_2_KiemTraTextboxUsernameVaPassword()
        {
            Assert.That(loginPage.IsUsernameInputDisplayed(), Is.True, "Textbox Username không hiển thị.");
            Assert.That(loginPage.IsPasswordInputDisplayed(), Is.True, "Textbox Password không hiển thị.");
            Assert.That(loginPage.GetPasswordInputType(), Is.EqualTo("password"), "Textbox Password không ẩn ký tự (type không phải là password).");
        }

        [Test]
        public void S_2_3_KiemTraButtonLogin()
        {
            Assert.That(loginPage.IsLoginButtonDisplayed(), Is.True, "Button Login không hiển thị.");
            Assert.That(loginPage.IsLoginButtonEnabled(), Is.True, "Button Login không cho phép click.");

            loginPage.ClickLoginButton();
            Assert.That(driver.Url.Contains("login.htm"), Is.True, "Hệ thống không xử lý hành động click Login.");
        }

        [Test]
        public void S_2_4_KiemTraLinkRegister()
        {
            loginPage.ClickRegisterLink();
            Assert.That(driver.Url.Contains("register.htm"), Is.True, "Link Register không điều hướng đúng trang đăng ký.");

            var registerTitle = driver.FindElement(By.XPath("//h1[text()='Signing up is easy!']"));
            Assert.That(registerTitle.Displayed, Is.True, "Không tìm thấy tiêu đề của trang đăng ký.");
        }

        [Test]
        public void S_2_5_KiemTraChoPhepNhapDuLieu()
        {
            List<string> testData = ExcelHelper.GetTestDataList(sheetName, "S.2.5");

            Assert.That(testData.Count, Is.GreaterThanOrEqualTo(2), "File Excel không đủ 2 dòng dữ liệu (Username, Password) cho S.2.5");

            string testUser = testData[0]; // Dòng data đầu tiên
            string testPass = testData[1]; // Dòng data thứ hai

            loginPage.EnterUsername(testUser);
            loginPage.EnterPassword(testPass);

            Assert.That(loginPage.GetUsernameValue(), Is.EqualTo(testUser), "Giá trị nhập vào ô Username không khớp.");
            Assert.That(loginPage.GetPasswordValue(), Is.EqualTo(testPass), "Giá trị nhập vào ô Password không khớp.");
        }

        [Test]
        public void S_2_6_KiemTraThongBaoLoiKhiLoginThatBai()
        {
            List<string> testData = ExcelHelper.GetTestDataList(sheetName, "S.2.6");
            Assert.That(testData.Count, Is.GreaterThanOrEqualTo(2), "File Excel không đủ 2 dòng dữ liệu cho S.2.6");

            loginPage.EnterUsername(testData[0]); // Username sai từ Excel
            loginPage.EnterPassword(testData[1]); // Password sai từ Excel
            loginPage.ClickLoginButton();

            string errorMsg = loginPage.GetErrorMessage();

            Assert.That(errorMsg, Is.Not.Empty, "Không hiển thị thông báo lỗi khi đăng nhập thất bại.");
            Assert.That(errorMsg, Is.EqualTo("The username and password could not be verified."), "Thông báo lỗi không đúng như mong đợi.");
        }
    }
}