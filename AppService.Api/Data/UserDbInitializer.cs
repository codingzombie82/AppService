﻿using AppService.Models;
using AppService.Models.Contants;
using AppService.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AppService.Api.Data
{
    public class UserDbInitializer : IInitializer
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IEnumerable<IInitialData> initialDataProviders;

        public UserDbInitializer(
            ApplicationDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IEnumerable<IInitialData> initialDataProviders)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.initialDataProviders = initialDataProviders;
        }

        public void Initialize()
        {
            this.db.Database.Migrate();

            this.AddAdministrator();

            foreach (var initialDataProvider in this.initialDataProviders)
            {
                if (this.DataSetIsEmpty(initialDataProvider.EntityType))
                {
                    var data = initialDataProvider.GetData();

                    foreach (var entity in data)
                    {
                        this.db.Add(entity);
                    }
                }
            }

            this.db.SaveChanges();
        }

        private void AddAdministrator()
            => Task
                .Run(async () =>
                {
                    var existingRole = await this.roleManager.FindByNameAsync(IdentityConst.AdministratorRole);

                    if (existingRole != null)
                    {
                        return;
                    }

                    var adminRole = new AppRole(IdentityConst.AdministratorRole);

                    await this.roleManager.CreateAsync(adminRole);

                    var adminUser = new AppUser
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        Email = "admin@blazorshop.com",
                        UserName = "admin@blazorshop.com",
                        SecurityStamp = "RandomSecurityStamp"
                    };

                    await this.userManager.CreateAsync(adminUser, "admin123456");
                    await this.userManager.AddToRoleAsync(adminUser, IdentityConst.AdministratorRole);
                })
                .GetAwaiter()
                .GetResult();

        private bool DataSetIsEmpty(Type type)
        {
            var setMethod = this.GetType()
                .GetMethod(nameof(this.GetSet), BindingFlags.Instance | BindingFlags.NonPublic)
                ?.MakeGenericMethod(type);

            var set = setMethod?.Invoke(this, Array.Empty<object>());

            var countMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == nameof(Queryable.Count) && m.GetParameters().Length == 1)
                .MakeGenericMethod(type);

            var result = (int)countMethod.Invoke(null, new[] { set })!;

            return result == 0;
        }

        private DbSet<TEntity> GetSet<TEntity>()
            where TEntity : class
            => this.db.Set<TEntity>();
    }
}
