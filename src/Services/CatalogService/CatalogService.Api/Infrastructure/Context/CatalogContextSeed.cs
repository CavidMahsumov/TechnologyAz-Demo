using CatalogService.Api.Core.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using System.Globalization;
using System.IO.Compression;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
        {
            var policy = Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Deneme {retry} sırasında {ExceptionType} türünde hata oluştu: {Message}", "CatalogContextSeed", exception.GetType().Name, exception.Message, retry);
                    });

            var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");
            var picturePath = "Pics";

            await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath, logger));
        }

        private async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath, ILogger logger)
        {
            if (!context.CatalogBrands.Any())
            {
                logger.LogInformation("Katalog markaları tohumlanıyor...");
                await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));
                await context.SaveChangesAsync();
            }

            if (!context.CatalogTypes.Any())
            {
                logger.LogInformation("Katalog türleri tohumlanıyor...");
                await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));
                await context.SaveChangesAsync();
            }

            if (!context.CatalogItems.Any())
            {
                logger.LogInformation("Katalog öğeleri tohumlanıyor...");
                await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath, context));
                await context.SaveChangesAsync();

                logger.LogInformation("Katalog öğe resimleri tohumlanıyor...");
                GetCatalogItemPictures(setupDirPath, picturePath);
            }

            logger.LogInformation("Tohumlama işlemi tamamlandı.");
        }

        private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentPath)
        {
            IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
            {
                return new List<CatalogBrand>()
                {
                    new CatalogBrand() { Brand = "Azure" },
                    new CatalogBrand() { Brand = ".NET" },
                    new CatalogBrand() { Brand = "Visual Studio" },
                    new CatalogBrand() { Brand = "SQL Server" },
                    new CatalogBrand() { Brand = "Other" },
                };
            }

            string fileName = Path.Combine(contentPath, "BrandsTextFile.txt");
            if (!File.Exists(fileName))
            {
                return GetPreconfiguredCatalogBrands();
            }

            var fileContent = File.ReadAllLines(fileName);
            var list = fileContent.Select(i => new CatalogBrand()
            {
                Brand = i.Trim('"'),
            }).Where(i => i != null);

            return list ?? GetPreconfiguredCatalogBrands();
        }

        private IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentPath)
        {
            IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
            {
                return new List<CatalogType>
                {
                    new CatalogType { Type = "Mug" },
                    new CatalogType { Type = "T-Shirt" },
                    new CatalogType { Type = "Sheet" },
                    new CatalogType { Type = "USB Memory Stick" }
                };
            }

            string fileName = Path.Combine(contentPath, "CatalogTypes.txt");
            if (!File.Exists(fileName))
            {
                return GetPreconfiguredCatalogTypes();
            }

            var fileContent = File.ReadAllLines(fileName);
            var list = fileContent.Select(i => new CatalogType()
            {
                Type = i.Trim('"'),
            }).Where(i => i != null);

            return list ?? GetPreconfiguredCatalogTypes();
        }

        private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentPath, CatalogContext context)
        {
            IEnumerable<CatalogItem> GetPreconfiguredItems()
            {
                return new List<CatalogItem>()
                {
                    new CatalogItem(){ CatalogTypeId=2, CatalogBrandId=1, AvailableStock=100, Description=".NET Bot Black Hoodie", Name=".NET Bot Black Hoodie", Price=20, PictureFileName="1.png", OnReorder=false },
                    new CatalogItem(){ CatalogTypeId=1, CatalogBrandId=2, AvailableStock=100, Description=".NET Black &White Mug", Name=".NET Black &White Mug", Price=8, PictureFileName="2.png", OnReorder=true },
                    new CatalogItem(){ CatalogTypeId=2, CatalogBrandId=5, AvailableStock=100, Description="Prism White T - Shirt", Name="Prism White T - Shirt", Price=12, PictureFileName="2.png", OnReorder=false },
                    new CatalogItem(){ CatalogTypeId=2, CatalogBrandId=2, AvailableStock=100, Description=".NET Foundation T - Shirt", Name=".NET Foundation T - Shirt", Price=12, PictureFileName="2.png", OnReorder=false },
                    new CatalogItem(){ CatalogTypeId=3, CatalogBrandId=5, AvailableStock=100, Description="Roslyn Red Sheet", Name="Roslyn Red Sheet", Price=8, PictureFileName="2.png", OnReorder=false },
                };
            }

            string filename = Path.Combine(contentPath, "CatalogItems.txt");
            if (!File.Exists(filename))
            {
                return GetPreconfiguredItems();
            }

            var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
            var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);

            var fileContent = File.ReadAllLines(filename)
                .Skip(1) // Skip header row
                .Select(i => i.Split(','))
                .Select(i => new CatalogItem()
                {
                    CatalogTypeId = catalogTypeIdLookup[i[0]],
                    CatalogBrandId = catalogBrandIdLookup[i[1]],
                    Description = i[2].Trim('"').Trim(),
                    Name = i[3].Trim('"').Trim(),
                    Price = Decimal.Parse(i[4].Trim('"').Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                    PictureFileName = i[5].Trim('"').Trim(),
                    AvailableStock = string.IsNullOrEmpty(i[6]) ? 0 : int.Parse(i[6]),
                    OnReorder = Convert.ToBoolean(i[7])
                });

            return fileContent;
        }

        private void GetCatalogItemPictures(string contentPath, string picturePath)
        {
            picturePath ??= "pics";
            if (picturePath != null)
            {
                DirectoryInfo directory = new DirectoryInfo(picturePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }
    }
}
