using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IRulesBL
    {
        List<Law> GetAllLaws();
        Law? GetLaw(int id);
        bool IsLawNumberTaken(string number, int? exceptId);
        void CreateLaw(LawFormViewModel form);
        void UpdateLaw(Law law, LawFormViewModel form);
        void DeleteLaw(Law law);

        List<ParliamentMeeting> GetAllMeetings();
        ParliamentMeeting? GetMeeting(int id);
        void CreateMeeting(MeetingFormViewModel form, string createdBy);
        void UpdateMeeting(ParliamentMeeting meeting, MeetingFormViewModel form);
        void DeleteMeeting(ParliamentMeeting meeting);

        Task<string> SaveImage(IFormFile file, string extension);
    }
}
