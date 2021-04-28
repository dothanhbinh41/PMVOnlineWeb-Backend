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

        Task<Department> CreateDepartmentAsync(Department request);
        Task<Department> UpdateDepartmentAsync(Department request);
        Task DeleteDepartmentAsync(int request);

        Task<bool> AddUserToDepartmentAsync(DepartmentUser request);
        Task<bool> AddUserToDeparmentAsync(DepartmentUser[] request);
        Task<bool> UpdateUserToDepartmentAsync(DepartmentUser request);
        Task<bool> UpdateUserToDepartmentsAsync(Guid uid, DepartmentUser[] request);
        Task<bool> DeleteUserToDepartmentAsync(DepartmentUser request);
        Task<bool> DeleteUserToDepartmentsAsync(DepartmentUser[] request);
        Task<Department[]> GetAllDepartmentAsync();
        Task<DepartmentUser[]> GetAllUserAsync(int departmentId);
        Task<DepartmentUser[]> GetAllUserInDepartmentAsync(int[] departmentId);
        Task<DepartmentUser[]> GetAllUserAsync(string department);
        Task<DepartmentUser[]> GetAllUserAsync();
        Task<DepartmentUser[]> GetUserDepartmentsAsync(Guid userId);
        Department GetDepartmentByName(string name);
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

        public async Task<bool> AddUserToDepartmentAsync(DepartmentUser request)
        {
            var us = await departmentUserRepository.FirstOrDefaultAsync(d => d.UserId == request.UserId && d.DepartmentId == request.DepartmentId);
            if (us != null)
            {
                return false;
            }

            await departmentUserRepository.InsertAsync(request);
            return true;
        }

        public async Task<bool> AddUserToDeparmentAsync(DepartmentUser[] request)
        {
            await departmentUserRepository.InsertManyAsync(request);
            return true;
        }

        public async Task<bool> DeleteUserToDepartmentAsync(DepartmentUser request)
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

        public async Task<DepartmentUser[]> GetAllUserInDepartmentAsync(int[] departmentId)
        {
            return (await departmentUserRepository.WithDetailsAsync(d => d.User, c => c.Department)).Where(c => departmentId.Contains(c.DepartmentId)).ToArray();
        }

        public Department GetDepartmentByName(string name)
        {
            return departmentRepository.FirstOrDefault(d => d.Name == name);
        }

        public async Task<DepartmentUser[]> GetUserDepartmentsAsync(Guid userId)
        {
            return (await departmentUserRepository.WithDetailsAsync(d => d.User, c => c.Department)).Where(c => c.UserId == userId).ToArray();
        }

        public async Task<bool> UpdateUserToDepartmentAsync(DepartmentUser request)
        {
            var us = await departmentUserRepository.FirstOrDefaultAsync(d => d.UserId == request.UserId && d.DepartmentId == request.DepartmentId);
            if (us != null)
            {
                us.IsLeader = request.IsLeader;
                await departmentUserRepository.UpdateAsync(us);
            }
            return true;
        }

        public Task<Department> CreateDepartmentAsync(Department request)
        {
            return departmentRepository.InsertAsync(request);
        }

        public async Task<Department> UpdateDepartmentAsync(Department request)
        {
            var dep = await departmentRepository.GetAsync(request.Id);
            dep.Name = request.Name;
            return await departmentRepository.UpdateAsync(dep);
        }

        public Task DeleteDepartmentAsync(int request)
        {
            return departmentRepository.DeleteAsync(request);
        }

        public async Task<bool> DeleteUserToDepartmentsAsync(DepartmentUser[] request)
        {
            var us = departmentUserRepository.Where(d => request.Any(c => c.UserId == d.UserId && c.DepartmentId == d.DepartmentId));
            if (us != null)
            {
                await departmentUserRepository.DeleteManyAsync(us);
            }
            return true;
        }

        public async Task<bool> UpdateUserToDepartmentsAsync(Guid uid, DepartmentUser[] request)
        {
            var deps = departmentUserRepository.Where(d => d.UserId == uid).ToList() ?? new List<DepartmentUser>();

            try
            {
                var removed = deps.Where(d => !request.Any(c => c.DepartmentId == d.DepartmentId)).ToArray();
                if (removed.Length > 0)
                    await departmentUserRepository.DeleteManyAsync(removed.Select(d => d.Id));
                var added = request.Where(d => !deps.Any(c => c.DepartmentId == d.DepartmentId)).ToArray();
                if (added.Length > 0)
                {
                    await departmentUserRepository.InsertManyAsync(added);
                }

                var updated = deps?
                    .Where(d => request.Any(c => c.DepartmentId == d.DepartmentId))?
                    .ToArray();
                for (int i = 0; i < updated.Length; i++)
                {
                    var u = updated[i];
                    u.IsLeader = request.FirstOrDefault(d => d.DepartmentId == u.DepartmentId)?.IsLeader ?? false;
                }
                if (updated?.Length > 0)
                {
                    await departmentUserRepository.UpdateManyAsync(updated);
                }
            }
            catch (Exception e)
            {

            }
            return true;
        }
    }
}
