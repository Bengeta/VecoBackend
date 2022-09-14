using Microsoft.EntityFrameworkCore;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Requests;
using VecoBackend.Responses;
using VecoBackend.Utils;

namespace VecoBackend.Services;

public class MaterialService
{
    private ApplicationContext _context;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public MaterialService(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    public void AddContext(ApplicationContext applicationContext)
    {
        _context = applicationContext;
    }

    public async Task<bool> CreateMaterial(MaterialCreateRequest material)
    {
        try
        {
            var newMaterial = new MaterialModel()
            {
                Title = material.Title,
                Description = material.Description,
                Author = material.Author,
                Date = material.Date,
                IsSeen = material.IsSeen,
                Category = material.Category,
            };
            await _context.MaterialModels.AddAsync(newMaterial);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> UpdateMaterial(MaterialUpdateRequest material)
    {
        try
        {
            var materialToUpdate = await _context.MaterialModels.FirstOrDefaultAsync(x => x.Id == material.Id);
            if (materialToUpdate == null)
            {
                return false;
            }

            materialToUpdate.Title = material.Title;
            materialToUpdate.Description = material.Description;
            materialToUpdate.Author = material.Author;
            materialToUpdate.Date = material.Date;
            materialToUpdate.IsSeen = material.IsSeen;
            materialToUpdate.Category = material.Category;
            _context.MaterialModels.Update(materialToUpdate);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> DeleteMaterial(int id)
    {
        try
        {
            var material = await _context.MaterialModels.FirstOrDefaultAsync(x => x.Id == id);
            if (material == null) return false;
            _context.MaterialModels.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<MaterialResponse> GetMaterial(int id,string baseUrl)
    {
        try
        {
            var Material = await (from material in _context.MaterialModels
                join materialImage in _context.MaterialImageModels on material.Id equals materialImage.MaterialId
                join image in _context.ImageStorageModels on materialImage.ImageId equals image.id
                where material.IsSeen == true && material.Id == id
                orderby material.Id
                select new
                {
                    material.Id,
                    material.Title,
                    material.Description,
                    material.Author,
                    material.Date,
                    material.Category,
                    image.imagePath,
                    image.imageType
                }).ToListAsync();
            var ans = new MaterialResponse()
            {
                Id = Material[0].Id,
                Title = Material[0].Title,
                Description = Material[0].Description,
                Author = Material[0].Author,
                Date = Converter.ToUnixTime(Material[0].Date),
                Category = Material[0].Category,
                imagePaths = Material.Select(x => Path.Combine(baseUrl, x.imagePath)).ToList(),
                imageTypes = Material.Select(x => x.imageType).ToList()
            };

            return ans;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<PaginatedListModel<MaterialResponse>> GetMaterials(int page,int pageSize,string baseUrl)
    {
        try
        {
            var materials = await (from material in _context.MaterialModels
                join materialImage in _context.MaterialImageModels on material.Id equals materialImage.MaterialId
                join Image in _context.ImageStorageModels on materialImage.ImageId equals Image.id
                where material.IsSeen == true
                orderby material.Id
                select new
                {
                    material.Id,
                    material.Title,
                    material.Description,
                    material.Author,
                    material.Date,
                    material.Category,
                    Image.imagePath,
                    Image.imageType
                }).ToListAsync();
            var list = new List<MaterialResponse>();
            var prevId = 0;
            foreach (var material in materials)
                if (material.Id == prevId)
                {
                    list[^1].imagePaths.Add(Path.Combine(baseUrl, material.imagePath));
                    list[^1].imageTypes.Add(material.imageType);
                }
                else
                {
                    list.Add(new MaterialResponse()
                    {
                        Id = material.Id,
                        Title = material.Title,
                        Description = material.Description,
                        Author = material.Author,
                        Date = Converter.ToUnixTime(material.Date),
                        Category = material.Category,
                        imagePaths = new List<string>()
                            {Path.Combine(baseUrl, material.imagePath)},
                        imageTypes = new List<ImageType>() {material.imageType}
                    });
                    prevId = material.Id;
                }

            return GetPagedMaterials(list, page,pageSize);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<PaginatedListModel<MaterialResponse>> GetMaterialsByCategory(MaterialCategory category,int page,int pageSize,string baseUrl)
    {
        try
        {
            var materials = await (from material in _context.MaterialModels
                join materialImage in _context.MaterialImageModels on material.Id equals materialImage.MaterialId
                join Image in _context.ImageStorageModels on materialImage.ImageId equals Image.id
                where material.IsSeen == true && material.Category == category
                orderby material.Id
                select new
                {
                    material.Id,
                    material.Title,
                    material.Description,
                    material.Author,
                    material.Date,
                    material.Category,
                    Image.imagePath,
                    Image.imageType
                }).ToListAsync();
            var list = new List<MaterialResponse>();
            var prevId = 0;
            foreach (var material in materials)
                if (material.Id == prevId)
                {
                    list[^1].imagePaths.Add(Path.Combine(baseUrl, material.imagePath));
                    list[^1].imageTypes.Add(material.imageType);
                }
                else
                {
                    list.Add(new MaterialResponse()
                    {
                        Id = material.Id,
                        Title = material.Title,
                        Description = material.Description,
                        Author = material.Author,
                        Date = Converter.ToUnixTime(material.Date),
                        Category = material.Category,
                        imagePaths = new List<string>()
                            {Path.Combine(baseUrl, material.imagePath)},
                        imageTypes = new List<ImageType>() {material.imageType}
                    });
                    prevId = material.Id;
                }

            return GetPagedMaterials(list, page,pageSize);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private PaginatedListModel<MaterialResponse> GetPagedMaterials(List<MaterialResponse> materials, int page,int pagesize)
    {
        var answer = PagedList<MaterialResponse>.ToPagedList(materials, page, pagesize);
        return new PaginatedListModel<MaterialResponse>()
        {
            countPage = answer.TotalPages, currentPage = answer.CurrentPage, data = answer.ToList(),
            isNext = answer.HasNext, isPrev = answer.HasPrevious
        };
    }
}