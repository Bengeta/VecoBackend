using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VecoBackend.Data;
using VecoBackend.Enums;
using VecoBackend.Interfaces;
using VecoBackend.Models;
using VecoBackend.Responses;

namespace VecoBackend.Controllers;

[ApiController]
[Authorize]
public class MaterialController : BaseController
{
    private readonly IMaterialService _materialService;
    

    public MaterialController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    [Route("/materials/pages/{page}/{pageSize}")]
    public async Task<ResponseModel<PaginatedListModel<MaterialResponse>>> GetMaterials(int page,int pageSize)
    {
        var materials = await _materialService.GetMaterials(page,pageSize,BaseUrl());
        if (materials == null)
            return new ResponseModel<PaginatedListModel<MaterialResponse>> {ResultCode = ResultCode.Failed};
        return new ResponseModel<PaginatedListModel<MaterialResponse>>
            {ResultCode = ResultCode.Success, Data = materials};
    }

    [HttpGet]
    [Route("/materials/{id}")]
    public async Task<ResponseModel<MaterialResponse>> GetMaterial(int id)
    {
        var material = await _materialService.GetMaterial(id,BaseUrl());
        if (material == null)
            return new ResponseModel<MaterialResponse> {ResultCode = ResultCode.Failed};
        return new ResponseModel<MaterialResponse> {ResultCode = ResultCode.Success, Data = material};
    }

    [HttpGet]
    [Route("/materials/{category}/pages/{page}/{pageSize}")]
    public async Task<ResponseModel<PaginatedListModel<MaterialResponse>>> GetMaterialsByCategory(MaterialCategory category,
        int page,int pageSize)
    {
        var materials = await _materialService.GetMaterialsByCategory(category, page,pageSize,BaseUrl());
        if (materials == null)
            return new ResponseModel<PaginatedListModel<MaterialResponse>> {ResultCode = ResultCode.Failed};
        return new ResponseModel<PaginatedListModel<MaterialResponse>>
            {ResultCode = ResultCode.Success, Data = materials};
    }
}