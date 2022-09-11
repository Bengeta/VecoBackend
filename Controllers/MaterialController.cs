using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Responses;
using VecoBackend.Services;

namespace VecoBackend.Controllers;
[ApiController]
[Authorize]
public class MaterialController : BaseController
{
    private readonly MaterialService _materialService;

    public MaterialController(MaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    [Route("/materials")]
    public async Task<ResponseModel<List<MaterialResponse>>> GetMaterials()
    {
        var materials = await _materialService.GetMaterials();
        if (materials == null)
            return new ResponseModel<List<MaterialResponse>> {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<MaterialResponse>> {ResultCode = ResultCode.Success, Data = materials};
    }

    [HttpGet]
    [Route("/materials/{id}")]
    public async Task<ResponseModel<MaterialResponse>> GetMaterial(int id)
    {
        var material = await _materialService.GetMaterial(id);
        if (material == null)
            return new ResponseModel<MaterialResponse> {ResultCode = ResultCode.Failed};
        return new ResponseModel<MaterialResponse> {ResultCode = ResultCode.Success, Data = material};
    }

    [HttpGet]
    [Route("/materials/{category}")]
    public async Task<ResponseModel<List<MaterialResponse>>> GetMaterialsByCategory(MaterialCategory category)
    {
        var materials = await _materialService.GetMaterialsByCategory(category);
        if (materials == null)
            return new ResponseModel<List<MaterialResponse>> {ResultCode = ResultCode.Failed};
        return new ResponseModel<List<MaterialResponse>> {ResultCode = ResultCode.Success, Data = materials};
    }
}