using PMVOnline.Files;
using PMVOnline.Targets;
using PMVOnline.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace PMVOnline.Reports
{
    public interface IReportAppService
    {
        Task<PagedResultDto<ReportDto>> GetReportAsync(ReportRequestdto request);
    }

    public class ReportDto : EntityDto<long>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreationTime { get; set; }
        public Priority Priority { get; set; }
        public int TargetId { get; set; }
        public Status Status { get; set; }
        public SimpleUserDto Assignee { get; set; }
        public SimpleUserDto Creator { get; set; }
        //public SimpleUserDto[] TaskFollows { get; set; }
        public TaskCommentDto[] TaskComments { get; set; }
        public TaskDto[] ReferenceTasks { get; set; }
        public TaskRatingDto[] TaskRatings { get; set; }
        public TaskActionDto[] TaskHistory { get; set; }
        public TargetDto Target { get; set; }
        public TaskFileDto[] TaskFiles { get; set; }
    }

    public class ReportRequestdto : PagedResultRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DepartmentId { get; set; }
    }
}