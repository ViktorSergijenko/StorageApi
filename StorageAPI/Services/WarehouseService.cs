﻿using StorageAPI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StorageAPI.Models;
using ZXing.QrCode;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace StorageAPI.Services
{
    public class WarehouseService
    {
        protected StorageContext DB { get; private set; }
        protected SimpleLogTableServcie SimpleLogTableService { get; private set; }
        private readonly UserManager<User> userManager;



        public WarehouseService(IServiceProvider service, UserManager<User> userManager)
        {
            this.userManager = userManager;
            DB = service.GetService<StorageContext>();
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();
        }

        #region BaseCrud

        /// <summary>
        /// Method gets warehouse by id
        /// </summary>
        /// <param name="id">Id of an warehouse that we want to get</param>
        /// <returns>Warehouse from DB</returns>
        public async Task<Warehouse> GetWarehouseById(Guid id) {
            // Getting warehouse from DB
            var warehouse = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == id);
            // Checking if it's not null
            if (warehouse == null)
            {
                // If it's null, then we will throw new exception
                throw new Exception("Not found");
            }
            // If object was found, then we return it
            else
            {
                return warehouse;
            }
        }
        /// <summary>
        /// Method add or modifies a  warehouse
        /// </summary>
        /// <param name="newWarehouse">Warehouse object that we want to add or modifie</param>
        /// <returns>New warehouse with id included or modified object</returns>
        public async Task<Warehouse> SaveWarehouse(Warehouse warehouse, string username) {

            // If warehouse does not have id, that means that it's a new entity, and we need an add functionality
            if (warehouse.Id == null || warehouse.Id.Equals(Guid.Empty))
            {
                var adminRoleId = await DB.Roles.FirstOrDefaultAsync(x => x.Name == "Level one");
                var adminId = await DB.UserRoles.FirstOrDefaultAsync(x => x.RoleId == adminRoleId.Id);

                // Adding new warehouse to DB
               await DB.WarehouseDB.AddAsync(warehouse);
                var creator = await DB.Users.FirstOrDefaultAsync(x => x.FullName == username);
                if (creator.Id == adminId.UserId)
                {
                    UserWarehouse userWarehouse = new UserWarehouse
                    {
                        WarehouseId = warehouse.Id,
                        UserId = creator.Id,
                        DoesUserHaveAbilityToSeeProductAmount = true,
                        WarehousePositionInTable = await DB.UserWarehouseDB.CountAsync(x => x.UserId == creator.Id) + 1
                    };

                    await DB.UserWarehouseDB.AddAsync(userWarehouse);

                }
                else
                {
                    UserWarehouse userWarehouse = new UserWarehouse
                    {
                        WarehouseId = warehouse.Id,
                        UserId = creator.Id,
                        DoesUserHaveAbilityToSeeProductAmount = true,
                        WarehousePositionInTable = await DB.UserWarehouseDB.CountAsync(x => x.UserId == creator.Id) + 1
                    };

                    UserWarehouse userWarehouseForAdmin = new UserWarehouse
                    {
                        WarehouseId = warehouse.Id,
                        UserId = adminId.UserId,
                        DoesUserHaveAbilityToSeeProductAmount = true,
                        WarehousePositionInTable = await DB.UserWarehouseDB.CountAsync(x => x.UserId == adminId.UserId) + 1
                    };
                    await DB.UserWarehouseDB.AddAsync(userWarehouse);
                    await DB.UserWarehouseDB.AddAsync(userWarehouseForAdmin);


                }

                await SimpleLogTableService.AddAdminLog($"Izveidoja noliktavu: {warehouse.Name}", username);
                // Saving changes in DB
                await DB.SaveChangesAsync();
                // Generating QR code for warehouse
                warehouse = await AddQrCodeForWarehouse(warehouse);
            }
            // If warehouse has an id, that means that it's  not a new entity, and we need an edit functionality
            else
            {
                
                // Getting object from DB that has similar id like in our param variable
                var warehouseFromDb = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == warehouse.Id);
                // Using mapper to edit all fields
                Mapper.Map(warehouse, warehouseFromDb);
                // Updating DB
                DB.WarehouseDB.Update(warehouseFromDb);
                // Saving changes in DB
                await DB.SaveChangesAsync();
            }
            // Returning object
            return warehouse;
        }

        /// <summary>
        /// Method deletes warehouse from DB
        /// </summary>
        /// <param name="id">Id of an warehouse to delete</param>
        /// <returns></returns>
        public async Task DeleteWarehouse(Guid id, string username) {
            // Getting warehouse from DB with the same id like in param
            var warehouse = await DB.WarehouseDB.FirstOrDefaultAsync(x => x.Id == id);
            // Checkinf if warehouse variable for null
            if (warehouse == null)
            {
                // If it's null then we throw exception
                 throw new Exception("Not found");
            }
            // Removing warehouse from DB
            DB.WarehouseDB.Remove(warehouse);
            await SimpleLogTableService.AddAdminLog($"Nodzesa noliktavu: {warehouse.Name}", username);

            // Saving changes
            await DB.SaveChangesAsync();
        }
        #endregion BaseCrud

        #region QrCodeGenerator

        public async Task<Warehouse> AddQrCodeForWarehouse(Warehouse createdWarehouse) {
            
            var QrcodeContent = $"https://warehouse-manager.azurewebsites.net/#/pages/warehouse/details/{createdWarehouse.Id}";
            var width = 250; // width of the Qr Code
            var height = 250; // height of the Qr Code
            var margin = 0;

            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions { Height = height, Width = width, Margin = margin }
            };

            var pixelData = qrCodeWriter.Write(QrcodeContent);
            // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference
            // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            using (var ms = new MemoryStream())
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                    pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                // save to stream as PNG
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                createdWarehouse.QrCodeBase64 = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray()));
                DB.Entry(createdWarehouse).State = EntityState.Modified;
                await DB.SaveChangesAsync();
                return createdWarehouse;

            }
        }
        #endregion QrCodeGenerator

        #region Filtration method
        /// <summary>
        /// Method gets filtered warehouse query, depednding on the filter option
        /// </summary>
        /// <param name="filterOption">Filter option, by which filtration will be done</param>
        /// <returns></returns>
        public async Task<List<Warehouse>> FilterWarehouses(FilterSorting filterSorting) {
        
            // Getting our warehouse query, that we will filter
            var warehouseQuery = await DB.WarehouseDB.ToListAsync();
            // Checking if our filter option is null
            if (!String.IsNullOrEmpty(filterSorting.FilterOption))
            {
                // If it's not null, then we set this option to lover case
                var filter = filterSorting.FilterOption.ToLower();
                // Filtering our query, where warehouse address or name contains something similar to our option
                warehouseQuery = warehouseQuery.Where(x => x.Name.ToLower().Contains(filter) 
                || x.Address.ToLower().Contains(filter)
                || x.Location.ToLower().Contains(filter)).ToList();
            }
            else
            {
                // TODO: This is only for short period of time, need to make functionality where user can chose by what field user can sort and in whick direction
                // If our option is null, then we just sorting our query by name
                warehouseQuery = warehouseQuery.OrderByDescending(x => x.Name).ToList();
            }
            return warehouseQuery;

        }

        public async Task<UserVM> AddUserToWarehouse(UserWarehouse userWarehouse)
        {
            var positionForNewUserWarehouse = await DB.UserWarehouseDB.CountAsync(x => x.UserId == userWarehouse.UserId);
            userWarehouse.WarehousePositionInTable = ++positionForNewUserWarehouse;
           
            await DB.UserWarehouseDB.AddAsync(userWarehouse);
            await DB.SaveChangesAsync();
            var addedUser = await DB.Users.FirstOrDefaultAsync(x => x.Id == userWarehouse.UserId);
            var userRoleName = await userManager.GetRolesAsync(addedUser);
            UserVM userWithRole = new UserVM
            {
                Id = addedUser.Id,
                FullName = addedUser.FullName,
                Email = addedUser.Email,
                RoleName = userRoleName[0],
                HasAbilityToLoad = addedUser.HasAbilityToLoad,
                ReportsTo = addedUser.ReportsTo
            };


            return userWithRole;
        }
        public async Task ChangeWarehousePosition(UserWarehouse userWarehouse)
        {
            var userWarehouseList = await DB.UserWarehouseDB
                          .AsNoTracking()
                          .OrderBy(x => x.WarehousePositionInTable)
                          .Where(x => x.UserId == userWarehouse.UserId)
                          .ToListAsync();
            if (userWarehouse.WarehousePositionInTable > userWarehouseList.Count || userWarehouse.WarehousePositionInTable < 1)
            {
                throw new Exception();
            }
            var warehouseOldPosition = await DB.UserWarehouseDB
               .Where(x => x.WarehouseId == userWarehouse.WarehouseId && x.UserId == userWarehouse.UserId)
               .Select(x => x.WarehousePositionInTable)
               .FirstOrDefaultAsync();
            var warehouseWhithTahtWeSwappingPlaces = userWarehouseList.FirstOrDefault(x => x.WarehousePositionInTable == userWarehouse.WarehousePositionInTable);
            warehouseWhithTahtWeSwappingPlaces.WarehousePositionInTable = warehouseOldPosition;
            DB.UserWarehouseDB.Update(warehouseWhithTahtWeSwappingPlaces);
            DB.UserWarehouseDB.Update(userWarehouse);
            await DB.SaveChangesAsync();
        }
        public async Task<UserVM> RemoveUserToWarehouse(UserWarehouse userWarehouse)
        {
            var warehousePosition = await DB.UserWarehouseDB
                .Where(x => x.WarehouseId == userWarehouse.WarehouseId && x.UserId == userWarehouse.UserId)
                .Select(x => x.WarehousePositionInTable)
                .FirstOrDefaultAsync();

            var userWarehouseList = await DB.UserWarehouseDB
                          .AsNoTracking()
                          .OrderBy(x => x.WarehousePositionInTable)
                          .Where(x => x.UserId == userWarehouse.UserId)
                          .ToListAsync();
            foreach (var warehouse in userWarehouseList)
            {
                if (warehouse.WarehousePositionInTable > warehousePosition)
                {
                    warehouse.WarehousePositionInTable = warehouse.WarehousePositionInTable - 1;
                    DB.UserWarehouseDB.Update(warehouse);
                }
            }
            DB.UserWarehouseDB.Remove(userWarehouse);
            await DB.SaveChangesAsync();
            var removedUser = await DB.Users.FirstOrDefaultAsync(x => x.Id == userWarehouse.UserId);
            var userRoleName = await userManager.GetRolesAsync(removedUser);
            UserVM userWithRole = new UserVM
            {
                Id = removedUser.Id,
                FullName = removedUser.FullName,
                Email = removedUser.Email,
                RoleName = userRoleName[0],
                HasAbilityToLoad = removedUser.HasAbilityToLoad,
                ReportsTo = removedUser.ReportsTo
            };

            return userWithRole;
        }

        public async Task<List<UserVM>> GetUsersThatAllowToUseWarehouse(UserWarehouse userWarehouse, string role)
        {
            List<UserVM> usersVM = new List<UserVM>();

            #region level one
            if (role == "Level one")
            {
                var usersForLoop = await DB.Users.Where(x => x.Id != userWarehouse.UserId).ToListAsync();

                foreach (var user in usersForLoop)
                {
                    if (await DB.UserWarehouseDB.AnyAsync(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId))
                    {
                        var userRoleName = await userManager.GetRolesAsync(user);
                        var abilityToSeeAmount = await DB.UserWarehouseDB.Where(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId).Select(x => x.DoesUserHaveAbilityToSeeProductAmount).ToListAsync();
                        UserVM userWithRole = new UserVM
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Email = user.Email,
                            RoleName = userRoleName[0],
                            HasAbilityToLoad = user.HasAbilityToLoad,
                            ReportsTo = user.ReportsTo,
                            DoesUserHaveAbilityToSeeProductAmount = abilityToSeeAmount[0]
                        };
                        usersVM.Add(userWithRole);
                    } 
                }
            }
            #endregion level one

            if (role == "Level two")
            {
                var allEmployees = new List<User>();
                var levelThreeEmployees = await DB.Users.Where(x => x.ReportsTo == userWarehouse.UserId).ToListAsync();
                allEmployees.AddRange(levelThreeEmployees);
                var employeesFirstLoop = await DB.Users.Where(x => x.ReportsTo == userWarehouse.UserId).ToListAsync();
                foreach (var employee in levelThreeEmployees)
                {
                    if (await DB.Users.AnyAsync(x => x.ReportsTo == employee.Id))
                    {
                        allEmployees.AddRange(await DB.Users.Where(x => x.ReportsTo == employee.Id).ToListAsync());
                    }
                }
                var listOnlyWithAllowedUsers = new List<User>();
                foreach (var user in allEmployees)
                {
                    if (await DB.UserWarehouseDB.AnyAsync(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId))
                    {
                        listOnlyWithAllowedUsers.Add(user);
                        var userRoleName = await userManager.GetRolesAsync(user);
                        var abilityToSeeAmount = await DB.UserWarehouseDB.Where(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId).Select(x => x.DoesUserHaveAbilityToSeeProductAmount).ToListAsync();
                        UserVM userWithRole = new UserVM
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Email = user.Email,
                            RoleName = userRoleName[0],
                            HasAbilityToLoad = user.HasAbilityToLoad,
                            ReportsTo = user.ReportsTo,
                            DoesUserHaveAbilityToSeeProductAmount = abilityToSeeAmount[0]
                        };
                        usersVM.Add(userWithRole);
                    }
                }

            }
            if (role == "Level three")
            {
                var levelFourEmployees = await DB.Users.Where(x => x.ReportsTo == userWarehouse.UserId).ToListAsync();
                var listOnlyWithAllowedUsers = new List<User>();
                listOnlyWithAllowedUsers.AddRange(levelFourEmployees);
                foreach (var user in levelFourEmployees)
                {
                    if (await DB.UserWarehouseDB.AnyAsync(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId))
                    {
                        var userRoleName = await userManager.GetRolesAsync(user);
                        var abilityToSeeAmount = await DB.UserWarehouseDB.Where(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId).Select(x => x.DoesUserHaveAbilityToSeeProductAmount).ToListAsync();
                        UserVM userWithRole = new UserVM
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Email = user.Email,
                            RoleName = userRoleName[0],
                            HasAbilityToLoad = user.HasAbilityToLoad,
                            ReportsTo = user.ReportsTo,
                            DoesUserHaveAbilityToSeeProductAmount = abilityToSeeAmount[0]
                        };
                        usersVM.Add(userWithRole);
                    }
                }

            }
            return usersVM;
        }

        }

        #endregion Filtration method
    }

