using EyeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.Services;

public class ProfileService
{
    private readonly ApplicationDbContext _context;
    public Profile? SelectedProfile { get; set; }

    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }


    public List<Profile> GetProfiles()
    {
        return _context.Profiles.AsNoTracking().ToList();
    }
    
}
