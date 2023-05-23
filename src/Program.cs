using OfficeOpenXml;
using src.ServiceProvider;
try
{
    //Console.WriteLine(Directory.GetCurrentDirectory());
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    Console.WriteLine("!---------- Claim Bill Creator (Rojia) ------------!");
    Console.WriteLine();
    Console.Write("Username:\t");
    var username = Console.ReadLine();
    Console.Write("Password:\t");
    var password = Console.ReadLine();
    string? userid = null;
    string? companyName = null;
    string? address = null;
    string? phoneNumber = null;
    switch (username.Trim())
    {
        case "mishraaalok123@gmail.com":
            userid = "630";
            companyName = "MAA KALI AND LAXMI ENTERPRISES";
            address = "SHANTINAGAR,BANESHWOR";
            phoneNumber = "9849918525/9843888939";
            break;
        case "baisnabdevi@gmail.com":
            userid = "1309";
            companyName = "Ma baisnabdevi enterprises";
            address = "Himsekhar marg, shantinagar";
            phoneNumber = "9843888939/9808532091";
            break;
    }
    var year = DateTime.Now.Year;
    Console.WriteLine($"Selected Year:\t{year}");
    char choice;
    Console.Write("Press Y/y to change year or any key to proceed?\t");
    choice = Console.ReadKey().KeyChar.ToString().ToLower()[0];
    Console.WriteLine();
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
    Console.WriteLine($"Selected Month:\t{month}");
    Console.Write("Press Y/y to change year or any key to proceed?\t");
    choice = Console.ReadKey().KeyChar.ToString().ToLower()[0];
    Console.WriteLine();
    while (choice == 'y')
    {
        Console.Write("Enter the month (1-12):\t");
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
    IBillFetcher fetcher = new RojiaApiWholeSaleBillFetcher(baseAddress, username, password);
    var from = new DateOnly(year, month, 1);
    var to = new DateOnly(year, month+1, 1);
    var bills = await fetcher.FetchInvoices(from, to, userid);
    var combined_bills = await BillCombinerStatic.CombineBillByOutlet(bills);
    IClaimFileCreator fileCreator = new ClaimExcelFileCreator();
    var filename = $"{username}_claim_{DateTime.Now.Ticks}.xlsx";
    var filepath = Path.Combine(Global.DefaultClaimSavePath, filename);
    await fileCreator.CreateFile(companyName, combined_bills, discount: 0.05m, filepath: filepath, year: year, month: month, address: address, phoneNumber: phoneNumber);
    fileCreator.Dispose();
    Console.WriteLine("Sucessfull downloaded");
}
catch(Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Operation Failed");
    Console.WriteLine(ex.Message);
}
finally
{
    Console.ForegroundColor= ConsoleColor.White;
    Console.WriteLine("Press 0 to exit?");
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
