using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface IRulesRepository
    {
        List<Law> GetAllLaws();
        Law? GetLaw(int id);
        Law? GetLawByNumber(string number);
        void CreateLaw(Law law);
        void Update();
        void DeleteLaw(Law law);

        List<ParliamentMeeting> GetAllMeetings();
        ParliamentMeeting? GetMeeting(int id);
        void CreateMeeting(ParliamentMeeting meeting);
        void DeleteMeeting(ParliamentMeeting meeting);

        List<ParliamentImage> GetImages(IEnumerable<string> fileNames);
        List<ParliamentImage> GetMeetingImages(int meetingId);
        List<ParliamentImage> GetOrphanImages(DateTime uploadedBeforeUtc);
        void CreateImage(ParliamentImage image);
        void DeleteImages(IEnumerable<ParliamentImage> images);
    }
}
