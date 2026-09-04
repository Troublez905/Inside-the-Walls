namespace InsideTheWalls.Application
{
    public static class MenuAvailability
    {
        public static bool CanContinue(bool hasSave, bool saveIsCompatible)
        {
            return hasSave && saveIsCompatible;
        }

        public static string ContinueReason(bool hasSave, bool saveIsCompatible)
        {
            if (!hasSave)
            {
                return "No saved session";
            }

            return saveIsCompatible ? string.Empty : "Saved session is incompatible";
        }
    }
}
