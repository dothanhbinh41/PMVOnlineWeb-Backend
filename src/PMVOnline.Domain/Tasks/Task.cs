﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Tasks
{
    public class Task : FullAuditedAggregateRoot<ulong>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Priority Priority { get; set; }
        public  Target Target { get; set; }
        public Status Status { get; set; }
        public Guid Assignee { get; set; }

        public virtual TaskFile[] TaskFiles { get; set; }
        public virtual ReferenceTask[] ReferenceTasks { get; set; }
        public virtual TaskHistory[] TaskHistory { get; set; }
    } 
}