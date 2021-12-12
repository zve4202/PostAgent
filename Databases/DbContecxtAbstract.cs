using GH.Context;
using GH.Cfg;
using PostAgent.Domain;
using PostAgent.Domain.App;
using PostAgent.Domain.Cfgs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PostAgent.databases
{
    public class DbContecxtAbstract : DbContext
    {
        public DbContecxtAbstract() : base("name=Databse")
        {
            Database.Connection.ConnectionString = GetConnectionString();
        }

        private string GetConnectionString()
        {
            return GetCfg().ConnectionString();
        }

        protected virtual CfgFirebird GetCfg()
        {
            CfgFirebird cfg = RunContext.GetCfgApp().Get<CfgFirebird>();
            cfg.Selected = EnumBaseId.ShopId;
            return cfg;
        }

        public void ReOpen()
        {
            try
            {
                Database.Connection.Close();
                Database.Connection.ConnectionString = GetConnectionString();
                Open();
            }
            catch (Exception err)
            {
                Log.Error(err, "Error");
            }
        }
        public void Open()
        {
            try
            {
                Database.Connection.Open();
            }
            catch (Exception err)
            {
                Log.Error(err, "Error");
            }
        }

        public void Close()
        {
            Database.Connection.Close();
        }

        public DbSet<Letter> Letters { get; set; }

        public List<Letter> GetLetterList()
        {
            this.DetachAll(Letters);

            var leters =
                from leter in Letters
                orderby leter.Id
                select leter;

            return leters.ToList<Letter>();
        }

    }
}
