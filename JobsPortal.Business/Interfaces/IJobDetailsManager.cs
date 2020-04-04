using JobsPortal.Business.Models;
using System.Collections.Generic;

namespace JobsPortal.Business.Interfaces
{
    public interface IJobDetailsManager
    {
        List<JobDetailsDto> GetByPageNum(int first, int last, JobDetailsDto details);
        int Count();
        JobDetailsDto Update(JobDetailsDto jobDetails);
    }
}
