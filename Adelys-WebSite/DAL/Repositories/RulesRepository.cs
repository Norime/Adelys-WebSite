using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Microsoft.EntityFrameworkCore;

namespace Adelys_WebSite.DAL.Repositories
{
    public class RulesRepository : IRulesRepository
    {
        private readonly SiteDbContext _context;

        public RulesRepository(SiteDbContext context)
        {
            _context = context;
        }

        public List<Law> GetAllLaws()
        {
            return _context.Laws.Include(l => l.Amendments).ToList();
        }

        public Law? GetLaw(int id)
        {
            return _context.Laws.Include(l => l.Amendments).FirstOrDefault(l => l.Id == id);
        }

        public Law? GetLawByNumber(string number)
        {
            return _context.Laws.FirstOrDefault(l => l.Number == number);
        }

        public void CreateLaw(Law law)
        {
            _context.Laws.Add(law);
            _context.SaveChanges();
        }

        /// <summary>
        /// Les lois et réunions viennent du contexte et restent suivies, les modifications sont détectées ici
        /// </summary>
        public void Update()
        {
            _context.SaveChanges();
        }

        public void DeleteLaw(Law law)
        {
            _context.Laws.Remove(law);
            _context.SaveChanges();
        }

        public List<ParliamentMeeting> GetAllMeetings()
        {
            return _context.ParliamentMeetings.OrderByDescending(m => m.HeldOn).ThenByDescending(m => m.Id).ToList();
        }

        public ParliamentMeeting? GetMeeting(int id)
        {
            return _context.ParliamentMeetings.FirstOrDefault(m => m.Id == id);
        }

        public void CreateMeeting(ParliamentMeeting meeting)
        {
            _context.ParliamentMeetings.Add(meeting);
            _context.SaveChanges();
        }

        public void DeleteMeeting(ParliamentMeeting meeting)
        {
            _context.ParliamentMeetings.Remove(meeting);
            _context.SaveChanges();
        }

        public List<ParliamentImage> GetImages(IEnumerable<string> fileNames)
        {
            List<string> names = fileNames.ToList();
            return _context.ParliamentImages.Where(i => names.Contains(i.FileName)).ToList();
        }

        public List<ParliamentImage> GetMeetingImages(int meetingId)
        {
            return _context.ParliamentImages.Where(i => i.MeetingId == meetingId).ToList();
        }

        public List<ParliamentImage> GetOrphanImages(DateTime uploadedBeforeUtc)
        {
            return _context.ParliamentImages.Where(i => i.MeetingId == null && i.UploadedAtUtc < uploadedBeforeUtc).ToList();
        }

        public void CreateImage(ParliamentImage image)
        {
            _context.ParliamentImages.Add(image);
            _context.SaveChanges();
        }

        public void DeleteImages(IEnumerable<ParliamentImage> images)
        {
            _context.ParliamentImages.RemoveRange(images);
            _context.SaveChanges();
        }
    }
}
