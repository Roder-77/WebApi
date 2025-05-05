using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
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
            var member = await _service.Get(id);

            if (member == null)
                return NotFound(Response404);

            Response200.Data = member;

            return Ok(Response200);
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SwaggerSuccessResponse()]
        public async Task<IActionResult> Create([FromBody] InsertMemberRequest request)
        {
            await _service.Create(request);

            return Ok(Response200);
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerSuccessResponse()]
        public async Task<IActionResult> Update([FromBody] UpdateMemberRequest request)
        {
            await _service.Update(request);

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
    }
}
