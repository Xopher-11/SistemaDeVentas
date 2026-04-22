namespace SistemaDeVentas.Application.Utils
{
    public static class DateHelper
    {
        public static int ConvertToDateKey(DateTime date)
        {
            string value = date.ToString("yyyyMMdd");
            return Convert.ToInt32(value);
        }
    }
}