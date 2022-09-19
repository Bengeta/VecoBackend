using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Requests;
using VecoBackend.Responses;

namespace VecoBackend.Interfaces;

public interface IMaterialService
{
    public Task<bool> CreateMaterial(MaterialCreateRequest material);
    public Task<bool> UpdateMaterial(MaterialUpdateRequest material,bool isPictureChanged);
    public Task<bool> DeleteMaterial(int id);
    public Task<MaterialResponse> GetMaterial(int id, string baseUrl="");
    public Task<PaginatedListModel<MaterialResponse>> GetMaterials(int page, int pageSize, string baseUrl);

    public Task<PaginatedListModel<MaterialResponse>> GetMaterialsByCategory(MaterialCategory category, int page,
        int pageSize, string baseUrl);
}