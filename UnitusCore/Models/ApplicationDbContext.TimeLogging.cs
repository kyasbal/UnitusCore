using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models
{
    public partial class ApplicationDbContext
    {
        /// <summary>
        /// ModelbaseWithTimeLoggingに対して更新時の時間と作成時の時間を記録させる
        /// </summary>
        private void ApplyTimeLogging()
        {
            ObjectContext context = ((IObjectContextAdapter)this).ObjectContext;
            //ModelBaseWithTimeLoggingを継承しているクラスを探す
            IEnumerable<ObjectStateEntry> objectStateEntries
                = from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified)
                  where
                      e.IsRelationship == false && e.Entity != null &&
                      e.Entity is ModelBaseWithTimeLogging
                  select e;
            var currentTime = DateTime.Now;

            foreach (ObjectStateEntry entry in objectStateEntries)
            {
                var modelEntity = (ModelBaseWithTimeLogging)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    modelEntity.CreationDate = currentTime;
                }
                modelEntity.LastModefied = currentTime;
            }
        }

        public override int SaveChanges()
        {
            ApplyTimeLogging();
            return base.SaveChanges();
        }



        public override Task<int> SaveChangesAsync()
        {
            ApplyTimeLogging();
            return base.SaveChangesAsync();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ApplyTimeLogging();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}