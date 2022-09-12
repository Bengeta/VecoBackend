using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Models;
using VecoBackend.Requests;
using VecoBackend.Responses;
using VecoBackend.Services;

namespace VecoBackend.Controllers;

[ApiController]
[Authorize]
public class MaterialController : BaseController
{
    private readonly MaterialService _materialService;

    public MaterialController(MaterialService materialService, ApplicationContext context)
    {
        _materialService = materialService;
        materialService.AddContext(context);
    }

    [HttpGet]
    [Route("/materials/pages/{page}/{pageSize}")]
    public async Task<ResponseModel<PaginatedListModel<MaterialResponse>>> GetMaterials(int page,int pageSize)
    {
        var materials = await _materialService.GetMaterials(page,pageSize);
        if (materials == null)
            return new ResponseModel<PaginatedListModel<MaterialResponse>> {ResultCode = ResultCode.Failed};
        return new ResponseModel<PaginatedListModel<MaterialResponse>>
            {ResultCode = ResultCode.Success, Data = materials};
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
    [Route("/materials/{category}/pages/{page}/{pageSize}")]
    public async Task<ResponseModel<PaginatedListModel<MaterialResponse>>> GetMaterialsByCategory(MaterialCategory category,
        int page,int pageSize)
    {
        var materials = await _materialService.GetMaterialsByCategory(category, page,pageSize);
        if (materials == null)
            return new ResponseModel<PaginatedListModel<MaterialResponse>> {ResultCode = ResultCode.Failed};
        return new ResponseModel<PaginatedListModel<MaterialResponse>>
            {ResultCode = ResultCode.Success, Data = materials};
    }
}