using OpenQA.Selenium;
using System.Collections.Generic;

namespace ParaBankAutomation.Pages
{
    public class RegisterPage
    {
        private IWebDriver driver;

        public RegisterPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // 1. Định nghĩa Locators (Các phần tử trên trang)
        private By firstNameInput = By.Id("customer.firstName");
        private By lastNameInput = By.Id("customer.lastName");
        private By addressInput = By.Id("customer.address.street");
        private By cityInput = By.Id("customer.address.city");
        private By stateInput = By.Id("customer.address.state");
        private By zipCodeInput = By.Id("customer.address.zipCode");
        private By phoneInput = By.Id("customer.phoneNumber");
        private By ssnInput = By.Id("customer.ssn");
        private By usernameInput = By.Id("customer.username");
        private By passwordInput = By.Id("customer.password");
        private By confirmPasswordInput = By.Id("repeatedPassword");

        private By registerButton = By.XPath("//input[@value='Register']");
        private By successMessage = By.XPath("//p[contains(text(),'Your account was created successfully')]");
        private By usernameError = By.Id("customer.username.errors");

        // 2. Các Action Methods
        public void NavigateToRegisterPage()
        {
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/register.htm");
        }

        public string GetPageTitle()
        {
            return driver.Title;
        }

        public bool AreAllTextBoxesDisplayed()
        {
            return driver.FindElement(firstNameInput).Displayed &&
                   driver.FindElement(lastNameInput).Displayed &&
                   driver.FindElement(usernameInput).Displayed &&
                   driver.FindElement(passwordInput).Displayed;
            // Có thể thêm đầy đủ 11 textboxes vào đây
        }

        public bool IsRegisterButtonDisplayedAndEnabled()
        {
            var btn = driver.FindElement(registerButton);
            return btn.Displayed && btn.Enabled;
        }

        public void ClickRegister()
        {
            driver.FindElement(registerButton).Click();
        }

        public void FillRegistrationForm(string fName, string lName, string address, string city, string state,
                                         string zip, string phone, string ssn, string user, string pass, string confirmPass)
        {
            driver.FindElement(firstNameInput).SendKeys(fName);
            driver.FindElement(lastNameInput).SendKeys(lName);
            driver.FindElement(addressInput).SendKeys(address);
            driver.FindElement(cityInput).SendKeys(city);
            driver.FindElement(stateInput).SendKeys(state);
            driver.FindElement(zipCodeInput).SendKeys(zip);
            driver.FindElement(phoneInput).SendKeys(phone);
            driver.FindElement(ssnInput).SendKeys(ssn);
            driver.FindElement(usernameInput).SendKeys(user);
            driver.FindElement(passwordInput).SendKeys(pass);
            driver.FindElement(confirmPasswordInput).SendKeys(confirmPass);
        }

        public void EnterTextInFirstName(string text)
        {
            driver.FindElement(firstNameInput).SendKeys(text);
        }

        public string GetFirstNameInputValue()
        {
            return driver.FindElement(firstNameInput).GetAttribute("value");
        }

        public string GetSuccessMessage()
        {
            return driver.FindElement(successMessage).Text;
        }

        public string GetUsernameErrorMessage()
        {
            return driver.FindElement(usernameError).Text;
        }

        public bool IsErrorMessageDisplayedForEmptyFields()
        {
            // Kiểm tra một vài thông báo lỗi mẫu xuất hiện khi bỏ trống form
            var fNameError = driver.FindElement(By.Id("customer.firstName.errors"));
            return fNameError.Displayed && fNameError.Text.Contains("is required");
        }
    }
}