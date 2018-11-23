using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Models;
using Models.Enums;

namespace Infrastructure
{
    public class DatabaseSeedMiddleware
    {
        private readonly RequestDelegate _next;

        public DatabaseSeedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationContext db)
        {
            db.Database.Migrate();
            if (!db.Categories.Any())
            {
                db.Categories.AddRange(new Category()
                    {
                        Id = Guid.NewGuid(),
                        Image = "https://tshirtbg.com/image/catalog/revslider_media_folder/3715209__3.png",
                        Name = "T-Shirts"
                    },
                    new Category()
                    {
                        Id = Guid.NewGuid(),
                        Image =
                            "https://ae01.alicdn.com/kf/HTB1aLMvJVXXXXaAXXXXq6xXFXXXf/2017-New-Brand-Slim-Men-Bomber-Jackets-Casual-Fashion-Plaid-PU-Leather-Jacket-Men-Jaqueta-de.jpg_640x640.jpg",
                        Name = "Jackets",
                        Products = new List<Product>
                        {
                            new Product()
                            {
                                Id = Guid.NewGuid(),
                                Color = "black",
                                Description = "some description",
                                Details = "some details",
                                ImageUrls = "https://ae01.alicdn.com/kf/HTB112MnKFXXXXXtXFXXq6xXFXXXS/Autumn-Soft-Faux-Leather-Jackets-Men-2018-Fashion-Solid-Slim-Fit-Motorcycle-Jacket-Top-Quality-Men.jpg_640x640.jpg",
                                Name = "Autumn soft lether jacket",
                                Price = 20,
                                Sex = Sex.Men
                            },
                            new Product()
                            {
                                Id = Guid.NewGuid(),
                                Color = "yellow",
                                Description = "some description",
                                Details = "some details",
                                ImageUrls = "https://d3hcojf5o91ouy.cloudfront.net/store/media/catalog/product/cache/3/small_image/470x500/9df78eab33525d08d6e5fb8d27136e95/2/1/215085_TANN_1.jpg",
                                Name = "Women jacket",
                                Price = 15,
                                Sex = Sex.Women
                            },
                            new Product()
                            {
                                Id = Guid.NewGuid(),
                                Color = "brown",
                                Description = "some description",
                                Details = "some details",
                                ImageUrls = "https://images-na.ssl-images-amazon.com/images/I/A14qL9a5jPL._UX679_.jpg",
                                Name = "Carhafft Boys",
                                Sex = Sex.Men,
                                Price = 51
                            },
                            new Product()
                            {
                                Id = Guid.NewGuid(),
                                Color = "black",
                                Description = "some description",
                                Details = "some details",
                                ImageUrls = "https://cdn.shopify.com/s/files/1/0423/8761/products/Robyn_CO3504_web_v01_grande.jpg?v=1524524461",
                                Name = "Black Bomber Leather Jacket",
                                Sex = Sex.Men,
                                Price = 10
                            },
                        }
                    });

                db.SaveChanges();
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
