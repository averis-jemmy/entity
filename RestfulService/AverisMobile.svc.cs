using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TestData;
using WebServiceEntity;

namespace WebServiceTest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class AverisMobile : IAverisMobile
    {
        public void CheckPhoneNumber(string phoneNumber)
        {
            UserRepository userRep = new UserRepository(Database.Instant);
            if (!userRep.CheckRegisteredUsers(phoneNumber))
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", "Unknown Number");
            }
            else
            {
                if (userRep.HasRequestedVerificationCode(phoneNumber))
                {
                    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                    response.Headers.Add("ErrorMessage", "Has Requested Before");
                    response.Headers.Add("ErrorDescription", "Please wait for 1 minute before retrying.");
                }
                else
                {
                    userRep.SendVerificationCode(phoneNumber);
                }
            }
        }

        public UserInfo VerifyPhoneNumber(Verification info)
        {
            UserRepository userRep = new UserRepository(Database.Instant);

            try
            {
                UserInfo user = userRep.IsVerified(info);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Wrong Verification Code");
                }
            }
            catch
            { }

            return null;
        }

        public List<DeclarationDetailInfo> GetDeclarationDetails()
        {
            DeclarationDetailRepository ddRep = new DeclarationDetailRepository(Database.Instant);

            try
            {
                var details = ddRep.GetDeclarationDetails();
                return details;
            }
            catch { }

            return null;
        }

        public JobApplicationInfo GetLatestJobApplication(string userID)
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);

            try
            {
                Guid id = Guid.Parse(userID);
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                var jobApp = jobAppRep.GetJobApplicationByUserID(id);
                if (jobApp == null)
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Job application form does not exist");
                }
                else
                {
                    return jobApp;
                }
            }
            catch { }
            return null;
        }

        public void SubmitJobApplication(JobApplicationInfo info)
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);

            try
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                var status = jobAppRep.SaveJobApplication(info);
                if (!string.IsNullOrEmpty(status))
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", status);
                }
            }
            catch { }
        }
    }
}
