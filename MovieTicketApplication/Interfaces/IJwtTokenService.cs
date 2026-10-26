using MovieTicketBookingDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Users user, string roleName);
    }
}
