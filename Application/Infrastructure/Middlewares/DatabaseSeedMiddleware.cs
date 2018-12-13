using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Application.Infrastructure.Middlewares
{
    public class DatabaseSeedMiddleware
    {
        private readonly RequestDelegate _next;

        public DatabaseSeedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationContext db, RoleManager<IdentityRole> roleManager)
        {
            db.Database.Migrate();

            var check = await roleManager.RoleExistsAsync("Admin");
            if (!check)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!db.Categories.Any())
            {
                await db.Categories.AddRangeAsync(new Category()
                {
                    Id = Guid.NewGuid(),
                    Image = "https://tshirtbg.com/image/catalog/revslider_media_folder/3715209__3.png",
                    Name = "T-Shirts",
                    Type = CategoryType.Clothes
                },
                    new Category()
                {
                    Id = Guid.NewGuid(),
                    Image = "https://tshirtbg.com/image/catalog/revslider_media_folder/3715209__3.png",
                    Name = "Shoes",
                    Type = CategoryType.Shoes
                },
                new Category()
                    {
                        Id = Guid.NewGuid(),
                        Image =
                            "https://ae01.alicdn.com/kf/HTB1aLMvJVXXXXaAXXXXq6xXFXXXf/2017-New-Brand-Slim-Men-Bomber-Jackets-Casual-Fashion-Plaid-PU-Leather-Jacket-Men-Jaqueta-de.jpg_640x640.jpg",
                        Name = "Jackets",
                        Type = CategoryType.Clothes,
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
            }

            if (!db.Sizes.Any())
            {
                await db.Sizes.AddRangeAsync(new ClothesSize
                {
                    Id = Guid.NewGuid(),
                    Name = "S"
                },
            new ClothesSize
            {
                Id = Guid.NewGuid(),
                Name = "M"
            },
            new ClothesSize
            {
                Id = Guid.NewGuid(),
                Name = "L"
            },
            new ClothesSize
            {
                Id = Guid.NewGuid(),
                Name = "XL"
            },
            new ClothesSize
            {
                Id = Guid.NewGuid(),
                Name = "XS"
            },
            new ClothesSize
            {
                Id = Guid.NewGuid(),
                Name = "XXL"
            });

                await db.Sizes.AddRangeAsync(new ShoesSize
                {
                    Id = Guid.NewGuid(),
                    Name = "40",
                    Sex = Sex.Men
                },
                    new ShoesSize
                    {
                        Id = Guid.NewGuid(),
                        Name = "41",
                        Sex = Sex.Men
                    },
                    new ShoesSize
                    {
                        Id = Guid.NewGuid(),
                        Name = "42",
                        Sex = Sex.Men
                    },
                    new ShoesSize
                    {
                        Id = Guid.NewGuid(),
                        Name = "43",
                        Sex = Sex.Men
                    },
                    new ShoesSize
                    {
                        Id = Guid.NewGuid(),
                        Name = "44",
                        Sex = Sex.Men
                    },
                    new ShoesSize
                    {
                        Id = Guid.NewGuid(),
                        Name = "45",
                        Sex = Sex.Men
                    });
            }



            //            if (!db.Reports.Any())
            //            {
            //                db.Reports.AddRange(new Report()
            //                {
            //                    Details =
            //                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            //                    ReporterId = "03cd4ab7-7864-44f1-84b5-729f4e65c0fb",
            //                    ReportedUserId = "1c560c6b-d724-47ca-b22f-7d0277593efd"
            //                }, new Report()
            //                {
            //                    Details =
            //                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            //                    ReporterId = "03cd4ab7-7864-44f1-84b5-729f4e65c0fb",
            //                    ReportedUserId = "1c560c6b-d724-47ca-b22f-7d0277593efd"
            //                });
            //            }

            await db.SaveChangesAsync();

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
