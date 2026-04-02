using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;

namespace Propertia.Controllers
{
    [ApiController]
    [Route("api/properties")]
    public class PropertyController : ControllerBase
    {
        private readonly PropertiaContext _db;

        public PropertyController(PropertiaContext db)
        {
            _db = db;
        }
        // GET: api/properties
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var properties = await _db.Properties
        //        .Include(p => p.User)
        //        .Include(p => p.Category)
        //        .Include(p => p.PropertyAddress)
        //        .Include(p => p.PropertyPrices)
        //            .ThenInclude(pp => pp.TransactionType)
        //        .Select(p => new
        //        {
        //            p.PropertyId,
        //            p.Title,
        //            p.Description,
        //            p.AreaSqft,
        //            p.Status,
        //            p.RequireType,
        //            p.CreatedAt,

        //            // ✅ USER
        //            User = new
        //            {
        //                p.User.UserId,
        //                p.User.Name
        //            },

        //            // ✅ CATEGORY
        //            Category = new
        //            {
        //                p.Category.CategoryId,
        //                p.Category.CategoryName
        //            },

        //            // ✅ ADDRESS
        //            Address = new
        //            {
        //                p.PropertyAddress.Location,
        //                p.PropertyAddress.City,
        //                p.PropertyAddress.State,
        //                p.PropertyAddress.Country
        //            },
        //            Amenities = p.PropertyAmenities.Select(pa => pa.Amenity.AmenityName),

        //            // ✅ IMAGES
        //            Images = p.PropertyImages.Select(img => img.ImagePath),

        //            // ✅ PRICES
        //            Prices = p.PropertyPrices.Select(pp => new
        //            {
        //                pp.Amount,
        //                TransactionType = pp.TransactionType.TransactionTypeName
        //            })

        //        })
        //        .ToListAsync();

        //    return Ok(properties);
        //}
        // GET: api/properties
        //[Authorize(Roles = "admin,buyer,seller")]
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var properties = await _db.Properties
        //        .Include(p => p.User)
        //        .Include(p => p.Category)
        //        .Include(p => p.PropertyAddress)
        //        .Include(p => p.PropertyPrices)
        //            .ThenInclude(pp => pp.TransactionType)
        //        .Include(p => p.PropertyImages)
        //        .Include(p => p.PropertyAmenities)
        //            .ThenInclude(pa => pa.Amenity)
        //        .Select(p => new
        //        {
        //            p.PropertyId,
        //            p.Title,
        //            p.Description,
        //            p.AreaSqft,
        //            p.Status,
        //            p.RequireType,
        //            p.CreatedAt,

        //            // ✅ USER
        //            User = new
        //            {
        //                p.User.UserId,
        //                p.User.Name

        //            },

        //            // ✅ CATEGORY
        //            Category = new
        //            {
        //                p.Category.CategoryId,
        //                p.Category.CategoryName
        //            },

        //            // ✅ ADDRESS
        //            Address = new
        //            {
        //                p.PropertyAddress.Location,
        //                p.PropertyAddress.City,
        //                p.PropertyAddress.State,
        //                p.PropertyAddress.Country,
        //                p.PropertyAddress.FamousArea,
        //                p.PropertyAddress.SocietyName,
        //                p.PropertyAddress.Landmark,
        //                p.PropertyAddress.Latitude,
        //                p.PropertyAddress.Longitude
        //            },

        //            // ✅ PRICES
        //            Prices = p.PropertyPrices.Select(pp => new
        //            {
        //                pp.Amount,
        //                TransactionType = pp.TransactionType.TransactionTypeName
        //            }).ToList(), // ✅ convert to List

        //            // ✅ IMAGES
        //            Images = p.PropertyImages.Select(img => img.ImagePath).ToList(), // ✅ convert to List

        //            // ✅ AMENITIES
        //            Amenities = p.PropertyAmenities.Select(pa => pa.Amenity.AmenityName).ToList() // ✅ convert to List
        //        })
        //        .ToListAsync();

        //    return Ok(properties);
        //}
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            var query = _db.Properties
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.PropertyAddress)
                .Include(p => p.PropertyPrices).ThenInclude(pp => pp.TransactionType)
                .Include(p => p.PropertyImages)
                .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
                .AsQueryable();

            int totalCount = await query.CountAsync();

            var pagedItems = await query          // ← renamed from "properties" to "pagedItems"
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.PropertyId,
                    p.Title,
                    p.Description,
                    p.AreaSqft,
                    p.Status,
                    p.RequireType,
                    p.CreatedAt,
                    User = new { p.User.UserId, p.User.Name },
                    Category = new { p.Category.CategoryId, p.Category.CategoryName },
                    Address = new
                    {
                        p.PropertyAddress.Location,
                        p.PropertyAddress.City,
                        p.PropertyAddress.State,
                        p.PropertyAddress.Country,
                        p.PropertyAddress.FamousArea,
                        p.PropertyAddress.SocietyName,
                        p.PropertyAddress.Landmark,
                        p.PropertyAddress.Latitude,
                        p.PropertyAddress.Longitude
                    },
                    Prices = p.PropertyPrices.Select(pp => new
                    {
                        pp.Amount,
                        TransactionType = pp.TransactionType.TransactionTypeName
                    }).ToList(),
                    Images = p.PropertyImages.Select(img => img.ImagePath).ToList(),
                    Amenities = p.PropertyAmenities.Select(pa => pa.Amenity.AmenityName).ToList()
                })
                .ToListAsync();

            return Ok(new PagedResult<object>
            {
                Items = pagedItems,               // ← use pagedItems here
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
        [Authorize(Roles = "admin,Admin,buyer,Buyer,seller,Seller")]
        [HttpGet("{id}/similar")]
        public async Task<IActionResult> GetSimilarProperties(int id)
        {
            // Step 1: Get the current property to extract city + price + category
            var current = await _db.Properties
                .Include(p => p.PropertyAddress)
                .Include(p => p.PropertyPrices)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (current == null) return NotFound(new { message = "Property not found" });

            var city = current.PropertyAddress?.City;
            var categoryId = current.CategoryId;
            var currentPrice = current.PropertyPrices?.FirstOrDefault()?.Amount ?? 0;
            var minPrice = currentPrice * 0.8m;   // 20% below
            var maxPrice = currentPrice * 1.2m;   // 20% above

            // Step 2: Find similar properties matching at least one condition
            var similar = await _db.Properties
                .Include(p => p.Category)
                .Include(p => p.PropertyAddress)
                .Include(p => p.PropertyPrices)
                    .ThenInclude(pp => pp.TransactionType)
                .Include(p => p.PropertyImages)
                .Where(p =>
                    p.PropertyId != id &&           // not the current one
                    p.Status != "Cancelled" &&
                    p.PropertyAddress.City == city &&
                    (
                                 // same city
                         p.CategoryId == categoryId           // same type e.g Apartment
                        || p.PropertyPrices.Any(pr =>
                            pr.Amount >= minPrice &&
                            pr.Amount <= maxPrice)              // similar price ±20%
                    )
                )
                .Take(4)
                .ToListAsync();

            // Step 3: Return same shape your frontend already understands
            var result = similar.Select(p => new
            {
                p.PropertyId,
                p.Title,
                p.AreaSqft,
                p.Status,
                Category = new
                {
                    p.Category.CategoryId,
                    p.Category.CategoryName
                },
                Address = new
                {
                    p.PropertyAddress.City,
                    p.PropertyAddress.State,
                },
                Prices = p.PropertyPrices.Select(pp => new
                {
                    pp.Amount,
                    TransactionType = pp.TransactionType.TransactionTypeName
                }),
                Images = p.PropertyImages.Select(img => img.ImagePath)
            });

            return Ok(result);
        }
        [Authorize(Roles = "admin,Admin,buyer,Buyer,seller,Seller")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var property = await _db.Properties
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.PropertyAddress)
                .Include(p => p.BHK)
                .Include(p => p.PropertyPrices)
                    .ThenInclude(pp => pp.TransactionType)
                .Include(p => p.PropertyImages)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound(new { message = "Property not found" });

            var result = new
            {
                property.PropertyId,
                property.Title,
                property.Description,
                property.AreaSqft,
                property.Status,
                property.RequireType,
                property.CreatedAt,

                // ✅ USER
                User = new
                {
                    property.User.UserId,
                    property.User.Name
                },

                // ✅ CATEGORY
                Category = new
                {
                    property.Category.CategoryId,
                    property.Category.CategoryName
                },

                // ✅ ADDRESS
                Address = new
                {
                    property.PropertyAddress.Location,
                    property.PropertyAddress.City,
                    property.PropertyAddress.State,
                    property.PropertyAddress.Country,
                    property.PropertyAddress.FamousArea,
                    property.PropertyAddress.SocietyName,
                    property.PropertyAddress.Landmark,
                    property.PropertyAddress.Latitude,
                    property.PropertyAddress.Longitude


                },

                // ✅ BHK
                BHK = new
                {
                    property.BHK.BHKType,
                    property.BHK.TotalWashrooms
                },

                // ✅ PRICES
                Prices = property.PropertyPrices.Select(pp => new
                {
                    pp.Amount,
                    TransactionType = pp.TransactionType.TransactionTypeName
                }),

                // ✅ IMAGES
                Images = property.PropertyImages.Select(img => img.ImagePath),

                // ✅ AMENITIES
                Amenities = property.PropertyAmenities.Select(pa => new
                {
                    AmenityId = pa.Amenity.AmenityId,
                    AmenityName = pa.Amenity.AmenityName
                }),
            };

            return Ok(result);
        }


        // POST: api/properties
        [HttpPost]
        [Authorize(Roles = "seller,Seller,admin,Admin")]

        public async Task<IActionResult> Create([FromForm] PropertyCreateDto dto)
        {
            //var property = new Property
            //{
            //    Title = dto.Title,
            //    Description = dto.Description,
            //    AreaSqft = dto.AreaSqft,
            //    Status = dto.Status,
            //    RequireType = dto.RequireType,
            //    UserId = dto.UserId,
            //    CategoryId = dto.CategoryId,
            //    PropertyAddressId = dto.PropertyAddressId
            //};

            //_db.Properties.Add(property);
            //await _db.SaveChangesAsync();
            var address = new PropertyAddress
            {
                Location = dto.Address.Location,
                City = dto.Address.City,
                State = dto.Address.State,
                Country = dto.Address.Country,
                SocietyName = dto.Address.SocietyName,
                Landmark = dto.Address.Landmark,
                FamousArea = dto.Address.FamousArea,
                Latitude = dto.Address.Latitude,
                Longitude = dto.Address.Longitude


            };

            _db.PropertyAddresses.Add(address);
            await _db.SaveChangesAsync();
            var property = new Property
            {
                Title = dto.Title,
                Description = dto.Description,
                AreaSqft = dto.AreaSqft,
                Status = dto.Status,
                RequireType = dto.RequireType,
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                PropertyAddressId = address.PropertyAddressId
            };

            _db.Properties.Add(property);
            await _db.SaveChangesAsync();

            var bhk = new BHK
            {
                PropertyId = property.PropertyId,
                BHKType = dto.BHKType,
                TotalWashrooms = dto.TotalWashrooms
            };

            _db.BHKs.Add(bhk);
            await _db.SaveChangesAsync();

            // Add prices
            if (dto.SalePrice.HasValue)
            {
                _db.PropertyPrices.Add(new PropertyPrice
                {
                    PropertyId = property.PropertyId,
                    TransactionTypeId = 2, // Sale
                    Amount = dto.SalePrice.Value
                });
            }

            if (dto.RentPrice.HasValue)
            {
                _db.PropertyPrices.Add(new PropertyPrice
                {
                    PropertyId = property.PropertyId,
                    TransactionTypeId = 1, // Rent
                    Amount = dto.RentPrice.Value
                });
            }

            // 3. Add PropertyImage if provided
            // 3. Add PropertyImages if provided
            //if (dto.ImagePaths != null && dto.ImagePaths.Any())
            //{
            //    foreach (var path in dto.ImagePaths)
            //    {
            //        _db.PropertyImages.Add(new PropertyImage
            //        {
            //            PropertyId = property.PropertyId,
            //            ImagePath = path
            //        });
            //    }
            //}
            if (dto.Images != null && dto.Images.Count > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                foreach (var file in dto.Images)
                {
                    if (file.Length > 0)
                    {
                        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        string filePath = Path.Combine(folder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        _db.PropertyImages.Add(new PropertyImage
                        {
                            PropertyId = property.PropertyId,
                            ImagePath = fileName
                        });
                    }
                }
            }
            // 4. Add PropertyAmenities if provided
            if (dto.AmenityIds != null && dto.AmenityIds.Any())
            {
                foreach (var amenityId in dto.AmenityIds)
                {
                    // Optional: check if Amenity exists
                    var exists = await _db.Amenities.AnyAsync(a => a.AmenityId == amenityId);
                    if (exists)
                    {
                        _db.PropertyAmenities.Add(new PropertyAmenity
                        {
                            PropertyId = property.PropertyId,
                            AmenityId = amenityId
                        });
                    }
                }
            }

            //await _db.SaveChangesAsync();

            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = property.PropertyId },
                property.PropertyId);
        }

        // DELETE: api/properties/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "seller,Seller,admin,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _db.Properties
                .Include(p => p.PropertyPrices)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound(new { message = "Property not found" });

            // Remove child records first
            _db.PropertyPrices.RemoveRange(property.PropertyPrices);
            _db.Properties.Remove(property);

            await _db.SaveChangesAsync();
            return Ok(new { message = "Property deleted successfully" });
        }
        // PUT: api/properties/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromForm] PropertyCreateDto dto)
        //{
        //    var property = await _db.Properties
        //        .Include(p => p.PropertyAddress)
        //        .Include(p => p.PropertyPrices)
        //        .Include(p => p.PropertyAmenities)
        //        .Include(p => p.PropertyImages)
        //        .Include(p => p.BHK)
        //        .FirstOrDefaultAsync(p => p.PropertyId == id);

        //    if (property == null)
        //        return NotFound(new { message = "Property not found" });

        //    // ===== Update Address =====
        //    if (property.PropertyAddress != null)
        //    {
        //        property.PropertyAddress.Location = dto.Address.Location;
        //        property.PropertyAddress.City = dto.Address.City;
        //        property.PropertyAddress.State = dto.Address.State;
        //        property.PropertyAddress.Country = dto.Address.Country;
        //        property.PropertyAddress.Latitude = dto.Address.Latitude;
        //        property.PropertyAddress.Longitude = dto.Address.Longitude;

        //    }

        //    // ===== Update Property =====
        //    property.Title = dto.Title;
        //    property.Description = dto.Description;
        //    property.AreaSqft = dto.AreaSqft;
        //    property.Status = dto.Status;
        //    property.RequireType = dto.RequireType;
        //    property.UserId = dto.UserId;
        //    property.CategoryId = dto.CategoryId;

        //    // ===== Update BHK =====
        //    if (property.BHK != null)
        //    {
        //        property.BHK.BHKType = dto.BHKType;
        //        property.BHK.TotalWashrooms = dto.TotalWashrooms;
        //    }
        //    else
        //    {
        //        _db.BHKs.Add(new BHK
        //        {
        //            PropertyId = property.PropertyId,
        //            BHKType = dto.BHKType,
        //            TotalWashrooms = dto.TotalWashrooms
        //        });
        //    }

        //    // ===== Update Sale Price =====
        //    var salePrice = property.PropertyPrices
        //        .FirstOrDefault(p => p.TransactionTypeId == 2);

        //    if (dto.SalePrice.HasValue)
        //    {
        //        if (salePrice == null)
        //        {
        //            _db.PropertyPrices.Add(new PropertyPrice
        //            {
        //                PropertyId = property.PropertyId,
        //                TransactionTypeId = 2,
        //                Amount = dto.SalePrice.Value
        //            });
        //        }
        //        else
        //        {
        //            salePrice.Amount = dto.SalePrice.Value;
        //        }
        //    }

        //    // ===== Update Rent Price =====
        //    var rentPrice = property.PropertyPrices
        //        .FirstOrDefault(p => p.TransactionTypeId == 1);

        //    if (dto.RentPrice.HasValue)
        //    {
        //        if (rentPrice == null)
        //        {
        //            _db.PropertyPrices.Add(new PropertyPrice
        //            {
        //                PropertyId = property.PropertyId,
        //                TransactionTypeId = 1,
        //                Amount = dto.RentPrice.Value
        //            });
        //        }
        //        else
        //        {
        //            rentPrice.Amount = dto.RentPrice.Value;
        //        }
        //    }

        //    // ===== Update Images (Replace Old) =====
        //    if (dto.Images != null && dto.Images.Count > 0)
        //    {
        //        _db.PropertyImages.RemoveRange(property.PropertyImages);

        //        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        //        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        //        foreach (var file in dto.Images)
        //        {
        //            if (file.Length > 0)
        //            {
        //                string fileName = $"{Guid.NewGuid()}_{file.FileName}";
        //                string filePath = Path.Combine(folder, fileName);

        //                using var stream = new FileStream(filePath, FileMode.Create);
        //                await file.CopyToAsync(stream);

        //                _db.PropertyImages.Add(new PropertyImage
        //                {
        //                    PropertyId = property.PropertyId,
        //                    ImagePath = fileName
        //                });
        //            }
        //        }
        //    }

        //    // ===== Update Amenities (Replace Old) =====
        //    if (dto.AmenityIds != null && dto.AmenityIds.Any())
        //    {
        //        _db.PropertyAmenities.RemoveRange(property.PropertyAmenities);

        //        foreach (var amenityId in dto.AmenityIds)
        //        {
        //            bool exists = await _db.Amenities.AnyAsync(a => a.AmenityId == amenityId);
        //            if (exists)
        //            {
        //                _db.PropertyAmenities.Add(new PropertyAmenity
        //                {
        //                    PropertyId = property.PropertyId,
        //                    AmenityId = amenityId
        //                });
        //            }
        //        }
        //    }

        //    await _db.SaveChangesAsync();

        //    return Ok(new { message = "Property updated successfully" });
        //}
        // Add this method to your PropertyController.cs

        [Authorize(Roles = "seller,Seller,admin,Admin")]
        [HttpGet("my-properties")]
        public async Task<IActionResult> GetMyProperties()
        {
            // Get the current user's ID from the JWT token
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token" });
            }

            var properties = await _db.Properties
                .Where(p => p.UserId == userId) // Filter by current user
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.PropertyAddress)
                .Include(p => p.PropertyPrices)
                    .ThenInclude(pp => pp.TransactionType)
                .Include(p => p.PropertyImages)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Select(p => new
                {
                    p.PropertyId,
                    p.Title,
                    p.Description,
                    p.AreaSqft,
                    p.Status,
                    p.RequireType,
                    p.CreatedAt,

                    // ✅ USER
                    User = new
                    {
                        p.User.UserId,
                        p.User.Name
                    },

                    // ✅ CATEGORY
                    Category = new
                    {
                        p.Category.CategoryId,
                        p.Category.CategoryName
                    },

                    // ✅ ADDRESS
                    Address = new
                    {
                        p.PropertyAddress.Location,
                        p.PropertyAddress.City,
                        p.PropertyAddress.State,
                        p.PropertyAddress.Country,
                        p.PropertyAddress.FamousArea,
                        p.PropertyAddress.SocietyName,
                        p.PropertyAddress.Landmark,
                        p.PropertyAddress.Latitude,
                        p.PropertyAddress.Longitude
                    },

                    // ✅ PRICES
                    Prices = p.PropertyPrices.Select(pp => new
                    {
                        pp.Amount,
                        TransactionType = pp.TransactionType.TransactionTypeName
                    }).ToList(),

                    // ✅ IMAGES
                    Images = p.PropertyImages.Select(img => img.ImagePath).ToList(),

                    // ✅ AMENITIES
                    Amenities = p.PropertyAmenities.Select(pa => pa.Amenity.AmenityName).ToList()
                })
                .ToListAsync();

            return Ok(properties);
        }
        // Add this to your PropertyController.cs or AuthController.cs
        [Authorize]
        [HttpGet("debug/token-claims")]
        public IActionResult GetTokenClaims()
        {
            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            return Ok(new
            {
                message = "Current token claims",
                claims = claims
            });
        }
        [Authorize(Roles ="admin,Admin,seller,Seller")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] PropertyUpdateDto dto)
        {
            var property = await _db.Properties
                .Include(p => p.PropertyAddress)
                .Include(p => p.PropertyPrices)
                .Include(p => p.PropertyAmenities)
                .Include(p => p.PropertyImages)
                .Include(p => p.BHK)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound(new { message = "Property not found" });

            // ===== Update Address =====
            if (property.PropertyAddress != null)
            {
                property.PropertyAddress.Location = dto.Address.Location;
                property.PropertyAddress.City = dto.Address.City;
                property.PropertyAddress.State = dto.Address.State;
                property.PropertyAddress.Country = dto.Address.Country;
                property.PropertyAddress.SocietyName = dto.Address.SocietyName;
                property.PropertyAddress.Landmark = dto.Address.Landmark;
                property.PropertyAddress.FamousArea = dto.Address.FamousArea;
                property.PropertyAddress.Latitude = dto.Address.Latitude;
                property.PropertyAddress.Longitude = dto.Address.Longitude;
            }

            // ===== Update Property =====
            property.Title = dto.Title;
            property.Description = dto.Description;
            property.AreaSqft = dto.AreaSqft;
            property.Status = dto.Status;
            property.RequireType = dto.RequireType;
            property.UserId = dto.UserId;
            property.CategoryId = dto.CategoryId;

            // Note: We don't store TransactionTypeId on Property table
            // It's determined by which prices exist in PropertyPrices table

            // ===== Update BHK =====
            if (property.BHK != null)
            {
                property.BHK.BHKType = dto.BHKType;
                property.BHK.TotalWashrooms = dto.TotalWashrooms;
            }
            else
            {
                _db.BHKs.Add(new BHK
                {
                    PropertyId = property.PropertyId,
                    BHKType = dto.BHKType,
                    TotalWashrooms = dto.TotalWashrooms
                });
            }

            // ===== Update Prices Based on TransactionTypeId =====
            // Get existing prices
            var existingSalePrice = property.PropertyPrices.FirstOrDefault(p => p.TransactionTypeId == 2);
            var existingRentPrice = property.PropertyPrices.FirstOrDefault(p => p.TransactionTypeId == 1);

            // Determine what prices to keep based on TransactionTypeId
            // 1 = Rent, 2 = Sale, 3 = Both

            if (dto.TransactionTypeId == 1) // Rent only
            {
                // Remove sale price if exists
                if (existingSalePrice != null)
                {
                    _db.PropertyPrices.Remove(existingSalePrice);
                }

                // Add or update rent price
                if (dto.RentPrice.HasValue && dto.RentPrice.Value > 0)
                {
                    if (existingRentPrice == null)
                    {
                        _db.PropertyPrices.Add(new PropertyPrice
                        {
                            PropertyId = property.PropertyId,
                            TransactionTypeId = 1,
                            Amount = dto.RentPrice.Value
                        });
                    }
                    else
                    {
                        existingRentPrice.Amount = dto.RentPrice.Value;
                    }
                }
            }
            else if (dto.TransactionTypeId == 2) // Sale only
            {
                // Remove rent price if exists
                if (existingRentPrice != null)
                {
                    _db.PropertyPrices.Remove(existingRentPrice);
                }

                // Add or update sale price
                if (dto.SalePrice.HasValue && dto.SalePrice.Value > 0)
                {
                    if (existingSalePrice == null)
                    {
                        _db.PropertyPrices.Add(new PropertyPrice
                        {
                            PropertyId = property.PropertyId,
                            TransactionTypeId = 2,
                            Amount = dto.SalePrice.Value
                        });
                    }
                    else
                    {
                        existingSalePrice.Amount = dto.SalePrice.Value;
                    }
                }
            }
            else if (dto.TransactionTypeId == 3) // Both
            {
                // Add or update rent price
                if (dto.RentPrice.HasValue && dto.RentPrice.Value > 0)
                {
                    if (existingRentPrice == null)
                    {
                        _db.PropertyPrices.Add(new PropertyPrice
                        {
                            PropertyId = property.PropertyId,
                            TransactionTypeId = 1,
                            Amount = dto.RentPrice.Value
                        });
                    }
                    else
                    {
                        existingRentPrice.Amount = dto.RentPrice.Value;
                    }
                }

                // Add or update sale price
                if (dto.SalePrice.HasValue && dto.SalePrice.Value > 0)
                {
                    if (existingSalePrice == null)
                    {
                        _db.PropertyPrices.Add(new PropertyPrice
                        {
                            PropertyId = property.PropertyId,
                            TransactionTypeId = 2,
                            Amount = dto.SalePrice.Value
                        });
                    }
                    else
                    {
                        existingSalePrice.Amount = dto.SalePrice.Value;
                    }
                }
            }

            // ===== Update Images (Only if new images provided) =====
            if (dto.Images != null && dto.Images.Count > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Delete old image files from disk
                foreach (var img in property.PropertyImages)
                {
                    string oldPath = Path.Combine(folder, img.ImagePath);
                    if (System.IO.File.Exists(oldPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delete image {img.ImagePath}: {ex.Message}");
                        }
                    }
                }

                // Remove old image records from database
                _db.PropertyImages.RemoveRange(property.PropertyImages);

                // Add new images
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                foreach (var file in dto.Images)
                {
                    if (file.Length > 0)
                    {
                        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        string filePath = Path.Combine(folder, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        _db.PropertyImages.Add(new PropertyImage
                        {
                            PropertyId = property.PropertyId,
                            ImagePath = fileName
                        });
                    }
                }
            }

            // ===== Update Amenities (Only if amenities provided) =====
            if (dto.AmenityIds != null && dto.AmenityIds.Any())
            {
                _db.PropertyAmenities.RemoveRange(property.PropertyAmenities);

                foreach (var amenityId in dto.AmenityIds)
                {
                    bool exists = await _db.Amenities.AnyAsync(a => a.AmenityId == amenityId);
                    if (exists)
                    {
                        _db.PropertyAmenities.Add(new PropertyAmenity
                        {
                            PropertyId = property.PropertyId,
                            AmenityId = amenityId
                        });
                    }
                }
            }

            try
            {
                await _db.SaveChangesAsync();
                return Ok(new { message = "Property updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                return StatusCode(500, new
                {
                    message = "Error updating property",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }


    }
