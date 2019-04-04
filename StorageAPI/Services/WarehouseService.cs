using StorageAPI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StorageAPI.Models;
using ZXing.QrCode;
using System.IO;


namespace StorageAPI.Services
{
    public class WarehouseService
    {
        protected StorageContext DB { get; private set; }

        public WarehouseService(IServiceProvider service)
        {
            DB = service.GetService<StorageContext>();
        }

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
        /// Method add a new warehouse to DB
        /// </summary>
        /// <param name="newWarehouse">Warehouse object that we want to add</param>
        /// <returns>New warehouse with id included</returns>
        public async Task<Warehouse> AddWarehouse(Warehouse newWarehouse) {
            // Adding new warehouse to DB
            DB.WarehouseDB.Add(newWarehouse);
            // Saving changes in DB
            await DB.SaveChangesAsync();


            var createdWarehouseWithQrCode = await AddQrCodeForWarehouse(newWarehouse);
            return createdWarehouseWithQrCode;
        }

        public async Task<Warehouse> AddQrCodeForWarehouse(Warehouse createdWarehouse) {
            
           

            var QrcodeContent = $"http://localhost:4200/api/{createdWarehouse.Id}";
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

        public async Task<List<Warehouse>> FilterWarehouses(string filterOption) {
        
            // Getting our warehouse query, that we will filter
            var warehouseQuery = await DB.WarehouseDB.ToListAsync();
            // Checking if our filter option is null
            if (!String.IsNullOrEmpty(filterOption))
            {
                // If it's not null, then we set this option to lover case
                var filter = filterOption.ToLower();
                // Filtering our query, where warehouse address or name contains something similar to our option
                warehouseQuery = warehouseQuery.Where(x => x.Name.ToLower().Contains(filterOption) 
                || x.Address.ToLower().Contains(filterOption)).ToList();
            }
            else
            {
                // TODO: This is only for short period of time, need to make functionality where user can chose by what field user can sort and in whick direction
                // If our option is null, then we just sorting our query by name
                warehouseQuery = warehouseQuery.OrderByDescending(x => x.Name).ToList();
            }
            return warehouseQuery;

        }
    }
}
