using System;
using Microsoft.EntityFrameworkCore;

namespace DataBaseProvider
{
    public class ProfileDBContext: DbContext
    {
        public ProfileDBContext(DbContextOptions options):base(options)
        {
        }
    }
}
