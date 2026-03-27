using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
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

        // Định nghĩa các Locators
        private By firstNameInput = By.Id("customer.firstName");
        private By lastNameInput = By.Id("customer.lastName");
        private By addressInput = By.Id("customer.address.street");
        private By registerButton = By.XPath("//input[@value='Register']");
        private By cityInput = By.Id("customer.address.city");
        private By stateInput = By.Id("customer.address.state");
        private By zipInput = By.Id("customer.address.zipCode");
        private By phoneInput = By.Id("customer.phoneNumber");
        private By ssnInput = By.Id("customer.ssn");
        private By usernameInput = By.Id("customer.username");
        private By passwordInput = By.Id("customer.password");
        private By confirmInput = By.Id("repeatedPassword");

        // Locators cho lỗi và thông báo thành công
        private By usernameError = By.Id("customer.username.errors");
        private By passwordError = By.Id("customer.password.errors");
        private By ssnError = By.Id("customer.ssn.errors");
        private By successMessage = By.XPath("//div[@id='rightPanel']/p");

        // Locator cho lỗi bỏ trống trường First Name
        private By firstNameError = By.Id("customer.firstName.errors");

        // Locator cho Label
        private By firstNameLabel = By.XPath("//b[text()='First Name:']");

        // Các hàm tương tác
        public void GoToRegisterPage()
        {
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/register.htm");
        }

        public string GetPageTitle()
        {
            return driver.Title;
        }

        public bool IsFirstNameTextBoxDisplayed()
        {
            return driver.FindElement(firstNameInput).Displayed;
        }

        public bool IsRegisterButtonDisplayedAndEnabled()
        {
            var btn = driver.FindElement(registerButton);
            return btn.Displayed && btn.Enabled; // Clickable
        }

        public string GetFirstNameLabelText()
        {
            return driver.FindElement(firstNameLabel).Text;
        }

        public void EnterFirstName(string text)
        {
            driver.FindElement(firstNameInput).SendKeys(text);
        }

        public string GetFirstNameInputValue()
        {
            return driver.FindElement(firstNameInput).GetAttribute("value");
        }

        public void ClickRegisterButton()
        {
            driver.FindElement(registerButton).Click();
        }

        public bool IsFirstNameErrorMessageDisplayed()
        {
            return driver.FindElement(firstNameError).Displayed;
        }
        public void FillRegistrationForm(string fName, string lName, string address, string city, string state, string zip, string phone, string ssn, string user, string pass, string confirm)
        {
            EnterFirstName(fName);
            driver.FindElement(lastNameInput).SendKeys(lName);
            driver.FindElement(addressInput).SendKeys(address);
            driver.FindElement(cityInput).SendKeys(city);
            driver.FindElement(stateInput).SendKeys(state);
            driver.FindElement(zipInput).SendKeys(zip);
            driver.FindElement(phoneInput).SendKeys(phone);
            driver.FindElement(ssnInput).SendKeys(ssn);
            driver.FindElement(usernameInput).SendKeys(user);
            driver.FindElement(passwordInput).SendKeys(pass);
            driver.FindElement(confirmInput).SendKeys(confirm);
        }
        
        public string GetPasswordErrorMessage()
        {
            try
            {
                return driver.FindElement(passwordError).Text;
            }
            catch (NoSuchElementException)
            {
                return "Lỗi: Không hiển thị thông báo lỗi (Element not found)";
            }
        }
        public string GetUsernameErrorMessage()
        {
            try { return driver.FindElement(usernameError).Text; }
            catch (NoSuchElementException) { return "Lỗi: Không hiển thị thông báo lỗi"; }
        }

        public string GetSsnErrorMessage()
        {
            try { return driver.FindElement(ssnError).Text; }
            catch (NoSuchElementException) { return "Lỗi: Không hiển thị thông báo lỗi"; }
        }

        // Đăng xuất nếu đang đăng nhập
        public void LogoutIfLoggedIn()
        {
            try
            {
                var logoutLink = driver.FindElement(By.XPath("//a[text()='Log Out']"));
                if (logoutLink.Displayed)
                {
                    logoutLink.Click();
                }
            }
            catch (NoSuchElementException)
            {
                // Nếu không tìm thấy nút Log Out nghĩa là chưa đăng nhập -> Bỏ qua không làm gì cả
            }
        }

        // STăng thời gian chờ lên 15 giây và bắt text thông minh hơn
        public string GetSuccessMessage()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            try
            {
                wait.Until(d =>
                {
                    try
                    {
                        string panelText = d.FindElement(By.Id("rightPanel")).Text;
                        return panelText.Contains("Your account was created successfully") ||
                               panelText.Contains("This username already exists.");
                    }
                    catch { return false; }
                });
                return driver.FindElement(successMessage).Text;
            }
            catch (WebDriverTimeoutException)
            {
                return "LỖI: Quá thời gian chờ (15s), không thể đăng ký thành công!";
            }
        }

        // 1. Hàm để sửa lại Username
        public void EnterUsernameOnly(string username)
        {
            var userField = driver.FindElement(usernameInput);
            userField.Clear();
            userField.SendKeys(username);
        }

        // 2. Hàm Double-Click
        public void DoubleClickRegisterButton()
        {
            Actions actions = new Actions(driver);
            actions.DoubleClick(driver.FindElement(registerButton)).Perform();
        }

        public void SubmitByEnterKey()
        {
            driver.FindElement(confirmInput).SendKeys(Keys.Enter);
        }
    }
}
