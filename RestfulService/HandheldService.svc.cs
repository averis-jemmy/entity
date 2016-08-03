using MobilityMiddlewareData;
using MobilityMiddlewareEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MobilityMiddlewareService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class HandheldService : IHandheldService
    {
        public void UploadNotice(NoticeInfo info)
        {
            NoticeMaintenanceRepository notRep = new NoticeMaintenanceRepository(Database.Instant);
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                if(!notRep.SaveNotice(info))
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Upload Failed");
                }
            }
            catch (Exception ex) {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }
        }

        public DeviceInfo RegisterDevice(string serialNumber)
        {
            HandheldMaintenanceRepository handheldRep = new HandheldMaintenanceRepository(Database.Instant);
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

            try
            {
                var info = handheldRep.RegisterDevice(serialNumber);
                if(info == null)
                {
                    response.Headers.Add("ErrorMessage", "Error");
                    response.Headers.Add("ErrorDescription", "Register Failed");
                }
                return info;
            }
            catch (Exception ex)
            {
                response.Headers.Add("ErrorMessage", "Error");
                response.Headers.Add("ErrorDescription", ex.Message);
            }

            return null;
        }

        public LookupTableInfo DownloadLookupTable(string lastUpdatedDate)
        {
            try
            {
                //OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                //response.Headers.Add("ErrorMessage", "Error");
                //response.Headers.Add("ErrorDescription", "Wrong Verification Code");
            }
            catch
            { }

            return null;
        }
    }
}
