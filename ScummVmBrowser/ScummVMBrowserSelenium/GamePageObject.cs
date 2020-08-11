using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowserSelenium
{
    public static class GamePageObject
    {
        public static void SendKeyToCanvas(this IWebDriver driver, string keys)
        {
            driver.FindCanvasDiv().SendKeys(keys);
        }

        private static IWebElement FindCanvasDiv(this IWebDriver driver)
        {
           return driver.FindElement(By.Id("gameFrame"));
        }
    }
}
