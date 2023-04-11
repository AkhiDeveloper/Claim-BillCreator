﻿using OfficeOpenXml;
using src.ServiceProvider;

try
{
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    Console.WriteLine("!---------- Claim Bill Creator (Rojia) ------------!");
    Console.WriteLine();
    Console.Write("Username:\t");
    var username = Console.ReadLine();
    Console.Write("Password:\t");
    var password = Console.ReadLine();
    string userid = "630";
    var year = DateTime.Now.Year;
    Console.WriteLine($"Selected Year:\t{year}");
    char choice;
    Console.Write("Press Y/y to change year or any key to proceed?\t");
    choice = Console.ReadKey().KeyChar.ToString().ToLower()[0];
    while (choice == 'y')
    {
        Console.Write("Enter the year:\t");
        var input = Console.ReadLine();
        var isInt = int.TryParse(input, out year);
        if (isInt)
        {
            choice = ' ';
        }
    }
    var month = DateTime.Now.Month - 1;
    Console.WriteLine($"Selected Year:\t{month}");
    Console.Write("Press Y/y to change year or any key to proceed?\t");
    choice = Console.ReadKey().KeyChar.ToString().ToLower()[0];
    while (choice == 'y')
    {
        Console.Write("Enter the year:\t");
        var input = Console.ReadLine();
        var isInt = int.TryParse(input, out month);
        if (isInt)
        {
            if (month > 0 && month < 13)
            {
                choice = ' ';
            }
        }
    }
    string baseAddress = "https://brit.udn.rosia.one/api";
    IBillFetcher fetcher = new RojiaApiBillFetcher(baseAddress, username, password);
    var from = new DateOnly(year, month, 1);
    var to = new DateOnly(year, month+1, 1);
    var bills = await fetcher.FetchInvoices(from, to, userid);

    var combined_bills = await BillCombinerStatic.CombineBillByOutlet(bills);

    IWholeSaleClaimCreator claimCreator = new WholsaleClaimCreator();
    var claim = claimCreator.CreateClaimFromBills(combined_bills);
}
catch
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Operation Failed");
}
finally
{
    Console.ForegroundColor= ConsoleColor.White;
    Console.WriteLine("Press any key to exit?");
    int i = 0;
    do
    {
        var key = Console.ReadKey().KeyChar;
        if(key == '0')
        {
            break;
        }
        i++;
    } while (i < 10);
}