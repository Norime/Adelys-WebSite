using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.ViewModels;
using System.Text.RegularExpressions;

namespace Adelys_WebSite.BL
{
    public partial class RulesBL : IRulesBL
    {
        private readonly IRulesRepository _repository;
        private readonly IContentImageStorage _imageStorage;
        private readonly IMarkdownBL _markdownBL;

        public RulesBL(IRulesRepository repository, IContentImageStorage imageStorage, IMarkdownBL markdownBL)
        {
            _repository = repository;
            _imageStorage = imageStorage;
            _markdownBL = markdownBL;
        }

        /// <summary>
        /// Lois dans l'ordre de leur numéro : 13, 13bis puis 14
        /// </summary>
        public List<Law> GetAllLaws()
        {
            return _repository.GetAllLaws()
                .OrderBy(l => int.Parse(LawNumberDigits().Match(l.Number).Value))
                .ThenBy(l => l.Number, StringComparer.Ordinal)
                .ToList();
        }

        public Law? GetLaw(int id)
        {
            return _repository.GetLaw(id);
        }

        public bool IsLawNumberTaken(string number, int? exceptId)
        {
            Law? law = _repository.GetLawByNumber(number);
            return law != null && law.Id != exceptId;
        }

        public void CreateLaw(LawFormViewModel form)
        {
            Law law = new Law();
            Fill(law, form);
            _repository.CreateLaw(law);
        }

        public void UpdateLaw(Law law, LawFormViewModel form)
        {
            Fill(law, form);
            _repository.Update();
        }

        public void DeleteLaw(Law law)
        {
            _repository.DeleteLaw(law);
        }

        public List<ParliamentMeeting> GetAllMeetings()
        {
            return _repository.GetAllMeetings();
        }

        public ParliamentMeeting? GetMeeting(int id)
        {
            return _repository.GetMeeting(id);
        }

        public void CreateMeeting(MeetingFormViewModel form, string createdBy)
        {
            ParliamentMeeting meeting = new ParliamentMeeting
            {
                Title = form.Title.Trim(),
                HeldOn = form.HeldOn!.Value.Date,
                Content = form.Content,
                CreatedBy = createdBy,
                CreatedAtUtc = DateTime.UtcNow
            };
            _repository.CreateMeeting(meeting);
            SyncImages(meeting);
        }

        public void UpdateMeeting(ParliamentMeeting meeting, MeetingFormViewModel form)
        {
            meeting.Title = form.Title.Trim();
            meeting.HeldOn = form.HeldOn!.Value.Date;
            meeting.Content = form.Content;
            meeting.UpdatedAtUtc = DateTime.UtcNow;
            _repository.Update();
            SyncImages(meeting);
        }

        public void DeleteMeeting(ParliamentMeeting meeting)
        {
            List<ParliamentImage> images = _repository.GetMeetingImages(meeting.Id);
            _repository.DeleteMeeting(meeting);
            DeleteImages(images);
        }

        public async Task<string> SaveImage(IFormFile file, string extension)
        {
            DeleteImages(_repository.GetOrphanImages(DateTime.UtcNow - MarkdownBL.OrphanImageDelay));

            string fileName = await _imageStorage.Save(file, extension);
            try
            {
                _repository.CreateImage(new ParliamentImage { FileName = fileName, UploadedAtUtc = DateTime.UtcNow });
            }
            catch
            {
                _imageStorage.Delete(fileName);
                throw;
            }
            return MarkdownBL.ImageRequestPath + fileName;
        }

        private static void Fill(Law law, LawFormViewModel form)
        {
            law.Number = form.Number.Trim();
            law.Content = form.Content.Trim();
            law.Amendments.Clear();
            law.Amendments.AddRange(form.Amendments
                .OrderBy(a => a.AmendedOn)
                .Select(a => new LawAmendment { AmendedOn = a.AmendedOn!.Value.Date, Note = a.Note.Trim() }));
        }

        /// <summary>
        /// Rattache à la réunion les images présentes dans son compte rendu et supprime celles qui en ont été retirées
        /// </summary>
        private void SyncImages(ParliamentMeeting meeting)
        {
            HashSet<string> referenced = _markdownBL.GetReferencedImages(meeting.Content);

            List<ParliamentImage> removed = _repository.GetMeetingImages(meeting.Id).Where(i => !referenced.Contains(i.FileName)).ToList();
            foreach (ParliamentImage image in _repository.GetImages(referenced).Where(i => i.MeetingId == null))
            {
                image.MeetingId = meeting.Id;
            }
            _repository.Update();
            DeleteImages(removed);
        }

        private void DeleteImages(List<ParliamentImage> images)
        {
            if (images.Count == 0)
            {
                return;
            }
            _repository.DeleteImages(images);
            foreach (ParliamentImage image in images)
            {
                _imageStorage.Delete(image.FileName);
            }
        }

        [GeneratedRegex(@"^\d+")]
        private static partial Regex LawNumberDigits();
    }
}
