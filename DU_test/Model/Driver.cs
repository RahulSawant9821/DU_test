using DU_test.Model.DTOs;
using Microsoft.AspNetCore.Identity;

namespace DU_test.Model
{
    public class Driver
    {
    
        public string username { get; set; }

        public string password { get; set; }

        public JOffer jobOffers { get; set; }

    }
}
