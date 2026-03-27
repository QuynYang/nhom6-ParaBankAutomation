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
    public class RegisterBoundaryTests
    {
        private IWebDriver driver;
        private RegisterPage registerPage;
        private string currentScenarioId;
        private string sheetName = "F1-User Registration";

        // Hàm đọc data
        private void FillDataFromExcel(string scenarioId)
        {
            List<string> data = ExcelHelper.GetTestDataList(sheetName, scenarioId);
            while (data.Count < 11) data.Add("");

            string fName = data[0];
            string lName = data[1];
            string address = data[2];
            string city = data[3];
            string state = data[4];
            string zip = data[5];
            string phone = data[6];
            string ssn = data[7];

            string username = data[8];
            if (!string.IsNullOrEmpty(username))
            {
                username = username + DateTime.Now.ToString("ddHHmmss");
            }

            string password = data[9];
            string confirm = data[10];

            registerPage.FillRegistrationForm(fName, lName, address, city, state, zip, phone, ssn, username, password, confirm);
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
        public void S1_15_RegisterWithPassword7Chars_MinInvalid()
        {
            currentScenarioId = "S.1.15";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetPasswordErrorMessage();
            Assert.That(errorMsg, Does.Not.Contain("Lỗi: Không hiển thị thông báo lỗi"), "Web bị Bug: Chấp nhận tạo tài khoản với mật khẩu 7 ký tự mà không hề cảnh báo độ dài!");
        }

        [Test]
        public void S1_16_RegisterWithPassword8Chars_MinBoundary()
        {
            currentScenarioId = "S.1.16";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string actualMessage = registerPage.GetSuccessMessage();
            Assert.That(actualMessage, Does.Contain("Your account was created successfully"), "Lỗi: Không thể đăng ký với mật khẩu 8 ký tự (Biên dưới).");
        }

        [Test]
        public void S1_17_RegisterWithPassword20Chars_MaxBoundary()
        {
            currentScenarioId = "S.1.17";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string actualMessage = registerPage.GetSuccessMessage();
            Assert.That(actualMessage, Does.Contain("Your account was created successfully"), "Lỗi: Không thể đăng ký với mật khẩu 20 ký tự (Biên trên).");
        }

        [Test]
        public void S1_18_RegisterWithPassword21Chars_MaxInvalid()
        {
            currentScenarioId = "S.1.18";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetPasswordErrorMessage();
            Assert.That(errorMsg, Does.Not.Contain("Lỗi: Không hiển thị thông báo lỗi"), "Web bị Bug: Không có thuộc tính maxlength chặn 20 ký tự và cũng không báo lỗi khi nhập 21 ký tự!");
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
                    string screenshotDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                    Directory.CreateDirectory(screenshotDirectory);
                    string screenshotPath = Path.Combine(screenshotDirectory, $"{currentScenarioId}_Fail.png");
                    screenshot.SaveAsFile(screenshotPath);
                    ExcelHelper.WriteResultAndScreenshot(sheetName, currentScenarioId, "FAIL", screenshotPath);
                }
                else if (status == TestStatus.Passed)
                {
                    ExcelHelper.WriteResultAndScreenshot(sheetName, currentScenarioId, "PASS");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi ghi Excel: " + ex.Message);
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver.Dispose();
                }
            }
        }
    }
}