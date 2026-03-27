using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParaBankAutomation.Helpers;
using ParaBankAutomation.Pages;
using ParaBankAutomation.Helpers;
using System.IO;

namespace ParaBankAutomation.Test.Register
{
    [TestFixture]
    public class RegisterTests
    {
        private IWebDriver driver;
        private RegisterPage registerPage;

        private string currentScenarioId;
        private string sheetName = "F1-User Registration";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            registerPage = new RegisterPage(driver);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            registerPage.GoToRegisterPage();
        }

        [Test]
        public void S1_1_VerifyRegisterPageLayoutAndTitle()
        {
            currentScenarioId = "S.1.1";

            string expectedTitle = "ParaBank | Register for Free Online Account Access";
            string actualTitle = registerPage.GetPageTitle();

            Assert.That(actualTitle, Is.EqualTo(expectedTitle), "Tiêu đề trang không khớp!");
        }

        [Test]
        public void S1_2_VerifyAllTextBoxesAreDisplayed()
        {
            currentScenarioId = "S.1.2";

            Assert.That(registerPage.IsFirstNameTextBoxDisplayed(), Is.True, "First Name textbox không hiển thị.");
        }

        [Test]
        public void S1_3_VerifyRegisterButtonIsClickable()
        {
            currentScenarioId = "S.1.3";

            Assert.That(registerPage.IsRegisterButtonDisplayedAndEnabled(), Is.True, "Nút Register không hiển thị hoặc không thể click.");
        }

        [Test]
        public void S1_4_VerifyLabelsAreDisplayedCorrectly()
        {
            currentScenarioId = "S.1.4";

            Assert.That(registerPage.GetFirstNameLabelText(), Is.EqualTo("First Name:"), "Label First Name sai.");
        }

        [Test]
        public void S1_5_VerifyTextBoxAllowsInput()
        {
            currentScenarioId = "S.1.5";

            string testData = ExcelHelper.GetTestData(sheetName, currentScenarioId);

            if (string.IsNullOrEmpty(testData))
            {
                testData = "Nguyen";
            }

            registerPage.EnterFirstName(testData);

            string actualValue = registerPage.GetFirstNameInputValue();
            Assert.That(actualValue, Is.EqualTo(testData), "Dữ liệu nhập vào textbox không chính xác.");
        }

        [Test]
        public void S1_6_VerifyErrorMessageWhenSubmittingEmptyForm()
        {
            currentScenarioId = "S.1.6";

            registerPage.ClickRegisterButton();

            Assert.That(registerPage.IsFirstNameErrorMessageDisplayed(), Is.True, "Không hiển thị lỗi cảnh báo cho trường First Name.");
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
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Lỗi trong quá trình TearDown: " + ex.Message);
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