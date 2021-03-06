﻿
namespace ALS.Glance.Models.Security.Interfaces
{
    public interface IIdentityUserClaim <TKey>
    {
        // Summary:
        //     Claim type
        string ClaimType { get; set; }
        //
        // Summary:
        //     Claim value
        string ClaimValue { get; set; }
        //
        // Summary:
        //     Primary key
        int Id { get; set; }
        //
        // Summary:
        //     User Id for the user who owns this login
        TKey UserId { get; set; }
    }
}
