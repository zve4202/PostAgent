using PostAgent.Domain.App;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostAgent.databases
{
    public class Databases : IDisposable
    {


        private static Databases module = null;
        public static Databases GetModule()
        {
            if (module == null)
            {
                module = new Databases();
            }
            return module;
        }

        private readonly Dictionary<EnumBaseId, DbContecxtAbstract> contexts = null;

        public Databases()
        {
            contexts = new Dictionary<EnumBaseId, DbContecxtAbstract>();
            try
            {
                contexts.Add(EnumBaseId.ShopId, new ShopDbContext());
                contexts.Add(EnumBaseId.DiscogsId, new DiscogsDbContext());
            }
            catch (Exception err)
            {
                Log.Error(err, "Error");
            }
        }

        public void Open()
        {
            foreach (var item in contexts)
            {
                item.Value.Open();
            }
        }

        public void ReOpen()
        {
            foreach (var item in contexts)
            {
                item.Value.ReOpen();
            }
        }
        public void Dispose()
        {
            foreach (var item in contexts)
            {
                item.Value.Close();
                item.Value.Dispose();
            }
        }

        public BaseIdLists GetBaseIdLists()
        {
            var pairs = new BaseIdLists();
            foreach (var item in contexts)
            {
                pairs.Add(item.Key, item.Value.GetLetterList());
            }
            return pairs;
        }

        public async Task<BaseIdLists> GetLetters()
        {
            return await Task<BaseIdLists>.Run(() =>
            {
                return GetBaseIdLists();
            });
        }

        public async Task Save()
        {
            await Task.Run(() =>
            {
                foreach (var item in contexts)
                {
                    item.Value.SaveChanges();
                }
            });
        }

    }
}
