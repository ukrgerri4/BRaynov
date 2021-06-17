using Application.Common.Interfaces;
using Domain.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.Implementations;
using Infrastructure.Database.Entities;
using Infrastructure.Identity;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Infrastructure.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IAppDbContext
    {
        private readonly IDbCacheEntityManager dbCacheEntityManager;
        private readonly ICurrentRequestService currentUserService;

        #region Constructors
        public AppDbContext(
            DbContextOptions options,
            IDbCacheEntityManager dbCacheEntityManager,
            ICurrentRequestService currentUserService
            ) : base(options)
        {
            this.dbCacheEntityManager = dbCacheEntityManager;
            this.currentUserService = currentUserService;
        }

        static AppDbContext()
        {
            QueryCacheManager.DefaultMemoryCacheEntryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromHours(2)
            };

            AuditManager.DefaultConfiguration.IgnorePropertyUnchanged = true;
            AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
               // ADD "Where(x => x.AuditEntryID == 0)" to allow multiple SaveChanges with same Audit
               (context as AppDbContext).AuditEntries.AddRange(audit.Entries.Cast<AuditEntryExtended>());

            AuditManager.DefaultConfiguration.Include(x => true);
            AuditManager.DefaultConfiguration
                .ExcludeProperty<IAuditable>(x =>
                    new
                    {
                        x.Created,
                        x.CreatedBy,
                        x.Modified,
                        x.ModifiedBy
                    }
                );
            AuditManager.DefaultConfiguration.ExcludeProperty<IVersioned>(x => x.RowVersion);
        }
        #endregion

        public DbSet<AuditEntryExtended> AuditEntries { get; set; }
        public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Profile> AuthorIdentities { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Implementation> Implementations { get; set; }
        

        #region Overrides
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public override EntityEntry Attach(object entity)
        {
            UpdateCachedTag(entity);
            return base.Attach(entity);
        }
        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        {
            UpdateCachedTag(entity);
            return base.Attach(entity);
        }
        public override void AttachRange(IEnumerable<object> entities)
        {
            UpdateCachedTags(entities);
            base.AttachRange(entities);
        }
        public override void AttachRange(params object[] entities)
        {
            UpdateCachedTags(entities);
            base.AttachRange(entities);
        }
        public override EntityEntry Add(object entity)
        {
            UpdateCachedTag(entity);
            return base.Add(entity);
        }
        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            UpdateCachedTag(entity);
            return base.Add(entity);
        }
        public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
        {
            UpdateCachedTag(entity);
            return base.AddAsync(entity, cancellationToken);
        }
        public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        {
            UpdateCachedTag(entity);
            return base.AddAsync(entity, cancellationToken);
        }
        public override void AddRange(IEnumerable<object> entities)
        {
            UpdateCachedTags(entities);
            base.AddRange(entities);
        }
        public override void AddRange(params object[] entities)
        {
            UpdateCachedTags(entities);
            base.AddRange(entities);
        }
        public override Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            UpdateCachedTags(entities);
            return base.AddRangeAsync(entities, cancellationToken);
        }
        public override Task AddRangeAsync(params object[] entities)
        {
            UpdateCachedTags(entities);
            return base.AddRangeAsync(entities);
        }
        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            UpdateCachedTag(entity);
            return base.Remove(entity);
        }
        public override EntityEntry Remove(object entity)
        {
            UpdateCachedTag(entity);
            return base.Remove(entity);
        }
        public override void RemoveRange(IEnumerable<object> entities)
        {
            UpdateCachedTags(entities);
            base.RemoveRange(entities);
        }
        public override void RemoveRange(params object[] entities)
        {
            UpdateCachedTags(entities);
            base.RemoveRange(entities);
        }
        public override EntityEntry Update(object entity)
        {
            UpdateCachedTag(entity);
            return base.Update(entity);
        }
        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            UpdateCachedTag(entity);
            return base.Update(entity);
        }
        public override void UpdateRange(IEnumerable<object> entities)
        {
            UpdateCachedTags(entities);
            base.UpdateRange(entities);
        }
        public override void UpdateRange(params object[] entities)
        {
            UpdateCachedTags(entities);
            base.UpdateRange(entities);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();

            var audit = new Audit();
            audit.Configuration.AuditEntryFactory = args => new AuditEntryExtended() { RequestId = currentUserService.RequestId };
            audit.CreatedBy = currentUserService.UserId.ToString();
            audit.PreSaveChanges(this);
            var rowAffecteds = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            ClearExpiredTags();
            audit.PostSaveChanges();

            if (audit.Configuration.AutoSavePreAction != null && audit.Entries.Count > 0)
            {
                audit.Configuration.AutoSavePreAction(this, audit);
                await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            //await DispatchEvents();

            return rowAffecteds;
        }

        //private async Task DispatchEvents()
        //{
        //    var domainEventEntities = ChangeTracker.Entries<IHasDomainEvent>()
        //        .Select(x => x.Entity.DomainEvents)
        //        .SelectMany(x => x)
        //        .Where(domainEvent => !domainEvent.IsPublished)
        //        .ToArray();

        //    foreach (var domainEvent in domainEventEntities)
        //    {
        //        domainEvent.IsPublished = true;
        //        await _domainEventService.Publish(domainEvent);
        //    }
        //}

        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker
                .Entries<IAuditable>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity;
                DateTimeOffset now = DateTimeOffset.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.Created = now;
                    entity.CreatedBy = currentUserService.UserId;
                }
                else
                {
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    entry.Property(x => x.Created).IsModified = false;
                }

                entity.Modified = now;
                entity.ModifiedBy = currentUserService.UserId;
            }
        }
        #endregion

        #region Cashing
        private void UpdateCachedTag(object entity)
        {
            if (entity is ICached cachableEntity)
            {
                dbCacheEntityManager.AddTag(cachableEntity.GetCacheTag());
            }
        }
        private void UpdateCachedTags(IEnumerable<object> entities)
        {
            var tags = new HashSet<string>();
            foreach (var entity in entities)
            {
                if (entity is ICached cachableEntity)
                {
                    tags.Add(cachableEntity.GetCacheTag());
                }
            }
            dbCacheEntityManager.AddTags(tags);
        }
        private void ClearExpiredTags()
        {
            QueryCacheManager.ExpireTag(dbCacheEntityManager.Tags);
            dbCacheEntityManager.Clear();
        }
        #endregion
    }
}
