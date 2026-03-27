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
    public class RegisterFunctionalTests
    {
        private IWebDriver driver;
        private RegisterPage registerPage;

        private string currentScenarioId;
        private string sheetName = "F1-User Registration";

        private void FillDataFromExcel(string scenarioId, bool randomizeUsername = true)
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
            string password = data[9];
            string confirm = data[10];

            // Username
            // Sinh ra 1 mã ngẫu nhiên
            if (randomizeUsername)
            {
                username = "User_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            }

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
        public void S1_7_RegisterSuccessWithValidData()
        {
            currentScenarioId = "S.1.7";
            FillDataFromExcel(currentScenarioId); // Mặc định là true (tự làm mới username)
            registerPage.ClickRegisterButton();

            string actualMessage = registerPage.GetSuccessMessage();
            Assert.That(actualMessage, Does.Contain("Your account was created successfully"), $"Lỗi web: {actualMessage}");
        }

        [Test]
        public void S1_8_RegisterWithNewUsernameSameInfo()
        {
            currentScenarioId = "S.1.8";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string actualMessage = registerPage.GetSuccessMessage();
            Assert.That(actualMessage, Does.Contain("Your account was created successfully"), $"Lỗi web: {actualMessage}");
        }

        [Test]
        public void S1_9_RegisterWithExistingUsername()
        {
            currentScenarioId = "S.1.9";

            List<string> data = ExcelHelper.GetTestDataList(sheetName, currentScenarioId);
            while (data.Count < 11) data.Add("");

            // Tạo 1 username ngẫu nhiên riêng cho case này
            string uniqueUser = "User_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], uniqueUser, data[9], data[10]);
            registerPage.ClickRegisterButton();

            System.Threading.Thread.Sleep(3000);

            registerPage.LogoutIfLoggedIn();

            registerPage.GoToRegisterPage();
            registerPage.FillRegistrationForm(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], uniqueUser, data[9], data[10]);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetUsernameErrorMessage();
            Assert.That(errorMsg, Is.EqualTo("This username already exists."), "Không hiển thị lỗi trùng username!");
        }

        [Test]
        public void S1_10_RegisterWithValidPassword()
        {
            currentScenarioId = "S.1.10";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string actualMessage = registerPage.GetSuccessMessage();
            Assert.That(actualMessage, Does.Contain("Your account was created successfully"), $"Lỗi web: {actualMessage}");
        }

        [Test]
        public void S1_11_RegisterWithInvalidPassword_Whitespace()
        {
            currentScenarioId = "S.1.11";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetPasswordErrorMessage();
            Assert.That(errorMsg, Is.EqualTo("Password is required."), "Web bị lỗi: Không chặn Password chứa khoảng trắng!");
        }

        [Test]
        public void S1_12_RegisterWithEmptyUsername()
        {
            currentScenarioId = "S.1.12";
            FillDataFromExcel(currentScenarioId, false); // Cố tình truyền False để giữ nguyên Username rỗng
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetUsernameErrorMessage();
            Assert.That(errorMsg, Is.EqualTo("Username is required."), "Thiếu cảnh báo trống Username.");
        }

        [Test]
        public void S1_13_RegisterWithEmptyPassword()
        {
            currentScenarioId = "S.1.13";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetPasswordErrorMessage();
            Assert.That(errorMsg, Is.EqualTo("Password is required."), "Thiếu cảnh báo trống Password.");
        }

        [Test]
        public void S1_14_RegisterWithEmptyOtherRequiredFields()
        {
            currentScenarioId = "S.1.14";
            FillDataFromExcel(currentScenarioId);
            registerPage.ClickRegisterButton();

            string errorMsg = registerPage.GetSsnErrorMessage();
            Assert.That(errorMsg, Is.EqualTo("Social Security Number is required."), "Web bị lỗi: Không báo lỗi khi bỏ trống SSN.");
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