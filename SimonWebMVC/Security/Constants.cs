using ExchangeRatesLib;

namespace SimonWebMVC.Security
{
    public class Constants
    {
        // Car operations
        public static readonly string CreateCar = "Create";
        public static readonly string EditCar = "Edit";
        public static readonly string DeleteCar = "Delete";

        // Application roles
        public static readonly string UserRole = "User";
        public static readonly string AdminRole = "Admin";
        public static readonly string SuperAdminRole = "SuperAdmin";

        // Operations on users
        public static readonly string CreateUser = "Create";
        public static readonly string EditUser = "Edit";
        public static readonly string DeleteUser = "Delete";

        // Administration operations
        public static readonly string ListRoles = "List";
        public static readonly string CreateRole = "Create";
        public static readonly string EditRole = "Edit";
        public static readonly string DeleteRole = "Delete";

        public static readonly string EditRoleName = "EditName";
        public static readonly string EditUsersInRole = "EditUsersInRole";

        public static readonly string ListUsers = "ListUsers";
        public static readonly string ManageUserRoles = "ManageUserRoles";

        public static readonly int PageSize = 25;
        public static readonly CurrencyEnum DefaultCurrency = CurrencyEnum.EUR;
    }
}