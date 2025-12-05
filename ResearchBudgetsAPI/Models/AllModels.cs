using System.ComponentModel.DataAnnotations;

namespace RuppinResearchBudget.Models
{
    public class Users
    {
        public string IdNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserRoles
    {
        public string IdNumber { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }


    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class ResearchCenters
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public string ManagerId { get; set; } = string.Empty;
    }

    public class Researches
    {
        public int ResearchId { get; set; }
        public string ResearchCode { get; set; } = string.Empty;
        public string ResearchName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TotalBudget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsUnderCenter { get; set; }
        public int? CenterId { get; set; }
        public string PrimaryResearcherId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ResearchMembers
    {
        public int ResearchId { get; set; }
        public string IdNumber { get; set; } = string.Empty;
        public string ResearchRole { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Returned by SP
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class BudgetCategories
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "שם קטגוריה הוא שדה חובה")]
        [StringLength(100, ErrorMessage = "שם הקטגוריה יכול להכיל עד 100 תווים")]
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class Files
    {
        public int FileId { get; set; }
        public int ResearchId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string UploadedById { get; set; } = string.Empty;
    }

    public class PaymentRequests
    {
        public int PaymentRequestId { get; set; }
        public int ResearchId { get; set; }
        public string RequestedById { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "ILS";
        public DateTime RequestDate { get; set; }
        public string? Description { get; set; }
        public int? FileId { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class PaymentApprovals
    {
        public int ApprovalId { get; set; }
        public int PaymentRequestId { get; set; }
        public string ApprovedById { get; set; } = string.Empty;
        public string ApprovalRole { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ApprovalDate { get; set; }
        public string? Comment { get; set; }
    }
    //תוצאות של SP:

    public class UserWithRoles : Users
    {
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class PaymentRequestWithDetails : PaymentRequests
    {
        public string ResearchName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        public string RequestedByFirstName { get; set; } = string.Empty;
        public string RequestedByLastName { get; set; } = string.Empty;
        public string? FileOriginalName { get; set; }
        //public string? FileStoredName { get; set; }
        public string? FileRelativePath { get; set; }

    }

    public class ResearchBudgetDetails
    {
        public int ResearchId { get; set; }
        public string ResearchName { get; set; } = string.Empty;
        public string ResearchCode { get; set; } = string.Empty;
        public decimal TotalBudget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal TotalApprovedExpenses { get; set; }
        public List<BudgetCategoryTotal> ExpensesByCategory { get; set; } = new();
        public List<PaymentRequestDetailsRow> Requests { get; set; } = new();
        public decimal RemainingBudget { get; set; }
    }

    public class BudgetCategoryTotal
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalByCategory { get; set; }
    }

    public class PaymentRequestDetailsRow
    {
        public int PaymentRequestId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? FileId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public string? FileOriginalName { get; set; }
        public string? FileRelativePath { get; set; }

    }
}

