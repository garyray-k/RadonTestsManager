using System.Collections.Generic;
using AutoMapper;
using RadonTestsManager.DTOs;
using RadonTestsManager.Models;

namespace RTM.Server.Utility {
    public class JobHistoryToDTOConverter : ITypeConverter<List<Job>, List<int>> {
    
        List<int> ITypeConverter<List<Job>, List<int>>.Convert(List<Job> jobs, List<int> destination, ResolutionContext context) {
            var finalList = new List<int> { };
            foreach (var job in jobs) {
                finalList.Add(job.JobNumber);
            }
            return finalList;
        }
    }
}
