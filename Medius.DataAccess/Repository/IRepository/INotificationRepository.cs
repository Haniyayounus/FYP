using Medius.Model;
using Medius.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task<Notification> AddAsync(Notification entity);
        Task<List<Notification>> GetAll();
        Task<Notification> GetById(int id);
        Task<List<Notification>> GetNotificationForUser();
    }
}
