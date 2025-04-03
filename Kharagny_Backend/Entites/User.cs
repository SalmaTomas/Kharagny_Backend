using Microsoft.AspNetCore.Identity;

namespace Kharagny_Backend.Entites
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Area { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<Wishlist> wishlist { get; set; }
    }
}
