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
    
    public void Create(Profile profile)
    {
        _context.Profiles.Add(profile);
        _context.SaveChanges();
        _context.Entry(profile).State = EntityState.Detached;
    }
    public void Update(Profile profile)
    {
        var profileForUpdate = _context.Profiles.Where(x => x.Id == profile.Id).FirstOrDefault();
        if (profileForUpdate != null)
        {
            profileForUpdate.Name = profile.Name;
            _context.Profiles.Update(profileForUpdate);
            _context.SaveChanges();
            _context.Entry(profileForUpdate).State = EntityState.Detached;
        }
    }

    public void Delete(Guid profileId)
    {
        var profile = _context.Profiles.Where(x => x.Id == profileId).FirstOrDefault();
        if (profile != null)
        {
            _context.Profiles.Remove(profile);
            _context.SaveChanges();
        }
    }
}
