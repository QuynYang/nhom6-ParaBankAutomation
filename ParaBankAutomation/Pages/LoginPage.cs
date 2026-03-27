using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Support.UI;

namespace ParaBankAutomation.Pages
{
    public class LoginPage
    {
        private IWebDriver _driver;

        // --- Locators ---
        private By _customerLoginTitle = By.XPath("//h2[text()='Customer Login']");
        private By _usernameInput = By.Name("username");
        private By _passwordInput = By.Name("password");
        private By _loginButton = By.XPath("//input[@value='Log In']");
        private By _registerLink = By.LinkText("Register");
        private By _errorMessage = By.CssSelector("p.error");
        private By _logoutLink = By.LinkText("Log Out");
        // --- Constructor ---
        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // --- Hành động (Actions) & Kiểm tra (Verifications) ---

        public bool IsCustomerLoginTitleDisplayed()
        {
            return _driver.FindElement(_customerLoginTitle).Displayed;
        }

        public bool IsUsernameInputDisplayed()
        {
            return _driver.FindElement(_usernameInput).Displayed;
        }

        public bool IsPasswordInputDisplayed()
        {
            return _driver.FindElement(_passwordInput).Displayed;
        }

        public string GetPasswordInputType()
        {
            return _driver.FindElement(_passwordInput).GetAttribute("type");
        }

        public bool IsLoginButtonDisplayed()
        {
            return _driver.FindElement(_loginButton).Displayed;
        }

        public bool IsLoginButtonEnabled()
        {
            return _driver.FindElement(_loginButton).Enabled;
        }

        public void ClickLoginButton()
        {
            _driver.FindElement(_loginButton).Click();
        }

        public void ClickRegisterLink()
        {
            _driver.FindElement(_registerLink).Click();
        }

        public void EnterUsername(string username)
        {
            var element = _driver.FindElement(_usernameInput);
            element.Clear();
            element.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            var element = _driver.FindElement(_passwordInput);
            element.Clear();
            element.SendKeys(password);
        }

        public string GetUsernameValue()
        {
            return _driver.FindElement(_usernameInput).GetAttribute("value");
        }

        public string GetPasswordValue()
        {
            return _driver.FindElement(_passwordInput).GetAttribute("value");
        }

        public string GetErrorMessage()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

                wait.Until(d => d.Url.Contains("login.htm"));

                var errorElements = _driver.FindElements(By.CssSelector("p.error"));
                if (errorElements.Count > 0 && !string.IsNullOrEmpty(errorElements[0].Text))
                {
                    return errorElements[0].Text;
                }

                var fallbackElements = _driver.FindElements(By.XPath("//h1[text()='Error!']/following-sibling::p"));
                if (fallbackElements.Count > 0 && !string.IsNullOrEmpty(fallbackElements[0].Text))
                {
                    return fallbackElements[0].Text;
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void ClickLogout()
        {
            _driver.FindElement(_logoutLink).Click();
        }
    }
}