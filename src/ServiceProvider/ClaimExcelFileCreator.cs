using Humanizer;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public class ClaimExcelFileCreator
        : IClaimFileCreator, IDisposable
    {
        private readonly ExcelWorksheet _claimFormatSheet;
        private readonly ExcelWorkbook  _formatBook;
        private readonly ExcelPackage _formatPack;
        private readonly IFileManager _fileManager;
        private readonly IClaimCreator _claimCreator;
        private readonly IBillCombiner _billCombiner;
         
        public ClaimExcelFileCreator()
        {
            _fileManager = new FileManager();
            _claimCreator = new ClaimCreator();
            _billCombiner = new BillCombiner();
            string formatFilePath = "Assets/Excel/ClaimFormat.xlsx";
            _formatPack = new ExcelPackage(formatFilePath);
            _formatBook = _formatPack.Workbook;
            _claimFormatSheet = _formatBook.Worksheets[0];
        }

        ~ClaimExcelFileCreator() 
        {
            _claimFormatSheet.Dispose();
            _formatBook.Dispose();
            _formatPack.Dispose();
        }

        public async Task CreateFile(IEnumerable<Bill> bills, string? filepath = null)
        {
            if(String.IsNullOrEmpty(filepath))
            {
                filepath = @$"C:\Users\DishHome\Desktop\claimfile_{DateTime.UtcNow.Ticks}.xlsx";
            }
            var parentfolder = Directory.GetParent(filepath);
            if (!parentfolder.Exists)
            {
                parentfolder.Create();
            }
            //getting ready excel sheet
            File.Create(filepath).Close();
            var excelpack = new ExcelPackage(new FileInfo(filepath));
            var formatworkbook = excelpack.Workbook;
            var claimSheet = formatworkbook.Worksheets.Add("Claim", _claimFormatSheet);

            //creating claim
            var combined_bills = await _billCombiner.CombineBillByOutlet(bills);
            var claim = _claimCreator.CreateClaimFromBills(combined_bills);

            claimSheet.Cells["D5"].Value = $"Date: {claim.CreatedDate.ToString("yyyy/MM/dd")}";
            claimSheet.Cells["A7"].Value = $"sub: details about whole sales discount {claim.CreatedDate.ToString("MMMMM")}".ApplyCase(LetterCasing.Title);
            claimSheet.Cells["A9"].Value = "dear sir/madam,".ApplyCase(LetterCasing.Title);
            claimSheet.Cells["A10"].Value = "              With reference to the above subject, I would like to submit my expenses at wholesales. The details about expenses are given below.";

            int datarow = 13;
            var claim_items = claim.GetPartyClaims();
            claimSheet.InsertRow(datarow + 1, claim_items.Count() - 1);
            for (int i = 0; i < claim_items.Count(); i++)
            {
                if(i > 0)
                {
                    claimSheet.DeleteRow(44);
                }
                for(int j = 0; j < 6; j++)
                {
                    claimSheet.Cells[datarow + i, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (j == 1)
                    {
                        claimSheet.Cells[datarow + i, j + 1].Style.WrapText = true;
                        continue;
                    }
                    claimSheet.Cells[datarow + i, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                var item = claim_items.ElementAt(i);
                claimSheet.Cells[datarow + i, 1].Value = i + 1;
                claimSheet.Cells[datarow + i, 2].Value = item.Party;
                claimSheet.Cells[datarow + i, 3].Value = item.Amount;
                if (item.Discount > 0.01m)
                {
                    claimSheet.Cells[datarow + i, 5].Value = item.ClaimAmount;
                }
                else
                {
                    claimSheet.Cells[datarow + i, 4].Value = item.ClaimAmount;
                }
                claimSheet.Cells[datarow + i, 6].Value = item.ClaimAmount;
            }
            claimSheet.Cells[datarow + claim_items.Count(), 6].Value = claim.TotalClaimAmount;
            claimSheet.PrinterSettings.PaperSize = ePaperSize.A4;
            claimSheet.PrinterSettings.BlackAndWhite = true;
            excelpack.Save();
            claimSheet.Dispose();
            formatworkbook.Dispose();
            excelpack.Dispose();
        }

        public Task CreateFile(Claim claim, IEnumerable<Bill> bills, string? filepath = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _claimFormatSheet.Dispose();
            _formatBook.Dispose();
            _formatPack.Dispose();
        }
    }
}
