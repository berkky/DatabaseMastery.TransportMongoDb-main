namespace DatabaseMastery.TransportMongoDb.Security
{
    public static class AdminRoles
    {
        public const string SuperAdmin = "SuperAdmin";

        public const string Admin = "Admin";

        public const string Operator = "Operator";

        public const string Viewer = "Viewer";

        public const string AllAdminRoles =
            SuperAdmin + "," + Admin + "," + Operator + "," + Viewer;

        public const string OperatorsAndAbove =
            SuperAdmin + "," + Admin + "," + Operator;

        public const string AdminsOnly =
            SuperAdmin + "," + Admin;
    }
}
