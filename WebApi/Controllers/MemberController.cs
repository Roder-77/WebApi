using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Exceptions;
using Models.Requests;
using Models.Responses;
using Models.ViewModels;
using Services;
using Swashbuckle.AspNetCore.Annotations;
using static Services.Extensions.PaginationExtension;

namespace WebApi.Controllers
{
    [SwaggerTag("會員")]
    public class MemberController : BaseController
    {
        private readonly MemberService _service;

        public MemberController(MemberService service)
        {
            _service = service;
        }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerSuccessResponse(typeof(Response<MemberVM>))]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            Response200.Data = await _service.Get(id) ?? throw new NotFoundException("查無資料");
            return Ok(Response200);
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerSuccessResponse(typeof(Response<PaginationList<MemberVM>>))]
        public async Task<IActionResult> GetList([FromQuery] GetMembersRequest request)
        {
            Response200.Data = await _service.GetList(request.Page, request.PageSize);
            return Ok(Response200);
        }

        /// <summary>
        /// 取得會員選單
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <param name="menuId">選單代碼</param>
        /// <returns></returns>
        [HttpGet("{id}/menu")]
        [SwaggerSuccessResponse(typeof(Response<PaginationList<MemberVM>>))]
        public async Task<IActionResult> GetMenu(int id, [FromQuery] int menuId)
        {
            Response200.Data = await _service.GetMenu(id, menuId);
            return Ok(Response200);
        }
    }
}
