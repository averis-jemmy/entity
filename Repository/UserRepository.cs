using ESuntikanCommon;
using ESuntikanModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace ESuntikanAppData
{
    public class UserRepository : RepositoryBase<UserMaster>
    {
        public UserRepository(ESuntikanLocalDB context) : base(context) { }

        public bool CheckUserID(string userName)
        {
            var query = (from userMaster in context.UserMasters
                         join userGroup in context.UserGroups on userMaster.ID equals userGroup.UserID
                         join groupMaster in context.GroupMasters on userGroup.GroupID equals groupMaster.ID
                         where userMaster.UserName == userName && userMaster.IsDeleted == false && userMaster.IsActive == true && groupMaster.IsDeleted == false
                         select userMaster).Any();
            return query;
        }

        public UserInfo Login(string userName, string password)
        {
            var userInfo = new UserInfo();

            using (var context = new ESuntikanLocalDB())
            {
                var query = (from userMaster in context.UserMasters
                             join userGroup in context.UserGroups on userMaster.ID equals userGroup.UserID
                             join groupMaster in context.GroupMasters on userGroup.GroupID equals groupMaster.ID
                             join groupPermission in context.GroupPermissions on groupMaster.ID equals groupPermission.GroupID
                             join applicationObject in context.ApplicationObjects on groupPermission.ObjectID equals applicationObject.ID
                             where userMaster.UserName == userName && userMaster.Password == password && userMaster.IsDeleted == false && userMaster.IsActive == true && groupMaster.IsDeleted == false && applicationObject.IsDeleted == false
                             select new AnonymousType()
                             {
                                 UserMaster = new UserMasterRe()
                                 {
                                     ID = userMaster.ID,
                                     UserName = userMaster.UserName,
                                     Password = userMaster.Password,
                                     JobTitle = userMaster.JobTitle,
                                     FullName = userMaster.FullName,
                                     ShortName = userMaster.ShortName,
                                     PhoneNumber = userMaster.PhoneNumber,
                                     Email = userMaster.Email
                                 },
                                 UserGroup = userGroup,
                                 GroupMaster = groupMaster,
                                 GroupPermission = groupPermission,
                                 ApplicationObject = applicationObject
                             }).ToList();

                if (query.Count > 0)
                {
                    userInfo = GetUserInfo(userInfo, query);
                    UpdateLastSuccessLogin(userInfo);
                    return userInfo;
                }
            }

            return null;
        }

        public UserInfo Login(string userName)
        {
            var userInfo = new UserInfo();

            using (var context = new ESuntikanLocalDB())
            {
                var query = (from userMaster in context.UserMasters
                             join userGroup in context.UserGroups on userMaster.ID equals userGroup.UserID
                             join groupMaster in context.GroupMasters on userGroup.GroupID equals groupMaster.ID
                             join groupPermission in context.GroupPermissions on groupMaster.ID equals groupPermission.GroupID
                             join applicationObject in context.ApplicationObjects on groupPermission.ObjectID equals applicationObject.ID
                             where userMaster.UserName == userName && userMaster.IsDeleted == false && userMaster.IsActive == true && groupMaster.IsDeleted == false && applicationObject.IsDeleted == false
                             select new AnonymousType()
                             {
                                 UserMaster = new UserMasterRe()
                                 {
                                     ID = userMaster.ID,
                                     UserName = userMaster.UserName,
                                     Password = userMaster.Password,
                                     JobTitle = userMaster.JobTitle,
                                     FullName = userMaster.FullName,
                                     ShortName = userMaster.ShortName,
                                     PhoneNumber = userMaster.PhoneNumber,
                                     Email = userMaster.Email
                                 },
                                 UserGroup = userGroup,
                                 GroupMaster = groupMaster,
                                 GroupPermission = groupPermission,
                                 ApplicationObject = applicationObject
                             }).ToList();

                if (query.Count > 0)
                {
                    userInfo = GetUserInfo(userInfo, query);
                    UpdateLastSuccessLogin(userInfo);
                    return userInfo;
                }
            }

            return null;
        }

        public static void UpdateLastSuccessLogin(UserInfo userInfo)
        {
            try
            {
                using (var context = new ESuntikanLocalDB())
                {
                    var q = from usermaster in context.UserMasters
                            where usermaster.UserName == userInfo.UserName
                            select usermaster;
                    if (q.Count() > 0)
                    {
                        UserMaster u = q.FirstOrDefault();
                        u.LastLoginDate = DateTime.Now;

                        context.Entry(u).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            catch
            { }
        }

        private static UserInfo GetUserInfo(UserInfo userInfo, List<AnonymousType> query)
        {
            var user = (from item in query
                        select item.UserMaster).FirstOrDefault();

            var userGroups = (from item in query
                              select item.UserGroup).Distinct().ToArray();

            var groupMasters = (from item in query
                                select item.GroupMaster).Distinct().ToArray();

            var groupPermissions = (from item in query
                                    select item.GroupPermission).Distinct().ToArray();

            var applicationObjects = (from item in query
                                      select item.ApplicationObject).Distinct().ToArray();

            if (user != null)
            {
                userInfo.ID = user.ID;
                userInfo.UserName = user.UserName;
                userInfo.Password = user.Password;
                userInfo.FullName = user.FullName;
                userInfo.Email = user.Email;
                userInfo.ShortName = user.ShortName;

                userInfo.UserProfile = new ProfileUserViewModel()
                {
                    ID = user.ID,
                    ProfileJobTitle = user.JobTitle,
                    ProfilePassword = user.Password.Substring(0, 16),
                    ProfileConfirmPassword = user.Password.Substring(0, 16),
                    ProfileFullName = user.FullName,
                    ProfileUserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    ExtensionNumber = user.Extension,
                    Email = user.Email
                };

                var groups = new List<GroupInfo>();
                var applicationObjectInfos = new List<ApplicationObjectInfo>();

                foreach (var userGroup in userGroups)
                {
                    var group = new GroupInfo();
                    var groupMaster = groupMasters.Where(p => p.ID == userGroup.GroupID).FirstOrDefault();

                    if (groupMaster != null)
                    {
                        group.ID = groupMaster.ID;
                        group.GroupName = groupMaster.GroupName;
                        group.Description = groupMaster.Description;
                    }

                    groups.Add(group);
                }

                foreach (var groupPermission in groupPermissions)
                {
                    var applicationObjectInfo = new ApplicationObjectInfo();
                    var applicationObject = applicationObjects.Where(p => p.ID == groupPermission.ObjectID).FirstOrDefault();

                    if (applicationObject != null)
                    {
                        applicationObjectInfo.ObjectCode = applicationObject.Code;
                        applicationObjectInfo.ObjectName = applicationObject.Name;
                    }

                    applicationObjectInfo.Permission = new PermissionInfo()
                    {
                        AllowAdd = groupPermission.AllowAdd,
                        AllowDelete = groupPermission.AllowDelete,
                        AllowEdit = groupPermission.AllowEdit,
                        AllowExport = groupPermission.AllowExport,
                        AllowFullAccess = groupPermission.AllowFullAccess.GetValueOrDefault(),
                        AllowImport = groupPermission.AllowImport,
                        AllowView = groupPermission.AllowView
                    };

                    applicationObjectInfos.Add(applicationObjectInfo);
                }

                //assign group
                userInfo.Groups = groups.ToArray();
                var dics = new Dictionary<string, PermissionInfo>();
                var objectNames = applicationObjectInfos.Select(p => p.ObjectCode).Distinct();

                foreach (var objectCode in objectNames)
                {
                    var permissions = from row in applicationObjectInfos
                                      where row.ObjectCode == objectCode
                                      select row.Permission;

                    bool allowAdd = false;
                    bool allowDelete = false;
                    bool allowEdit = false;
                    bool allowExport = false;
                    bool allowFullAccess = false;
                    bool allowImport = false;
                    bool allowView = false;
                    foreach (var item in permissions)
                    {
                        allowAdd |= item.AllowAdd;
                        allowDelete |= item.AllowDelete;
                        allowEdit |= item.AllowEdit;
                        allowExport |= item.AllowExport;
                        allowFullAccess |= item.AllowFullAccess;
                        allowImport |= item.AllowImport;
                        allowView |= item.AllowView;
                    }

                    dics.Add(objectCode, new PermissionInfo()
                    {
                        AllowAdd = allowAdd,
                        AllowDelete = allowDelete,
                        AllowEdit = allowEdit,
                        AllowExport = allowExport,
                        AllowFullAccess = allowFullAccess,
                        AllowImport = allowImport,
                        AllowView = allowView
                    });
                }

                userInfo.ApplicationObjectPermissions = dics;
                userInfo.ApplicationObjects = applicationObjectInfos.ToArray();

                return userInfo;
            }
            else
            {
                return null;
            }
        }

        public KeyValuePair<Guid, string>[] GetActorID()
        {
            var query = (from user in GetAll().ToList()
                         orderby user.FullName descending
                         where user.IsDeleted == null || user.IsDeleted == false
                         select new KeyValuePair<Guid, string>(user.ID, user.FullName));

            return query.ToArray();
        }
    }

    public class AnonymousType
    {
        public UserMasterRe UserMaster { get; set; }
        public GroupMaster GroupMaster { get; set; }
        public UserGroup UserGroup { get; set; }
        public GroupPermission GroupPermission { get; set; }
        public ApplicationObject ApplicationObject { get; set; }
        public ApplicationObjectType ApplicationObjectType { get; set; }
    }

    public class UserMasterRe : UserMaster { }
}
