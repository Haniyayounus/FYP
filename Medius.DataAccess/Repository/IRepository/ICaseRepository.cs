using Medius.Model;
using Medius.Model.ViewModels;
using Medius.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface ICaseRepository : IRepositoryAsync<Case>
    {
        //get
        Task<List<Case>> GetAllTrademark();
        Task<List<Case>> GetAllCopyright();
        Task<List<Case>> GetAllDesign();
        Task<List<Case>> GetAllPatent();
        Task<List<Case>> GetUserTrademark(string userId);
        Task<List<Case>> GetUserCopyright(string userId);
        Task<List<Case>> GetUserDesign(string userId);
        Task<List<Case>> GetUserPatent(string userId);
        Task<List<Case>> GetAllUserCases(string userId);
        Task<Case> ChangeIPStatus(ChangeStatusViewModel viewModel);
        Task<Case> DeleteIP(int id);
        Task<Case> Add(CaseViewModel viewModel);
        Task<Case> Update(Case entity);
        Task<Case> GetCase(int id);
        Task<bool> IsCaseDuplicate(string title);
        Task<bool> IsCaseDuplicate(int id, string title);
        void sendCaseStatusEmail(Case entity);

        Task<Images> UploadedImage(CaseType ipType, IFormFile image);
        Task<Document> UploadFile(CaseType ipType, IFormFile file);
    }
}
