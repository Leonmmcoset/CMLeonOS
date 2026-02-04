using System;

namespace CMLeonOS.Commands.Utility
{
    public static class CalendarCommand
    {
        public static void ShowCalendar(string args)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            
            if (!string.IsNullOrEmpty(args))
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length >= 1)
                {
                    if (int.TryParse(parts[0], out int m) && m >= 1 && m <= 12)
                    {
                        month = m;
                    }
                }
                
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[1], out int y) && y >= 1 && y <= 9999)
                    {
                        year = y;
                    }
                }
            }
            
            DateTime firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DayOfWeek startDayOfWeek = firstDay.DayOfWeek;
            
            string[] monthNames = { 
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };
            
            Console.WriteLine($"     {monthNames[month - 1]} {year}");
            Console.WriteLine("  Su Mo Tu We Th Fr Sa");
            
            int dayOfWeek = (int)startDayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;
            
            for (int i = 1; i < dayOfWeek; i++)
            {
                Console.Write("   ");
            }
            
            for (int day = 1; day <= daysInMonth; day++)
            {
                Console.Write($"{day,2} ");
                
                dayOfWeek++;
                if (dayOfWeek > 7)
                {
                    dayOfWeek = 1;
                    Console.WriteLine();
                    Console.Write("  ");
                }
            }
            
            Console.WriteLine();
        }
    }
}
