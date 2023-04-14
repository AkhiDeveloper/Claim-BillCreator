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
        private readonly ExcelWorksheet _billFormatSheet;
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
            _billFormatSheet = _formatBook.Worksheets[1];
        }

        ~ClaimExcelFileCreator() 
        {
            _claimFormatSheet.Dispose();
            _formatBook.Dispose();
            _formatPack.Dispose();
        }

        public async Task CreateFile(IEnumerable<Bill> bills, string? filepath = null, decimal discount = 0.00m, decimal taxRate = 0.13m)
        {
            //creating claim
            var claim = _claimCreator.CreateClaimFromBills(bills, discount, taxRate);

            if (String.IsNullOrEmpty(filepath))
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
            await this.CreateClaimSheet(claim, claimSheet);
            foreach(var bill in bills)
            {
                var billSheet = formatworkbook.Worksheets.Add($"Bill_{bill.OutletName}", _billFormatSheet);
                await this.CreateBillSheet(bill, billSheet, discount, taxRate);
            }
            await excelpack.SaveAsync();
            claimSheet.Dispose();
            formatworkbook.Dispose();
            excelpack.Dispose();
        }

        public Task CreateFile(Claim claim, IEnumerable<Bill> bills, string? filepath = null, decimal discount = 0.00m, decimal taxRate = 0.13m)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _claimFormatSheet.Dispose();
            _formatBook.Dispose();
            _formatPack.Dispose();
        }

        private Task CreateBillSheet(Bill bill, ExcelWorksheet billSheet, decimal discount = 0.00m, decimal taxRate = 0.13m)
        {
            return Task.Run(() =>
            {
                billSheet.Cells["A1"].Value = bill.OutletName;
                int datarow = 4;
                var bill_items = bill.Items;
                billSheet.InsertRow(datarow, bill_items.Count()-1);
                for(int i = 0; i < bill_items.Count(); i++)
                {
                    for(int j = 0; j < 6; j++)
                    {
                        billSheet.Cells[datarow + i, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    var item = bill_items[i];
                    var cell = billSheet.Cells[datarow + i, 1];
                    cell.Offset(0, 0).Value = i + 1;
                    var nameCell = cell.Offset(0, 1);
                    nameCell.Value = item.Name;
                    nameCell.Style.WrapText = true;
                    if(item.DiscountRate > 0.00m)
                    {
                        cell.Offset(0, 2).Value = "P";
                    }
                    cell.Offset(0, 3).Value = item.Quantity;
                    cell.Offset(0, 4).Value = item.Rate;
                    cell.Offset(0, 5).Value = item.Amount;
                }
                var subTotalCell = billSheet.Cells["F5"].Offset(bill_items.Count() - 1, 0);
                var discountCell = subTotalCell.Offset(1, 0);
                var taxableCell = subTotalCell.Offset(2, 0);
                var VATCell = subTotalCell.Offset(3, 0);
                var totalCell = subTotalCell.Offset(4, 0);
                subTotalCell.Value = bill.SubTotal();
                discountCell.Value = bill.Discount(discount);
                taxableCell.Value = bill.TaxableAmount(discount);
                VATCell.Value = bill.TaxAmount(discount: discount, taxRate: taxRate);
                totalCell.Value = bill.TotalAmount(discount: discount, taxRate: taxRate);
                billSheet.PrinterSettings.PaperSize = ePaperSize.A4;
                billSheet.PrinterSettings.BlackAndWhite = true;
            });
        }

        private Task CreateClaimSheet(Claim claim, ExcelWorksheet claimSheet)
        {
            return Task.Run(() =>
            {
                claimSheet.Cells["D5"].Value = $"Date: {claim.CreatedDate.ToString("yyyy/MM/dd")}";
                claimSheet.Cells["A7"].Value = $"sub: details about whole sales discount {claim.CreatedDate.ToString("MMMMM")}".ApplyCase(LetterCasing.Title);
                claimSheet.Cells["A9"].Value = "dear sir/madam,".ApplyCase(LetterCasing.Title);
                claimSheet.Cells["A10"].Value = "              With reference to the above subject, I would like to submit my expenses at wholesales. The details about expenses are given below.";

                int datarow = 13;
                var claim_items = claim.GetPartyClaims();
                claimSheet.InsertRow(datarow + 1, claim_items.Count() - 1);
                for (int i = 0; i < claim_items.Count(); i++)
                {
                    if (i > 0)
                    {
                        claimSheet.DeleteRow(44);
                    }
                    for (int j = 0; j < 6; j++)
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
            });
        }
    }
}
