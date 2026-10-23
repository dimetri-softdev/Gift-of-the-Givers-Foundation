namespace GiftOfTheGivers.Helpers
{
    public static class DonationHelper
    {
        public static string GenerateCertificateNumber()
        {
            return $"GOG-{DateTime.Now:yyyy}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        }
    }
}
