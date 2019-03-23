namespace Ozon.Examination.Service.Filters
{
    public interface ITextReport
    {
        /// <summary>
        /// Get report in text format
        /// </summary>
        /// <returns>Text report</returns>
        string GetReport();
    }
}
