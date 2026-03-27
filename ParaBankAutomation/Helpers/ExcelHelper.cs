using OfficeOpenXml; 
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace ParaBankAutomation.Helpers
{
    public static class ExcelHelper
    {
        private static readonly string excelFilePath = @"C:\Users\vungo\OneDrive\Tài liệu\2025\BDCLPM\nhom6.xlsx";
        static ExcelHelper()
        {
           
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // Hàm đọc dữ liệu (Test Data)
        public static string GetTestData(string sheetName, string scenarioId)
        {
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets[sheetName];
                if (worksheet == null) return null;

                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    var currentScenarioId = worksheet.Cells[row, 2].Text; // Cột B (Scenario ID)
                    if (currentScenarioId == scenarioId)
                    {
                        return worksheet.Cells[row, 7].Text; // Cột G (Test Data)
                    }
                }
            }
            return string.Empty;
        }

        public static List<string> GetTestDataList(string sheetName, string scenarioId)
        {
            List<string> testDataList = new List<string>();

           
            if (!File.Exists(excelFilePath))
            {
                throw new FileNotFoundException($"[LỖI NGHIÊM TRỌNG]: Không tìm thấy file Excel tại đường dẫn: '{excelFilePath}'. Vui lòng copy lại đường dẫn chuẩn xác!");
            }

            FileInfo file = new FileInfo(excelFilePath);

            using (var package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets[sheetName];

                
                if (worksheet == null)
                {
                    throw new Exception($"[LỖI NGHIÊM TRỌNG]: Không tìm thấy Tab nào có tên là '{sheetName}' trong file Excel. Vui lòng mở Excel ra kiểm tra xem có dư khoảng trắng nào không!");
                }

                int rowCount = worksheet.Dimension.Rows;
                bool foundScenario = false;

                for (int row = 2; row <= rowCount; row++)
                {
                    string currentScenario = worksheet.Cells[row, 2].Text.Trim();

                    if (currentScenario == scenarioId)
                    {
                        foundScenario = true;
                    }
                    else if (foundScenario && !string.IsNullOrEmpty(currentScenario))
                    {
                        break;
                    }

                    if (foundScenario)
                    {
                        string data = worksheet.Cells[row, 7].Text.Trim();
                        testDataList.Add(data);
                    }
                }

                
                if (testDataList.Count == 0)
                {
                    throw new Exception($"[LỖI NGHIÊM TRỌNG]: Đã mở được Sheet '{sheetName}' nhưng tìm mỏi mắt không thấy Scenario ID '{scenarioId}' ở Cột B, hoặc cột Test Data (Cột G) của nó bị trống hoàn toàn!");
                }
            }
            return testDataList;
        }

        // Hàm ghi trạng thái PASS/FAIL và chèn ảnh vào file Excel
        public static void WriteResultAndScreenshot(string sheetName, string scenarioId, string status, string screenshotPath = null)
        {
            FileInfo file = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets[sheetName];
                if (worksheet == null) return;

                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 2].Text == scenarioId)
                    {
                        worksheet.Cells[row, 10].Value = status;

                        worksheet.Cells[row, 10].Style.Font.Bold = true;
                        if (status == "PASS")
                        {
                            worksheet.Cells[row, 10].Style.Font.Color.SetColor(Color.Green);
                        }
                        else if (status == "FAIL")
                        {
                            worksheet.Cells[row, 10].Style.Font.Color.SetColor(Color.Red);
                        }

                        if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                        {
                            string pictureName = $"Fail_Pic_{scenarioId}";

                            var existingPicture = worksheet.Drawings[pictureName];
                            if (existingPicture != null)
                            {
                                worksheet.Drawings.Remove(existingPicture);
                            }

                            var picture = worksheet.Drawings.AddPicture(pictureName, new FileInfo(screenshotPath));
                            picture.SetPosition(row - 1, 5, 11, 5);
                            picture.SetSize(150, 80);
                        }
                        break;
                    }
                }
                package.Save();
            }
        }
    }
}