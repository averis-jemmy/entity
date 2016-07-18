using ESuntikanCommon;
using ESuntikanModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;

namespace ESuntikanAppData
{
    public class VisitorRepository : RepositoryBase<Visitor>
    {
        public VisitorRepository(ESuntikanLocalDB context) : base(context) { }

        public string Update(VisitorViewModel model, UserInfo user)
        {
            try
            {
                if (model.ID.HasValue)
                {
                    var visitor = GetSingle(item => item.ID == model.ID);
                    if (visitor != null)
                    {
                        visitor.Name = model.Name;
                        visitor.ICNo = model.ICNo;
                        visitor.CountryID = model.CountryID;
                        visitor.RaceID = model.RaceID;
                        visitor.Sex = model.Sex;
                        visitor.Age = model.Age;
                        visitor.SalaryNo = model.SalaryNo;
                        visitor.Pregnant = model.Pregnant;
                        visitor.Allergy = model.Allergy;
                        visitor.Remarks = model.Remarks;
                        visitor.KPStreet = model.KPStreet;
                        visitor.KPArea = model.KPArea;
                        visitor.KPCity = model.KPCity;
                        visitor.KPState = model.KPState;
                        visitor.KPPostCode = model.KPPostCode;
                        visitor.ResidentStreet = model.ResidentStreet;
                        visitor.ResidentArea = model.ResidentArea;
                        visitor.ResidentCity = model.ResidentCity;
                        visitor.ResidentState = model.ResidentState;
                        visitor.ResidentPostCode = model.ResidentPostCode;
                        visitor.BusinessTypeID = model.BusinessTypeID;
                        visitor.BusinessName = model.BusinessName;
                        visitor.CompanyRegNo = model.CompanyRegNo;
                        visitor.BusinessStreet = model.BusinessStreet;
                        visitor.BusinessArea = model.BusinessArea;
                        visitor.BusinessCity = model.BusinessCity;
                        visitor.BusinessState = model.BusinessState;
                        visitor.BusinessPostCode = model.BusinessPostCode;
                        visitor.ReceiptNo = model.ReceiptNo;
                        visitor.Photo = model.Photo;
                        visitor.LastChanged = DateTime.Now;
                        visitor.LastChangedBy = user.UserName;
                    }
                    else
                    {
                        context.Visitors.Add(new Visitor
                        {
                            ID = model.ID.Value,
                            Name = model.Name,
                            ICNo = model.ICNo,
                            CountryID = model.CountryID,
                            RaceID = model.RaceID,
                            Sex = model.Sex,
                            Age = model.Age,
                            SalaryNo = model.SalaryNo,
                            Pregnant = model.Pregnant,
                            Allergy = model.Allergy,
                            Remarks = model.Remarks,
                            KPStreet = model.KPStreet,
                            KPArea = model.KPArea,
                            KPCity = model.KPCity,
                            KPState = model.KPState,
                            KPPostCode = model.KPPostCode,
                            ResidentStreet = model.ResidentStreet,
                            ResidentArea = model.ResidentArea,
                            ResidentCity = model.ResidentCity,
                            ResidentState = model.ResidentState,
                            ResidentPostCode = model.ResidentPostCode,
                            BusinessTypeID = model.BusinessTypeID,
                            BusinessName = model.BusinessName,
                            CompanyRegNo = model.CompanyRegNo,
                            BusinessStreet = model.BusinessStreet,
                            BusinessArea = model.BusinessArea,
                            BusinessCity = model.BusinessCity,
                            BusinessState = model.BusinessState,
                            BusinessPostCode = model.BusinessPostCode,
                            ReceiptNo = model.ReceiptNo,
                            PrintTrial = 0,
                            Photo = model.Photo,
                            IsDeleted = false,
                            LastChanged = DateTime.Now,
                            LastChangedBy = user.UserName
                        });

                        context.Vaccinations.Add(new Vaccination()
                        {
                            ID = Guid.NewGuid(),
                            VaccineID = model.VaccineID,
                            VisitorID = model.ID.Value,
                            TicketNo = model.TicketNo,
                            IsDeleted = false,
                            VaccinationStatus = Status.New,
                            RegisteredDate = DateTime.Now,
                            RegisteredBy = user.UserName,
                            LastChanged = DateTime.Now,
                            LastChangedBy = user.UserName
                        });
                    }

                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }

        public List<VisitorViewModel> GetAllVisitors()
        {
            try
            {
                var visitors = (from visitor in GetAll()
                                join vaccination in context.Vaccinations on visitor.ID equals vaccination.VisitorID
                                join vaccine in context.Vaccines on vaccination.VaccineID equals vaccine.ID
                                orderby vaccination.RegisteredDate descending
                                select new VisitorViewModel
                                {
                                    ID = visitor.ID,
                                    Name = visitor.Name,
                                    ICNo = visitor.ICNo,
                                    Sex = visitor.Sex,
                                    Age = visitor.Age,
                                    RaceName = visitor.Race.Name,
                                    VaccineName = vaccine.Name,
                                    BusinessTypeName = visitor.BusinessType.Name,
                                    CountryName = visitor.Country.Name,
                                    VaccinationStatus = vaccination.VaccinationStatus,
                                    VaccinationDate = vaccination.VaccinationDate,
                                    ReceiptNo = visitor.ReceiptNo
                                });

                return visitors.ToList();
            }
            catch
            {
                return new List<VisitorViewModel>();
            }
        }

        public List<VisitorViewModel> GetAllVisitors(int page, int top)
        {
            try
            {
                var visitors = (from visitor in GetAll()
                                join vaccination in context.Vaccinations on visitor.ID equals vaccination.VisitorID
                                join vaccine in context.Vaccines on vaccination.VaccineID equals vaccine.ID
                                where (visitor.IsDeleted == false)
                                orderby vaccination.RegisteredDate descending
                                select new VisitorViewModel
                                {
                                    ID = visitor.ID,
                                    Name = visitor.Name,
                                    ICNo = visitor.ICNo,
                                    Sex = visitor.Sex,
                                    Age = visitor.Age,
                                    RaceName = visitor.Race.Name,
                                    VaccineName = vaccine.Name,
                                    BusinessTypeName = visitor.BusinessType.Name,
                                    CountryName = visitor.Country.Name,
                                    VaccinationStatus = vaccination.VaccinationStatus,
                                    VaccinationDate = vaccination.VaccinationDate,
                                    ReceiptNo = visitor.ReceiptNo
                                }).Skip(page * top).Take(top);

                return visitors.ToList();
            }
            catch
            {
                return new List<VisitorViewModel>();
            }
        }

        public int GetVisitorCount()
        {
            try
            {
                var visitorCount = (from visitor in GetAll()
                                    join vaccination in context.Vaccinations on visitor.ID equals vaccination.VisitorID
                                    join vaccine in context.Vaccines on vaccination.VaccineID equals vaccine.ID
                                    join registeredBy in context.UserMasters on vaccination.RegisteredBy equals registeredBy.UserName into pp1
                                    from registeredBy in pp1.DefaultIfEmpty()
                                    where (visitor.IsDeleted == false)
                                    orderby vaccination.RegisteredDate descending
                                    select new VisitorViewModel
                                    {
                                        ID = visitor.ID,
                                        Name = visitor.Name,
                                        VaccinationStatus = vaccination.VaccinationStatus,
                                        RegisteredDate = vaccination.RegisteredDate,
                                        RegisteredBy = registeredBy.FullName
                                    }).Count();

                return visitorCount;
            }
            catch { return 0; }
        }

        public VisitorViewModel GetVisitorByID(Guid visID)
        {
            try
            {
                var visitor = (from vis in context.Visitors
                               join vaccination in context.Vaccinations on vis.ID equals vaccination.VisitorID
                               join vaccine in context.Vaccines on vaccination.VaccineID equals vaccine.ID
                               join registeredBy in context.UserMasters on vaccination.RegisteredBy equals registeredBy.UserName into pp1
                               from registeredBy in pp1.DefaultIfEmpty()
                               join administeredBy in context.UserMasters on vaccination.RegisteredBy equals administeredBy.UserName into pp2
                               from administeredBy in pp2.DefaultIfEmpty()
                               where vis.ID == visID
                               select new VisitorViewModel
                               {
                                   ID = vis.ID,
                                   Name = vis.Name,
                                   ICNo = vis.ICNo,
                                   CountryID = vis.CountryID,
                                   RaceID = vis.RaceID,
                                   Sex = vis.Sex,
                                   Age = vis.Age,
                                   SalaryNo = vis.SalaryNo,
                                   Pregnant = vis.Pregnant,
                                   Allergy = vis.Allergy,
                                   Remarks = vis.Remarks,
                                   Photo = vis.Photo,
                                   KPStreet = vis.KPStreet,
                                   KPArea = vis.KPArea,
                                   KPCity = vis.KPCity,
                                   KPState = vis.KPState,
                                   KPPostCode = vis.KPPostCode,
                                   ResidentStreet = vis.ResidentStreet,
                                   ResidentArea = vis.ResidentArea,
                                   ResidentCity = vis.ResidentCity,
                                   ResidentState = vis.ResidentState,
                                   ResidentPostCode = vis.ResidentPostCode,
                                   BusinessTypeID = vis.BusinessTypeID,
                                   BusinessName = vis.BusinessName,
                                   CompanyRegNo = vis.CompanyRegNo,
                                   BusinessStreet = vis.BusinessStreet,
                                   BusinessArea = vis.BusinessArea,
                                   BusinessCity = vis.BusinessCity,
                                   BusinessState = vis.BusinessState,
                                   BusinessPostCode = vis.BusinessPostCode,
                                   ReceiptNo = vis.ReceiptNo,
                                   TicketNo = vaccination.TicketNo,
                                   SerialNo = vaccination.SerialNo,
                                   VaccinationDate = vaccination.VaccinationDate,
                                   VaccinationStatus = vaccination.VaccinationStatus,
                                   VaccinationRemarks = vaccination.Remarks,
                                   VaccineID = vaccine.ID,
                                   RegisteredDate = vaccination.RegisteredDate,
                                   RegisteredBy = registeredBy.FullName,
                                   AdministeredBy = administeredBy.FullName
                               }).FirstOrDefault();

                return visitor;
            }
            catch
            {
                return null;
            }
        }
    }
}
