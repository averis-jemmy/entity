using ESuntikanCommon;
using ESuntikanModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Xml;
using System.Configuration;
using System.IO;

namespace ESuntikanAppData
{
    public class VaccinationRepository : RepositoryBase<Vaccination>
    {
        public VaccinationRepository(ESuntikanLocalDB context) : base(context) { }

        public string GetRunningNumber(string code)
        {
            var arNumber = string.Empty;
            long count = GetSerialRunningNumber(code);
            arNumber = string.Format("{0}{1}", code, count.ToString().PadLeft(5, '0'));

            return arNumber;
        }

        private int GetSerialRunningNumber(string term)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + ConfigurationManager.AppSettings["LogFile"]);
                XmlElement elem = xDoc.DocumentElement;
                XmlNode nodeLastRecordedDate = elem.ChildNodes[0];
                XmlNode nodeLastSerialNumber = elem.ChildNodes[1];
                if (DateTime.Now.Date == DateTime.ParseExact(nodeLastRecordedDate.InnerText, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))
                {
                    int serial = int.Parse(nodeLastSerialNumber.InnerText) + 1;
                    return serial;
                }
                else
                {
                    return 1;
                }
            }
            catch { return 0; }

            //int runningNumber = 1;

            //var q = from item in context.Vaccinations
            //        where item.SerialNo.StartsWith(term)
            //        && item.IsDeleted == false
            //        select item.SerialNo.Substring(term.Length);

            //if (q.Count() > 0)
            //{
            //    List<string> nums = q.ToList();

            //    for (int i = 0; i < nums.Count; i++)
            //    {
            //        nums[i] = nums[i].PadLeft(5, '0');
            //    }

            //    int maxCurrent = Convert.ToInt32(nums.Max());
            //    runningNumber = maxCurrent + 1;
            //}

            //return runningNumber;
        }

        public string AfterVaccination(VaccinationViewModel model, UserInfo user, string serialNo)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + ConfigurationManager.AppSettings["LogFile"]);
                XmlElement elem = xDoc.DocumentElement;
                XmlNode nodeLastRecordedDate = elem.ChildNodes[0];
                nodeLastRecordedDate.InnerText = DateTime.Now.ToString("yyyy-MM-dd");
                XmlNode nodeLastSerialNumber = elem.ChildNodes[1];
                nodeLastSerialNumber.InnerText = serialNo.Substring(7);
                xDoc.Save(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + ConfigurationManager.AppSettings["LogFile"]);
            }
            catch { }

            try
            {
                var vac = GetSingle(p => p.ID == model.ID);
                if (vac != null)
                {
                    vac.SerialNo = serialNo;
                    vac.AdministeredBy = user.UserName;
                    vac.VaccinationStatus = Status.Printing;
                    vac.VaccinationDate = DateTime.Now;
                    vac.Remarks = model.Remarks;
                    vac.LastChangedBy = user.UserName;
                    vac.LastChanged = DateTime.Now;
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }

        public string CancelVaccination(VaccinationViewModel model, UserInfo user)
        {
            try
            {
                var vac = GetSingle(p => p.ID == model.ID);
                if (vac != null)
                {
                    vac.VaccinationStatus = Status.Cancelled;
                    vac.Remarks = model.Remarks;
                    vac.LastChangedBy = user.UserName;
                    vac.LastChanged = DateTime.Now;
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }

        public List<VaccinationViewModel> GetAllVaccinations()
        {
            try
            {
                var vaccinations = (from vaccination in GetAll()
                                    join administeredBy in context.UserMasters on vaccination.AdministeredBy equals administeredBy.UserName into pp1
                                    from administeredBy in pp1.DefaultIfEmpty()
                                    join registeredBy in context.UserMasters on vaccination.RegisteredBy equals registeredBy.UserName into pp2
                                    from registeredBy in pp2.DefaultIfEmpty()
                                    orderby vaccination.RegisteredDate descending
                                    select new VaccinationViewModel
                                    {
                                        ID = vaccination.ID,
                                        TicketNo = vaccination.TicketNo,
                                        SerialNo = vaccination.SerialNo,
                                        AdministeredBy = administeredBy.FullName,
                                        RegisteredBy = registeredBy.FullName,
                                        RegisteredDate = vaccination.RegisteredDate,
                                        VaccinationDate = vaccination.VaccinationDate,
                                        VaccinationStatus = vaccination.VaccinationStatus,
                                        VaccineName = vaccination.Vaccine.Name,
                                        VisitorName = vaccination.Visitor.Name,
                                        ICNo = vaccination.Visitor.ICNo
                                    });

                return vaccinations.ToList();
            }
            catch
            {
                return new List<VaccinationViewModel>();
            }
        }

        public List<VaccinationViewModel> GetAllVaccinations(int page, int top)
        {
            try
            {
                var vaccinations = (from vaccination in GetAll()
                                    join administeredBy in context.UserMasters on vaccination.AdministeredBy equals administeredBy.UserName into pp1
                                    from administeredBy in pp1.DefaultIfEmpty()
                                    join registeredBy in context.UserMasters on vaccination.RegisteredBy equals registeredBy.UserName into pp2
                                    from registeredBy in pp2.DefaultIfEmpty()
                                    where (vaccination.IsDeleted == false)
                                    orderby vaccination.RegisteredDate descending
                                    select new VaccinationViewModel
                                    {
                                        ID = vaccination.ID,
                                        TicketNo = vaccination.TicketNo,
                                        SerialNo = vaccination.SerialNo,
                                        AdministeredBy = administeredBy.FullName,
                                        RegisteredBy = registeredBy.FullName,
                                        RegisteredDate = vaccination.RegisteredDate,
                                        VaccinationDate = vaccination.VaccinationDate,
                                        VaccinationStatus = vaccination.VaccinationStatus,
                                        VaccineName = vaccination.Vaccine.Name,
                                        VisitorName = vaccination.Visitor.Name,
                                        ICNo = vaccination.Visitor.ICNo
                                    }).Skip(page * top).Take(top);

                return vaccinations.ToList();
            }
            catch
            {
                return new List<VaccinationViewModel>();
            }
        }

        public int GetVaccinationCount()
        {
            try
            {
                var vaccinationCount = (from vaccination in GetAll()
                                        join administeredBy in context.UserMasters on vaccination.AdministeredBy equals administeredBy.UserName into pp1
                                        from administeredBy in pp1.DefaultIfEmpty()
                                        join registeredBy in context.UserMasters on vaccination.RegisteredBy equals registeredBy.UserName into pp2
                                        from registeredBy in pp2.DefaultIfEmpty()
                                        where (vaccination.IsDeleted == false)
                                        orderby vaccination.RegisteredDate descending
                                        select new VaccinationViewModel
                                        {
                                            ID = vaccination.ID,
                                            TicketNo = vaccination.TicketNo,
                                            SerialNo = vaccination.SerialNo,
                                            AdministeredBy = administeredBy.FullName,
                                            RegisteredBy = registeredBy.FullName,
                                            RegisteredDate = vaccination.RegisteredDate,
                                            VaccinationDate = vaccination.VaccinationDate,
                                            VaccinationStatus = vaccination.VaccinationStatus,
                                            VaccineName = vaccination.Vaccine.Name,
                                            VisitorName = vaccination.Visitor.Name,
                                            ICNo = vaccination.Visitor.ICNo
                                        }).Count();

                return vaccinationCount;
            }
            catch { return 0; }
        }

        public VaccinationViewModel GetVaccinationByID(Guid vacID)
        {
            try
            {
                var vac = (from vaccination in GetAll()
                           join administeredBy in context.UserMasters on vaccination.AdministeredBy equals administeredBy.UserName into pp1
                           from administeredBy in pp1.DefaultIfEmpty()
                           join registeredBy in context.UserMasters on vaccination.RegisteredBy equals registeredBy.UserName into pp2
                           from registeredBy in pp2.DefaultIfEmpty()
                           where vaccination.ID == vacID
                           select new VaccinationViewModel
                           {
                               ID = vaccination.ID,
                               TicketNo = vaccination.TicketNo,
                               SerialNo = vaccination.SerialNo,
                               AdministeredBy = administeredBy.FullName,
                               RegisteredBy = registeredBy.FullName,
                               RegisteredDate = vaccination.RegisteredDate,
                               VaccinationDate = vaccination.VaccinationDate,
                               VaccinationStatus = vaccination.VaccinationStatus,
                               Remarks = vaccination.Remarks,
                               VaccineName = vaccination.Vaccine.Name,
                               VisitorName = vaccination.Visitor.Name,
                               ICNo = vaccination.Visitor.ICNo,
                               Photo = vaccination.Visitor.Photo
                           }).FirstOrDefault();

                return vac;
            }
            catch
            {
                return null;
            }
        }
    }
}
