
namespace DLMS_DataAccess
{
    public class Data
    {
        public static string SortingText { set; get; }
        private static string sortingType = "DESC";
        private static string currentSortingText;
        public static bool IsSortingUsed { get; set; }
        public static string GetSortingCommand()
        {
            sortingType = currentSortingText == SortingText ? sortingType == "ASC" ? "DESC" : "ASC" : "ASC";
            currentSortingText = SortingText;
            return $"\nORDER BY '{SortingText}' {sortingType}";
        }
        public static string SortingCondition;

        public static void ApplySorting()
        {
            SortingCondition = IsSortingUsed ? GetSortingCommand() : "";
        }
    }
}
