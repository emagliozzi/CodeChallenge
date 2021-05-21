using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public ReportingStructure GetReportingStructureById(string id)
        {
            Employee employee = GetById(id);
            //if id is real employee return the ReportStructure
            if(employee != null)
            {
                return new ReportingStructure
                {
                    Employee = employee,
                    NumberOfReports = GetReportCount(employee)
                };
            }
            return null;
        }

        public int GetReportCount(Employee employee)
        {
            int reportCount = 0;
            if(employee.DirectReports != null)
            {
                //Add a report for the first employee, then add report for every employee's directreport
                foreach(Employee reportee in employee.DirectReports)
                {
                    reportCount++;
                    reportCount += GetReportCount(reportee);
                }
            }
            return reportCount;
        }
        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
    }
}
