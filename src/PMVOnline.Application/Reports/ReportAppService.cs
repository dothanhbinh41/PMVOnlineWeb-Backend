using PMVOnline.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PMVOnline.Reports
{
    public class ReportAppService : ApplicationService, IReportAppService
    {
        private readonly IRepository<Tasks.Task, long> taskRepository;

        public ReportAppService(IRepository<Tasks.Task, long> taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public async Task<PagedResultDto<ReportDto>> GetReportAsync(ReportRequestdto request)
        {
            var result = await taskRepository.WithDetailsAsync(d => d.Creator, d => d.Assignee, d => d.TaskHistory, d => d.TaskComments, d => d.TaskFollows, d => d.Target, d => d.ReferenceTasks, d => d.TaskRatings, d => d.TaskFiles);
            var tasks = result
                .WhereIf(request.StartDate.HasValue, d => d.CreationTime >= request.StartDate.Value)
                .WhereIf(request.EndDate.HasValue, d => d.CreationTime <= request.EndDate.Value)
                .PageBy(request).ToList();
            var report = ObjectMapper.Map<List<Tasks.Task>, List<ReportDto>>(tasks); 
            return new PagedResultDto<ReportDto>(tasks.Count, ObjectMapper.Map<List<Tasks.Task>, List<ReportDto>>(tasks));
        }
    }
}