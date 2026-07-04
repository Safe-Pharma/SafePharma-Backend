namespace SafePharma.Common
{
    public class UserQueryParams
    {
        /// <summary>Matches against name or email — the "Search by name or email..." field.</summary>
        public string? Search { get; set; }

        /// <summary>Role name, or null/omitted for "All roles".</summary>
        public string? Role { get; set; }

        /// <summary>null = All statuses, true = Active, false = Inactive.</summary>
        public bool? IsActive { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;

        public string SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
    }
}
