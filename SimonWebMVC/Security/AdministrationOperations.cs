namespace SimonWebMVC.Security
{
    public static class AdministrationOperations
    {
        public static OperationRequirement ListRoles = new OperationRequirement {Name=Constants.ListRoles};
        public static OperationRequirement CreateRole = new OperationRequirement {Name=Constants.CreateRole};
        public static OperationRequirement EditRole = new OperationRequirement {Name=Constants.EditRole};
        public static OperationRequirement EditRoleName = new OperationRequirement {Name=Constants.EditRoleName};
        public static OperationRequirement EditUsersInRole = new OperationRequirement {Name=Constants.EditUsersInRole}; 
        public static OperationRequirement DeleteRole = new OperationRequirement {Name=Constants.DeleteRole};

        public static OperationRequirement ListUsers = new OperationRequirement {Name=Constants.ListUsers};
        public static OperationRequirement CreateUser = new OperationRequirement {Name=Constants.CreateUser};
        public static OperationRequirement EditUser = new OperationRequirement {Name=Constants.EditUser}; 
        public static OperationRequirement DeleteUser = new OperationRequirement {Name=Constants.DeleteUser};
        public static OperationRequirement ManageUserRoles = new OperationRequirement {Name=Constants.ManageUserRoles};
    }
}