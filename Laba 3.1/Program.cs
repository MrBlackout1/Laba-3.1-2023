using System;
using System.IO;

class Program
{
    static void Main()
    {
        string csvFilePath = "transactions.csv";
        string outputFilePath = "daily_expenses.csv";
        string dateFormat = "dd/MM/yyyy";

        Func<string, DateTime> getDate = GetDate;
        Func<string, double> getAmount = GetAmount;
        Action<DateTime, double> displayTotal = DisplayTotal;

        double dailyTotal = 0;
        DateTime currentDate = DateTime.MinValue;

        int count = 0; // Лічильник записів у вихідному файлі

        using (var reader = new StreamReader(csvFilePath))
        {
            using (var writer = new StreamWriter(outputFilePath, append: true))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    DateTime date = getDate(line);
                    double amount = getAmount(line);

                    if (date != currentDate)
                    {
                        if (currentDate != DateTime.MinValue)
                        {
                            displayTotal(currentDate, dailyTotal);
                            writer.WriteLine($"{currentDate.ToString(dateFormat)},{dailyTotal}");
                            count++;

                            if (count == 10)
                            {
                                // Перезаписати дані в новий файл
                                writer.Flush();
                                writer.Close();

                                // Скинути лічильник та відкрити новий файл для запису
                                count = 0;
                            }
                        }

                        currentDate = date;
                        dailyTotal = 0;
                    }

                    dailyTotal += amount;
                }

                // Відобразити та записати останню суму
                displayTotal(currentDate, dailyTotal);
                writer.WriteLine($"{currentDate.ToString(dateFormat)},{dailyTotal}");
                writer.Flush();
                writer.Close();
            }
        }
    }

    static DateTime GetDate(string line)
    {
        string[] fields = line.Split(',');
        string dateString = fields[0]; // Перше поле - дата
        return DateTime.Parse(dateString);
    }

    static double GetAmount(string line)
    {
        string[] fields = line.Split(',');
        string amountString = fields[1]; // Друге поле - сума
        return double.Parse(amountString);
    }

    static void DisplayTotal(DateTime date, double total)
    {
        Console.WriteLine($"{date.ToShortDateString()} - Total: {total}");
    }
}
