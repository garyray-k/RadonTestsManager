using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RadonTestsManager.Models;
using Bogus;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace RTM.Server.Utility {
    public class DevDataFaker {


        public Job[] GenerateJobs() {
            var jobNumbers = new Random().Next(1000, 2000);
            var deviceTypes = new string[] { "LS Vial", "CRM", "Unknown" };
            var timesOfDay = new string[] { "AM", "PM" };
            var jobs = new Faker<Job>()
                .StrictMode(false)
                .RuleFor(j => j.JobNumber, f => jobNumbers++)
                .RuleFor(j => j.ServiceType, f => f.Lorem.Word())
                .RuleFor(j => j.ServiceDate, f => f.Date.Future())
                .RuleFor(j => j.ServiceDeadLine, (f, u) => u.ServiceDate.AddDays(7))
                .RuleFor(j => j.DeviceType, f => f.PickRandom(deviceTypes))
                .RuleFor(j => j.AccessInfo, f => f.Lorem.Sentence())
                .RuleFor(j => j.SpecialNotes, f => f.Lorem.Sentences(3, " "))
                .RuleFor(j => j.Driver, f => f.Name.FirstName())
                .RuleFor(j => j.TimeOfDay, f => f.PickRandom(timesOfDay))
                .RuleFor(j => j.ArrivalTime, f => f.Date.Future().ToLocalTime())
                .RuleFor(j => j.Confirmed, f => f.Random.Bool())
                .RuleFor(j => j.Completed, f => f.Random.Bool())
                .RuleFor(j => j.LastUpdatedBy, f => f.Internet.Email());
            return jobs.Generate(10).ToArray();
        }

        public Address[] GenerateAddresses(Job[] jobsForHistory) {
            var addresses = new Faker<Address>()
                .StrictMode(false)
                .RuleFor(a => a.CustomerName, f => f.Name.FullName())
                .RuleFor(a => a.Address1, f => f.Address.StreetAddress())
                .RuleFor(a => a.Address2, f => f.Address.SecondaryAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.Country, f => f.Address.Country())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.JobHistory, f => f.PickRandom(jobsForHistory, 5).ToList());
            return addresses.Generate(5).ToArray();
        }

        public ContinuousRadonMonitor[] GenerateCRMs(Job[] jobsForHistory) {
            var mxLogEntries = new Faker<CRMMaintenanceLogEntry>()
                .RuleFor(ml => ml.EntryDate, f => f.Date.Recent())
                .RuleFor(ml => ml.MaintenanceDescription, f => f.Hacker.Phrase())
                .RuleFor(ml => ml.ActionsTaken, f => f.Hacker.Verb())
                .RuleFor(ml => ml.LastUpdatedBy, f => f.Internet.Email());

            var crmNumber = 0;
            var crms = new Faker<ContinuousRadonMonitor>()
                .RuleFor(c => c.MonitorNumber, f => crmNumber++)
                .RuleFor(c => c.SerialNumber, f => f.Random.Number(42487, 99999))
                .RuleFor(c => c.LastCalibrationDate, f => f.Date.Past())
                .RuleFor(c => c.PurchaseDate, f => f.Date.Past(2))
                .RuleFor(c => c.LastBatteryChangeDate, f => f.Date.Past(1))
                .RuleFor(c => c.TestStart, f => f.Date.Soon())
                .RuleFor(c => c.TestFinish, (f, u) => u.TestStart.AddDays(2))
                .RuleFor(c => c.Status, f => f.Lorem.Sentence())
                .RuleFor(c => c.LastUpdatedBy, f => f.Internet.Email())
                .RuleFor(c => c.JobHistory, f => f.PickRandom(jobsForHistory, 5).ToList())
                .RuleFor(c => c.MaintenanceLogHistory, f => mxLogEntries.Generate(3).ToList());

            return crms.Generate(20).ToArray();
        }

        public LSVial[] GenerateLSVials(Job[] jobsForHistory) {
            var lSVials = new Faker<LSVial>()
                .RuleFor(v => v.SerialNumber, f => f.Random.Number(4000, 5000))
                .RuleFor(v => v.Status, f => f.Lorem.Sentence())
                .RuleFor(v => v.TestStart, f => f.Date.Recent())
                .RuleFor(v => v.TestFinish, (f, u) => u.TestStart.AddDays(2))
                .RuleFor(v => v.LastUpdatedBy, f => f.Internet.Email());
            return lSVials.Generate(5).ToArray();
       }
      
    }
}
