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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IAverisMobile
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "SignIn")]
        void CheckPhoneNumber(string phoneNumber);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "Verify")]
        UserInfo VerifyPhoneNumber(Verification info);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDeclarationDetails")]
        List<DeclarationDetailInfo> GetDeclarationDetails();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetLatestJobApplication")]
        JobApplicationInfo GetLatestJobApplication();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetJobApplicationStatus")]
        ShortJobApplicationInfo GetJobApplicationStatus();

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, UriTemplate = "SubmitJobApplication", BodyStyle = WebMessageBodyStyle.Bare)]
        void SubmitJobApplication(JobApplicationInfo info);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetJobApplication")]
        JobApplicationInfo GetJobApplication(string jobID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllJobApplications")]
        List<ShortJobApplicationInfo> GetAllJobApplications();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Unlock")]
        void UnlockJobApplication(string jobID);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Reject")]
        void RejectJobApplication(string jobID);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Accept")]
        void AcceptJobApplication(string jobID);
    }
}
