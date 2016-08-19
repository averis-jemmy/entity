using MyAverisData;
using MyAverisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MyAverisService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class AverisMobile : IAverisMobile
    {
        public void CheckPhoneNumber(string phoneNumber)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
            try
            {
                UserRepository userRep = new UserRepository(Database.Instant);
                if (!userRep.CheckRegisteredUsers(phoneNumber))
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Unknown Number");
                }
                else
                {
                    if (userRep.HasRequestedVerificationCode(phoneNumber))
                    {
                        response.Headers.Add("ErrorMessage", "Has Requested Before");
                        response.Headers.Add("ErrorDescription", "Please wait for 1 minute before retrying.");
                    }
                    else
                    {
                        userRep.SendSMSVerificationCode(phoneNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }
        }

        public UserInfo VerifyPhoneNumber(Verification info)
        {
            UserRepository userRep = new UserRepository(Database.Instant);
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                UserInfo user = userRep.IsVerified(info);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Wrong Verification Code");
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }

            return null;
        }

        public List<DeclarationDetailInfo> GetDeclarationDetails()
        {
            DeclarationDetailRepository ddRep = new DeclarationDetailRepository(Database.Instant);
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                var details = ddRep.GetDeclarationDetails();
                return details;
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }

            return null;
        }

        public JobApplicationInfo GetLatestJobApplication()
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if(!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return null;
                }

                Guid id = Guid.Parse(request.Headers["UserID"]);
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
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }

            return null;
        }

        public ShortJobApplicationInfo GetJobApplicationStatus()
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if (!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return null;
                }

                Guid id = Guid.Parse(request.Headers["UserID"]);
                var jobApp = jobAppRep.GetJobApplicationStatusByUserID(id);
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
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }

            return null;
        }

        public void SubmitJobApplication(JobApplicationInfo info)
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if (!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return;
                }

                info.UserID = Guid.Parse(request.Headers["UserID"]);
                var status = jobAppRep.SaveJobApplication(info);
                if (!string.IsNullOrEmpty(status))
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", status);
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }
        }

        public JobApplicationInfo GetJobApplication(string jobID)
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if (!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return null;
                }

                if (!ValidRecruiter(request.Headers["UserID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return null;
                }

                Guid id = Guid.Parse(jobID);
                var jobApp = jobAppRep.GetJobApplicationByID(id);
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
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }

            return null;
        }

        public List<ShortJobApplicationInfo> GetAllJobApplications()
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if (!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return null;
                }

                if (!ValidRecruiter(request.Headers["UserID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return null;
                }

                var jobApps = jobAppRep.GetAllJobApplications();
                if (jobApps == null)
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Job application form does not exist");
                }
                else
                {
                    return jobApps;
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }

            return null;
        }

        public void UnlockJobApplication(string jobID)
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if (!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return;
                }

                if (!ValidRecruiter(request.Headers["UserID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return;
                }

                Guid id = Guid.Parse(jobID);
                Guid userID = Guid.Parse(request.Headers["UserID"]);
                var result = jobAppRep.UnlockJobApplication(id, userID);
                if (!result)
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Job application form does not exist");
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }
        }

        public void RejectJobApplication(string jobID)
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if (!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return;
                }

                if (!ValidRecruiter(request.Headers["UserID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return;
                }

                Guid id = Guid.Parse(jobID);
                Guid userID = Guid.Parse(request.Headers["UserID"]);
                var result = jobAppRep.RejectJobApplication(id, userID);
                if (!result)
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Job application form does not exist");
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }
        }

        public void AcceptJobApplication(string jobID)
        {
            JobApplicationRepository jobAppRep = new JobApplicationRepository(Database.Instant);
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if (!ValidToken(request.Headers["UserID"], request.Headers["TokenID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return;
                }

                if (!ValidRecruiter(request.Headers["UserID"]))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.StatusDescription = "It is forbidden";
                    return;
                }

                Guid id = Guid.Parse(jobID);
                Guid userID = Guid.Parse(request.Headers["UserID"]);
                var result = jobAppRep.AcceptJobApplication(id, userID);
                if (!result)
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Job application form does not exist");
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }
        }

        private bool ValidToken(string userID, string tokenID)
        {
            try
            {
                if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(tokenID))
                {
                    return false;
                }
                else
                {
                    Guid id = Guid.Parse(userID);
                    Guid tknID = Guid.Parse(tokenID);
                    UserRepository userRep = new UserRepository(Database.Instant);
                    if (!userRep.ValidToken(id, tknID))
                    {
                        return false;
                    }
                }
            }
            catch { return false; }

            return true;
        }

        private bool ValidRecruiter(string userID)
        {
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    return false;
                }
                else
                {
                    Guid id = Guid.Parse(userID);
                    UserRepository userRep = new UserRepository(Database.Instant);
                    if (!userRep.ValidRecruiter(id))
                    {
                        return false;
                    }
                }
            }
            catch { return false; }

            return true;
        }
    }
}
