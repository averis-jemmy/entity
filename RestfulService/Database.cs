using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyAveris
{
    public static class Database
    {
        private static SQLiteConnection db;

        public static SQLiteConnection newInstance(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            return db;
        }

        public static void CreateTables()
        {
            try
            {
                db.CreateTable<User>();
                db.CreateTable<DeclarationDetail>();
                db.CreateTable<JobApplication>();
                db.CreateTable<JobApplicationAddress>();
            }
            catch { }
        }

        public static void UpdateDeclarationDetails(List<DeclarationDetail> details)
        {
            try
            {
                if (db.Table<DeclarationDetail>().Count() != 0)
                {
                    db.DeleteAll<DeclarationDetail>();
                }

                db.InsertAll(details);
            }
            catch { }
        }

        public static User GetUser()
        {
            try
            {
                var model = (from s in db.Table<User>()
                             select s).FirstOrDefault();
                return model;
            }
            catch { }

            return null;
        }

        public static void DeleteUsers()
        {
            try
            {
                db.DeleteAll<User>();
            }
            catch { }
        }

        public static void ClearData()
        {
            try
            {
                db.DeleteAll<User>();
                db.DeleteAll<JobApplication>();
            }
            catch { }
        }

        public static User UpdateUser(User model)
        {
            try
            {
                if (db.Table<User>().Count() != 0)
                {
                    db.DeleteAll<User>();
                }

                db.Insert(model);

                try
                {
                    var item = (from s in db.Table<User>()
                                  select s).FirstOrDefault();
                    return item;
                }
                catch { }
            }
            catch { }

            return null;
        }

        public static JobApplication GetJobApplication()
        {
            try
            {
                var model = (from s in db.Table<JobApplication>()
                             select s).FirstOrDefault();
                return model;
            }
            catch { }

            return null;
        }

        public static JobApplication InsertJobApplication(JobApplication model)
        {
            try
            {
                if (db.Table<JobApplication>().Count() != 0)
                {
                    db.DeleteAll<JobApplication>();
                }

                db.Insert(model);

                try
                {
                    var item = (from s in db.Table<JobApplication>()
                                select s).FirstOrDefault();
                    return item;
                }
                catch { }
            }
            catch { }

            return null;
        }

        public static JobApplication UpdateJobApplication(JobApplication model)
        {
            try
            {
                db.Update(model);

                try
                {
                    var item = (from s in db.Table<JobApplication>()
                                select s).FirstOrDefault();
                    return item;
                }
                catch { }
            }
            catch { }

            return null;
        }
    }
}
