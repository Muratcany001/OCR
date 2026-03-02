namespace OCR.Helpers.OutputHelpers;

public class DateParser
{
    public static string parseDate(string date)
    {
        string[] dateParts = date.Split(',');

        foreach (var part in dateParts)
        {
            Console.WriteLine(part);
            
        }
        return string.Empty;
    }

}