using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParaBankAutomation.Helpers;
using ParaBankAutomation.Pages;
using System;
using System.Collections.Generic;
using System.IO;

namespace ParaBankAutomation.Test.Register
{
    [TestFixture]
    public class RegisterSystemStateTests
    {
        private IWebDriver driver;
        private RegisterPage registerPage;
        private string currentScenarioId;
        private string sheetName = "F1-User Registration";

        // Hàm đọc data cơ bản
        private List<string> GetExcelData(string scenarioId)
        {
            List<string> data = ExcelHelper.GetTestDataList(sheetName, scenarioId);
            while (data.Count < 11) data.Add("");
            return data;
        }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            registerPage = new RegisterPage(driver);
            registerPage.GoToRegisterPage();
        }

        [Test]
        public void S1_19_SubmitFail_Then_FixAndSubmitSuccess()
        {
            currentScenarioId = "S.1.19";
            var data = GetExcelData(currentScenarioId);
            string validUsername = "User_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], "", data[9], data[10]);
            registerPage.ClickRegisterButton();
            Assert.That(registerPage.GetUsernameErrorMessage(), Is.EqualTo("Username is required."), "Lần 1: Không báo lỗi Username trống!");

            registerPage.EnterUsernameOnly(validUsername);
            registerPage.ClickRegisterButton();

            Assert.That(registerPage.GetSuccessMessage(), Does.Contain("Your account was created successfully"), "Lần 2: Bổ sung dữ liệu nhưng vẫn không đăng ký được!");
        }

        [Test]
        public void S1_20_Security_SqlInjection()
        {
            currentScenarioId = "S.1.20";
            var data = GetExcelData(currentScenarioId);
            string sqlInjectionString = data[8]; 

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], sqlInjectionString, data[9], data[10]);
            registerPage.ClickRegisterButton();

            string actualMsg = registerPage.GetSuccessMessage();
            Assert.That(actualMsg, Does.Not.Contain("Your account was created successfully"), "BUG BẢO MẬT: Hệ thống cho phép đăng ký Username chứa mã SQL Injection!");
        }

        [Test]
        public void S1_21_DataIntegrity_WhitespaceHandling()
        {
            currentScenarioId = "S.1.21";
            var data = GetExcelData(currentScenarioId);
            string usernameWithSpaces = data[8];
            string passWithSpaces = data[9];     

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], usernameWithSpaces, passWithSpaces, passWithSpaces);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetUsernameErrorMessage();
            Assert.That(errorMsg, Does.Not.Contain("Lỗi: Không hiển thị thông báo lỗi"), "BUG TOÀN VẸN DỮ LIỆU: Hệ thống cho phép Username và Password chứa khoảng trắng ở giữa/hai đầu!");
        }

        [Test]
        public void S1_22_SystemState_CaseSensitivity()
        {
            currentScenarioId = "S.1.22";
            var data = GetExcelData(currentScenarioId);
            string baseId = Guid.NewGuid().ToString("N").Substring(0, 5);
            string upperUser = "tEsT_" + baseId;
            string lowerUser = "test_" + baseId; 

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], upperUser, data[9], data[10]);
            registerPage.ClickRegisterButton();
            System.Threading.Thread.Sleep(2000);
            registerPage.LogoutIfLoggedIn();

            registerPage.GoToRegisterPage();
            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], lowerUser, data[9], data[10]);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetUsernameErrorMessage();
            Assert.That(errorMsg, Is.EqualTo("This username already exists."), "BUG DATABASE: Hệ thống phân biệt hoa/thường cho Username, cho phép đăng ký 'tEsT' và 'test' như 2 user khác nhau!");
        }

        [Test]
        public void S1_23_SystemState_PreventDoubleClick()
        {
            currentScenarioId = "S.1.23";
            var data = GetExcelData(currentScenarioId);
            string uniqueUser = "User_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], uniqueUser, data[9], data[10]);

            registerPage.DoubleClickRegisterButton();

            string actualMsg = registerPage.GetSuccessMessage();
            Assert.That(actualMsg, Does.Contain("Your account was created successfully"), "BUG HỆ THỐNG: Web bị lỗi (crash hoặc duplicate) khi người dùng lỡ tay click đúp chuột vào nút Register!");
        }

        [Test]
        public void S1_24_Navigation_BrowserBackButton()
        {
            currentScenarioId = "S.1.24";
            var data = GetExcelData(currentScenarioId);
            string uniqueUser = "User_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], uniqueUser, data[9], data[10]);
            registerPage.ClickRegisterButton();
            System.Threading.Thread.Sleep(2000);

            driver.Navigate().Back();

            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetUsernameErrorMessage();
            Assert.That(errorMsg, Is.EqualTo("This username already exists."), "BUG ĐIỀU HƯỚNG: Trình duyệt không chặn Submit lại form hoặc hệ thống xử lý sai khi ấn Back!");
        }


        [Test]
        public void S1_25_Usability_SubmitByEnterKey()
        {
            currentScenarioId = "S.1.25";
            var data = GetExcelData(currentScenarioId);
            string uniqueUser = "User_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], uniqueUser, data[9], data[10]);

            registerPage.SubmitByEnterKey();

            string actualMsg = registerPage.GetSuccessMessage();
            Assert.That(actualMsg, Does.Contain("Your account was created successfully"), "BUG KHẢ DỤNG (UX): Không thể Submit form bằng cách nhấn phím Enter!");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                if (status == TestStatus.Failed)
                {
                    Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    string screenshotDir = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                    Directory.CreateDirectory(screenshotDir);
                    string path = Path.Combine(screenshotDir, $"{currentScenarioId}_Fail.png");
                    screenshot.SaveAsFile(path);
                    ExcelHelper.WriteResultAndScreenshot(sheetName, currentScenarioId, "FAIL", path);
                }
                else if (status == TestStatus.Passed)
                {
                    ExcelHelper.WriteResultAndScreenshot(sheetName, currentScenarioId, "PASS");
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi ghi Excel: " + ex.Message); }
            finally
            {
                if (driver != null) { driver.Quit(); driver.Dispose(); }
            }
        }
    }
}