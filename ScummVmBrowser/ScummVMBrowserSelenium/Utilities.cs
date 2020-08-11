using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowserSelenium
{
    public static class Utilities
    {

        private static void WaitForElementToBeVisible(this IWebDriver driver, By by, int seconds)
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, seconds));
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(by));
        }

        public static void ClearLocalStorage(this IWebDriver driver, string key)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript($"window.localStorage.removeItem('{key}');");
        }

        public static void AddToLocalStorage(this IWebDriver driver, string key, string data)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript($"window.localStorage.setItem('{key}', '{data.Replace("\"", "\\\"").Replace("\n","")}');");
        }

    }
}
