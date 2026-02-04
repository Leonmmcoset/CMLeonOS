namespace CMLeonOS.Commands.User
{
    public static class CpassCommand
    {
        public static void ProcessCpass(CMLeonOS.UserSystem userSystem)
        {
            userSystem.ChangePassword();
        }
    }
}
