using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace PMVOnline.Departments
{
    public interface IDepartmentManager
    {
        Task<bool> AddUserToDeparmentAsync(DepartmentUser request);
        Task<bool> UpdateUserToDeparmentAsync(DepartmentUser request);
        Task<bool> DeleteUserToDeparmentAsync(DepartmentUser request);
        Task<Department[]> GetAllDepartmentAsync();
        Task<DepartmentUser[]> GetAllUserAsync(int departmentId);
        Task<DepartmentUser[]> GetAllUserAsync(string department);
        Task<DepartmentUser[]> GetAllUserAsync();
        Task<DepartmentUser> GetUserDepartmentAsync(Guid userId);
    }

    public class DepartmentManager : IDomainService, IDepartmentManager
    {
        readonly IRepository<Department, int> departmentRepository;
        readonly IRepository<DepartmentUser, long> departmentUserRepository;

        public DepartmentManager(IRepository<Department, int> departmentRepository,
           IRepository<DepartmentUser, long> departmentUserRepository)
        {
            this.departmentRepository = departmentRepository;
            this.departmentUserRepository = departmentUserRepository;
        }

        public async Task<bool> AddUserToDeparmentAsync(DepartmentUser request)
        {
            var us = await departmentUserRepository.FirstOrDefaultAsync(d => d.UserId == request.UserId && d.DepartmentId == request.DepartmentId);
            if (us != null)
            {
                return false;
            }

            await departmentUserRepository.InsertAsync(request);
            return true;
        }

        public async Task<bool> DeleteUserToDeparmentAsync(DepartmentUser request)
        {
            var us = await departmentUserRepository.FirstOrDefaultAsync(d => d.UserId == request.UserId && d.DepartmentId == request.DepartmentId);
            if (us != null)
            {
                await departmentUserRepository.DeleteAsync(us);
            }
            return true;
        }

        public async Task<Department[]> GetAllDepartmentAsync()
        {
            return (await departmentRepository.GetListAsync()).ToArray();
        }

        public async Task<DepartmentUser[]> GetAllUserAsync(int departmentId)
        {
            return (await departmentUserRepository.WithDetailsAsync(d => d.User, c => c.Department)).Where(c => c.DepartmentId == departmentId).ToArray();
        }

        public async Task<DepartmentUser[]> GetAllUserAsync(string department)
        {
            return (await departmentUserRepository.WithDetailsAsync(d => d.User, c => c.Department)).Where(c => c.Department.Name == department).ToArray();
        }

        public async Task<DepartmentUser[]> GetAllUserAsync()
        {
            return (await departmentUserRepository.WithDetailsAsync(d => d.User, c => c.Department)).ToArray();
        }

        public async Task<DepartmentUser> GetUserDepartmentAsync(Guid userId)
        {
            return (await departmentUserRepository.WithDetailsAsync(d => d.User, c => c.Department)).FirstOrDefault(c => c.UserId == userId);
        }

        public async Task<bool> UpdateUserToDeparmentAsync(DepartmentUser request)
        {
            var us = await departmentUserRepository.FirstOrDefaultAsync(d => d.UserId == request.UserId && d.DepartmentId == request.DepartmentId);
            if (us != null)
            {
                us.IsLeader = request.IsLeader;
                await departmentUserRepository.UpdateAsync(us);
            }
            return true;
        }
    }
}
