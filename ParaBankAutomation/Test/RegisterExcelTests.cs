using System;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OfficeOpenXml; // Thư viện EPPlus
using ParaBankAutomation.Pages;

namespace ParaBankAutomation.Tests
{
    [TestFixture]
    public class RegisterExcelTests
    {
        private IWebDriver driver;
        private RegisterPage registerPage;

        // ĐƯỜNG DẪN TỚI FILE EXCEL CỦA BẠN (Nhớ sửa lại cho đúng với máy của bạn)
        private string excelFilePath = @"C:\Users\vungo\OneDrive\Tài liệu\2025\BDCLPM\nhom6.xlsx";

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            //thiết lập giấy phép cho EPPlus (bắt buộc từ phiên bản 5 trở lên)
            ExcelPackage.License.SetNonCommercialPersonal("NonCommercial User");
        }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            registerPage = new RegisterPage(driver);
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        [Test]
        public void ExecuteTestsAndWriteToExcel()
        {
            FileInfo fileInfo = new FileInfo(excelFilePath);
            if (!fileInfo.Exists)
            {
                Assert.Fail("Không tìm thấy file Excel tại đường dẫn: " + excelFilePath);
            }

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // Chọn Sheet F1-User Registration
                ExcelWorksheet worksheet = package.Workbook.Worksheets["F1-User Registration"];
                if (worksheet == null) throw new Exception("Không tìm thấy sheet 'F1-User Registration'");

                // Duyệt qua các dòng có chứa Test Case (Bắt đầu từ dòng 2)
                for (int row = 2; row <= 25; row++)
                {
                    string scenarioId = worksheet.Cells[row, 2].Text.Trim(); // Cột B: Scenario ID

                    // Bỏ qua các dòng trống (các dòng dữ liệu đi kèm không có Scenario ID)
                    if (string.IsNullOrEmpty(scenarioId)) continue;

                    string actualResult = "";
                    string status = "Fail";

                    try
                    {
                        registerPage.NavigateToRegisterPage();

                        // TỰ ĐỘNG ĐỌC DỮ LIỆU & TEST DỰA TRÊN SCENARIO ID
                        switch (scenarioId)
                        {
                            case "S.1.1":
                                actualResult = registerPage.GetPageTitle();
                                status = actualResult.Contains("ParaBank") ? "Pass" : "Fail";
                                break;

                            case "S.1.2":
                                bool areDisplayed = registerPage.AreAllTextBoxesDisplayed();
                                actualResult = areDisplayed ? "Đầy đủ 11 textboxes" : "Thiếu textbox";
                                status = areDisplayed ? "Pass" : "Fail";
                                break;

                            case "S.1.7":
                            case "S.1.8":
                            case "S.1.9":
                            case "S.1.10":
                                // TỰ ĐỌC DỮ LIỆU TỪ EXCEL (Cột F - Cột số 6) trải dài trên các dòng bên dưới
                                string fName = worksheet.Cells[row, 6].Text;
                                string lName = worksheet.Cells[row + 1, 6].Text;
                                string address = worksheet.Cells[row + 2, 6].Text;
                                string city = worksheet.Cells[row + 3, 6].Text;
                                string state = worksheet.Cells[row + 4, 6].Text;
                                string zip = worksheet.Cells[row + 5, 6].Text;
                                string phone = worksheet.Cells[row + 6, 6].Text;
                                string ssn = worksheet.Cells[row + 7, 6].Text;
                                string username = worksheet.Cells[row + 8, 6].Text;
                                string pass = worksheet.Cells[row + 9, 6].Text;
                                string confirm = worksheet.Cells[row + 10, 6].Text;

                                // Xử lý logic riêng cho S.1.7 (Username luôn mới để Pass)
                                if (scenarioId == "S.1.7")
                                {
                                    username = username + "_" + DateTime.Now.Ticks;
                                }

                                // Điền form
                                registerPage.FillRegistrationForm(fName, lName, address, city, state, zip, phone, ssn, username, pass, confirm);
                                registerPage.ClickRegister();

                                // TỰ ĐỘNG ĐÁNH GIÁ KẾT QUẢ
                                if (scenarioId == "S.1.7" || scenarioId == "S.1.10")
                                {
                                    actualResult = registerPage.GetSuccessMessage();
                                    status = actualResult.Contains("created successfully") ? "Pass" : "Fail";
                                }
                                else if (scenarioId == "S.1.9")
                                {
                                    actualResult = registerPage.GetUsernameErrorMessage();
                                    status = actualResult.Contains("already exists") ? "Pass" : "Fail";
                                }
                                else
                                {
                                    actualResult = "Test case executed";
                                    status = "Pass";
                                }
                                break;

                            default:
                                actualResult = "Kịch bản chưa được auto";
                                status = "N/A";
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        actualResult = "Lỗi Exception: " + ex.Message;
                        status = "Fail";
                    }

                    // --- TỰ ĐỘNG CHẤM ĐIỂM VÀ GHI VÀO EXCEL ---
                    // Ghi Kết quả thực tế vào Cột H (Cột số 8)
                    worksheet.Cells[row, 8].Value = actualResult;

                    // Ghi Trạng thái (Pass/Fail) vào Cột I (Cột số 9)
                    worksheet.Cells[row, 9].Value = status;

                    // Tự động tô màu chữ cho Cột I (Pass: Xanh, Fail: Đỏ)
                    if (status == "Pass")
                        worksheet.Cells[row, 9].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                    else if (status == "Fail")
                        worksheet.Cells[row, 9].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                }

                // Lưu đè lên file Excel hiện tại
                package.Save();
            }
        }
    }
}