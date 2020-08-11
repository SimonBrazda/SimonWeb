namespace SimonWebMVC.Security
{
    public static class CarOperations
    {
        public static OperationRequirement Create = new OperationRequirement {Name=Constants.CreateCar};
        public static OperationRequirement Edit = new OperationRequirement {Name=Constants.EditCar}; 
        public static OperationRequirement Delete = new OperationRequirement {Name=Constants.DeleteCar};
    }
}