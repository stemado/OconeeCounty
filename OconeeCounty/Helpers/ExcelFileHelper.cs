using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Attributes;

namespace OconeeCounty.Helpers;

public static class ExcelFileHelper
{
    public enum OutPutType
    {
        Xlsx,
        Csv
    }

    public static ExcelRangeBase LoadFromCollectionFiltered<T>(this ExcelRangeBase @this, IEnumerable<T> collection)
        where T : class
    {
        MemberInfo[] membersToInclude = typeof(T)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnore)))
            .ToArray();

        return @this.LoadFromCollection<T>(collection, false,
            OfficeOpenXml.Table.TableStyles.None,
            BindingFlags.Instance | BindingFlags.Public,
            membersToInclude);
    }

    public static string SaveAs<T>(IEnumerable<T> data, OutPutType outputType, string filePath,
        bool passwordProtect = false,
        string? password = null)
    {
        if (password != null && password == string.Empty)
            throw new ArgumentException(nameof(password), "Cannot be empty if provided");

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //if(data.Count() is 0) throw new ArgumentException(nameof(data), "Data cannot be empty");

        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        var dictionary = ConvertToIDictionary(data);


        using (var package = new ExcelPackage())
        {
            var sheetName = Path.GetFileNameWithoutExtension(filePath).Replace("_", " ");
            var sheet = package.Workbook.Worksheets.Add(sheetName);

            // We start at B1 because the first column is a generated UniqueId that is not needed in the final output.
            var r = sheet.Cells["B1"].LoadFromDictionaries(dictionary, c =>
            {
                c.PrintHeaders = true;
                c.TableStyle = OfficeOpenXml.Table.TableStyles.Light1;
            });

            r.Worksheet.Cells.AutoFitColumns();

            if (outputType == OutPutType.Csv) package.ConvertToCsv();

            if (!passwordProtect)
            {
                package.SaveAs(filePath);
            }

            else
            {
                package.SaveAs(filePath, password);
            }
        }

        return "Completed";
    }

    public static string SaveAs<T>(IEnumerable<T> data, OutPutType outputType, string filePath,
        List<string>? columnsToExlude, bool passwordProtect = false,
        string? password = null)
    {
        columnsToExlude ??= new List<string>();

        if (password != null && password == string.Empty)
            throw new ArgumentException(nameof(password), "Cannot be empty if provided");

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //if(data.Count() is 0) throw new ArgumentException(nameof(data), "Data cannot be empty");

        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        var dictionary = ConvertToIDictionary(data);

        foreach (var d in dictionary)
        {
            foreach (var p in columnsToExlude)
            {
                d.Remove(p);
            }
        }

        using (var package = new ExcelPackage())
        {
            var sheetName = System.IO.Path.GetFileNameWithoutExtension(filePath).Replace("_", " ");
            var sheet = package.Workbook.Worksheets.Add(sheetName);
            var r = sheet.Cells["A1"].LoadFromDictionaries(dictionary, c =>
            {
                c.PrintHeaders = true;
                c.TableStyle = OfficeOpenXml.Table.TableStyles.Light1;
                c.SetKeys();
            });


            r.Worksheet.Cells.AutoFitColumns();

            if (outputType == OutPutType.Csv) package.ConvertToCsv();

            if (!passwordProtect)
            {
                package.SaveAs(filePath);
            }

            else
            {
                package.SaveAs(filePath, password);
            }
        }

        return "Completed";
    }

    public static byte[] ConvertToCsv(this ExcelPackage package)
    {
        var worksheet = package.Workbook.Worksheets[0];

        var maxColumnNumber = worksheet.Dimension.End.Column;
        var currentRow = new List<string>(maxColumnNumber);
        var totalRowCount = worksheet.Dimension.End.Row;
        var currentRowNum = 1;

        var memory = new MemoryStream();

        using (var writer = new StreamWriter(memory, Encoding.Default))
        {
            while (currentRowNum <= totalRowCount)
            {
                BuildRow(worksheet, currentRow, currentRowNum, maxColumnNumber);
                WriteRecordToFile(currentRow, writer, currentRowNum, totalRowCount);
                currentRow.Clear();
                currentRowNum++;
            }
        }

        return memory.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="record">List of cell values</param>
    /// <param name="sw">Open Writer to file</param>
    /// <param name="rowNumber">Current row num</param>
    /// <param name="totalRowCount"></param>
    /// <remarks>Avoiding writing final empty line so bulk import processes can work.</remarks>
    private static void WriteRecordToFile(List<string> record, StreamWriter sw, int rowNumber, int totalRowCount)
    {
        var commaDelimitedRecord = record.ToDelimitedString(",");

        if (rowNumber == totalRowCount)
        {
            sw.Write(commaDelimitedRecord);
        }
        else
        {
            sw.WriteLine(commaDelimitedRecord);
        }
    }

    private static string DuplicateTicksForSql(this string s)
    {
        return s.Replace("'", "''");
    }

    /// <summary>
    /// Takes a List collection of string and returns a delimited string.  Note that it's easy to create a huge list that won't turn into a huge string because
    /// the string needs contiguous memory.
    /// </summary>
    /// <param name="list">The input List collection of string objects</param>
    /// <param name="qualifier">
    /// The default delimiter. Using a colon in case the List of string are file names,
    /// since it is an illegal file name character on Windows machines and therefore should not be in the file name anywhere.
    /// </param>
    /// <param name="insertSpaces">Whether to insert a space after each separator</param>
    /// <returns>A delimited string</returns>
    /// <remarks>This was implemented pre-linq</remarks>
    public static string ToDelimitedString(this List<string> list, string delimiter = ":", bool insertSpaces = false,
        string qualifier = "", bool duplicateTicksForSQL = false)
    {
        var result = new StringBuilder();
        for (int i = 0; i < list.Count; i++)
        {
            string initialStr = duplicateTicksForSQL ? list[i].DuplicateTicksForSql() : list[i];
            result.Append((qualifier == string.Empty) ? initialStr : string.Format("{1}{0}{1}", initialStr, qualifier));
            if (i < list.Count - 1)
            {
                result.Append(delimiter);
                if (insertSpaces)
                {
                    result.Append(' ');
                }
            }
        }

        return result.ToString();
    }

    private static void BuildRow(ExcelWorksheet worksheet, List<string> currentRow, int currentRowNum,
        int maxColumnNumber)
    {
        for (int i = 1; i <= maxColumnNumber; i++)
        {
            var cell = worksheet.Cells[currentRowNum, i];
            if (cell == null)
            {
                // add a cell value for empty cells to keep data aligned.
                AddCellValue(string.Empty, currentRow);
            }
            else
            {
                AddCellValue(GetCellText(cell), currentRow);
            }
        }
    }

    /// <summary>
    /// Can't use .Text: http://epplus.codeplex.com/discussions/349696
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private static string GetCellText(ExcelRangeBase cell)
    {
        return cell.Value == null ? string.Empty : cell.Value.ToString();
    }

    private static void AddCellValue(string s, List<string> record)
    {
        record.Add(string.Format("{0}{1}{0}", '"', s));
    }

    public static IEnumerable<IDictionary<string, object>> ConvertToIDictionary<T>(IEnumerable<T> data)
    {
        var dictionaryList = new List<IDictionary<string, object>>();

        if (data.Count() is 0)
        {
            dictionaryList.Add(new Dictionary<string, object>
            {
                {
                    "No Records Found", ""
                }
            });
            return dictionaryList;
        }

        foreach (var record in data)
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;
            var dictionary = expando;

            foreach (var property in record.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(record));
            }

            dictionaryList.Add(dictionary);
        }

        return dictionaryList;
    }

    public static List<T> ReadCsvFile<T>(string filePath, bool hasHeaderRecord = true, bool debug = true)
    {
        List<T> files = new List<T>();
        CsvConfiguration config;
        //Need some conversion to csv file before we can do this (and removing the header from xml file)
        if (hasHeaderRecord)
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                PrepareHeaderForMatch = (args => RemoveSpecialCharacter(args.Header)),
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = null,
                MissingFieldFound = debug ? new MissingFieldFound(args => args.HeaderNames.ToList()) : null,
            };
        }
        else
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,

                MissingFieldFound = null,
            };
        }


        try
        {
            using var fs = new StreamReader(filePath);
            using var csv = new CsvReader(fs, config);
            files = csv.GetRecords<T>().ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }


        return files;
    }

    public static string RemoveSpecialCharacter(string str)
    {
        str = new string(str.Where(c => !char.IsWhiteSpace(c)).ToArray());
        str = Regex.Replace(str, @"\s+", "");
        str = str.Replace("-", "");
        str = str.Replace(@"\", "");
        str = str.Replace(@"/", "");
        str = str.Replace(@"%", "Percent");
        str = str.Replace(@"`", "");
        str = str.Replace(@"'", "");
        str = str.Replace(@"*", "");
        str = str.Replace(@"[", "");
        str = str.Replace(@"]", "");
        str = str.Replace(@".", "");
        str = str.Replace(@"(", "");
        str = str.Replace(@")", "");

        return str;
    }
}