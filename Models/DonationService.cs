namespace GiftOfTheGivers.Models
{
    public class DonationService
    {
        public bool ValidateDonationAmount(decimal amount)
        {
            return amount > 0;
        }

        public string FormatReceiptNumber(int donationId)
        {
            return $"GOTG-DON-{donationId:D6}";
        }
    }
}