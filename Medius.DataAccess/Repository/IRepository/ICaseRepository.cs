using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModels;
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
        Task<Case> ChangeIPStatus(ChangeStatusViewModel vm);


        Task<Case> AddAsync(Case entity);
        Task<Case> Update(Case entity);
        Task<Case> GetCase(int id);
        Task<bool> IsCaseDuplicate(string title);
        Task<bool> IsCaseDuplicate(int id, string title);
        void sendCaseStatusEmail(Case entity);
    }
}
