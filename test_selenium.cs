using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
class Program {
    static void Main() {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--log-level=0");
        using (var driver = new ChromeDriver(options)) {
            driver.Navigate().GoToUrl("file:///C:/Users/abaqi/source/repos/MovieTicketBooking/test_chart.html");
            System.Threading.Thread.Sleep(3000);
            var logs = driver.Manage().Logs.GetLog(LogType.Browser);
            foreach (var log in logs) {
                Console.WriteLine(log.Level + " " + log.Message);
            }
        }
    }
}
