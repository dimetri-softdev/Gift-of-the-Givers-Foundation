namespace GiftOfTheGivers.Models
{
    public class VolunteerService
    {
        public bool IsEligibleForDeployment(int hoursLogged)
        {
            return hoursLogged >= 10;
        }

        public string GenerateVolunteerBadgeCode(int volunteerId)
        {
            return $"GOTG-VOL-{volunteerId:D5}";
        }
    }
}