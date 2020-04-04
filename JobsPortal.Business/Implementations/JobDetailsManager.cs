using JobsPortal.Business.Interfaces;
using JobsPortal.Business.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace JobsPortal.Business.Implementations
{
    public class JobDetailsManager : IJobDetailsManager
    {
        private readonly string connString;
        public JobDetailsManager(IConfiguration config)
        {
            connString = config.GetConnectionString("default");
        }
        public List<JobDetailsDto> GetByPageNum(int first, int last, JobDetailsDto filter)
        {
            //pageNum = (pageNum - 1) * 100;
            var jd = new List<JobDetailsDto>();
            try
            {
                using var conn = new MySqlConnection(connString);
                conn.Open();
                using var command = conn.CreateCommand();
                command.CommandText="SELECT JobId, JobTitle, JobUrl, PostedBy, PostedDate, AppliedBy, AppliedDate"+
                    " FROM autoapply_uipath.jobsdetailed WHERE 1=1 ";
                if (filter.Id != 0 && !sqlUnSafe(filter.Id.ToString()))
                    command.CommandText += $" && JobId LIKE '%{filter.Id}%'";
                if (!string.IsNullOrEmpty(filter.JobTitle) && !sqlUnSafe(filter.JobTitle))
                    command.CommandText += $" && JobTitle  LIKE '%{filter.JobTitle}%'";
                if (!string.IsNullOrEmpty(filter.JobUrl) && !sqlUnSafe(filter.JobUrl))
                    command.CommandText += $" && JobUrl  LIKE '%{filter.JobUrl}%'"; 
                if (!string.IsNullOrEmpty(filter.PostedBy) && !sqlUnSafe(filter.PostedBy))
                    command.CommandText += $" && PostedBy  LIKE '%{filter.PostedBy}%'";
                if (!string.IsNullOrEmpty(filter.AppliedBy) && !sqlUnSafe(filter.AppliedBy))
                    command.CommandText += $" && AppliedBy  LIKE '%{filter.AppliedBy}%'";


                command.CommandText += $" ORDER BY PostedDate desc, JobId desc, AppliedBy asc LIMIT @last OFFSET @first;";
                command.Parameters.AddWithValue("@last", last);
                command.Parameters.AddWithValue("@first", first);
                var rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    try
                    {
                        var job = new JobDetailsDto();
                        job.Id = rdr.GetInt32(0);
                        job.JobTitle = rdr.IsDBNull("jobtitle") ? "https://www.google.com" : rdr.GetString(1); ;
                        job.JobUrl = rdr.IsDBNull("joburl") ? "" : rdr.GetString(2);
                        job.PostedBy = rdr.IsDBNull("postedby") ? "" : rdr.GetString(3);
                        job.PostedDate = rdr.IsDBNull("posteddate") ? (DateTime?)null : rdr.GetDateTime(4);
                        job.AppliedBy = rdr.IsDBNull("appliedby") ? "" : rdr.GetString(5);
                        job.AppliedDate = rdr.IsDBNull("applieddate") ? (DateTime?)null : rdr.GetDateTime(6);
                        jd.Add(job);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error occured while processing your request. Please contact your supervisor", ex);
                    }
                };
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            return jd;
        }
        public int Count() 
        {
            var count = 0;
            try
            {
                using var conn = new MySqlConnection(connString);
                conn.Open();
                using var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(JobId) FROM autoapply_uipath.jobsdetailed";
                count = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while processing your request. Please contact your supervisor",ex);
            }
            return count;
        }
        public JobDetailsDto Update(JobDetailsDto jobDetails)
        {
            if (sqlUnSafe(jobDetails.AppliedBy) || sqlUnSafe(jobDetails.Id.ToString()))
                throw new Exception("Error occured while updating record. Please contact your supervisor");

            try
            {
                using var conn = new MySqlConnection(connString);
                conn.Open();
                using var command = conn.CreateCommand();
                command.CommandText = $"UPDATE autoapply_uipath.jobsdetailed SET AppliedBy ='{jobDetails.AppliedBy}', AppliedDate=now()  WHERE JobId = {jobDetails.Id}";
                var a =command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while updating record. Please contact your supervisor",ex);
            }
            return jobDetails;
        }

        private bool sqlUnSafe(string input)
        {
            bool isSQLInjection = false;
            string[] sqlCheckList = { "--",";--","'", ";","/*","*/","@@","@", "char","nchar","varchar","nvarchar","alter","begin","cast","create","cursor",
                "declare","delete","drop","end","exec","execute","fetch","insert","kill","select","sys","sysobjects","syscolumns","table","update"};

            foreach(var str in sqlCheckList)
            {
                if ((input.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0))
                    isSQLInjection = true;
            }
            return isSQLInjection;
        }
    }
}
