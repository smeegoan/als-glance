using ALS.Glance.Models.Security.Implementations;

namespace ALS.Glance.Api.Models
{
    public class ApiUserRegistration
    {
        public string Email { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        /*

        public string DocumentNumber { get; set; }

        public DocumentType DocumentType { get; set; }

        //*/

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }

        public bool AcceptsTermsAndConditions { get; set; }

        public IdentityUser User { get; set; }
    }
}