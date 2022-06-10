using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;
using Models.ViewModel;
using Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using static Models.Extensions.PaginationExtension;

namespace WebApi.Controllers
{
    [Route("api/v1")]
    public class MemberController : BaseController
    {
        private readonly IMemberService _service;

        public MemberController(IMemberService service)
        {
            _service = service;
        }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        [HttpGet("member/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, ok, typeof(Response<MemberVM>))]
        public IActionResult GetMember([FromRoute] int id)
        {
            var member = _service.GetMember(id);

            if (member == null)
                return NotFound(Response404);

            Response200.Data = member;

            return Ok(Response200);
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <returns></returns>
        [HttpPost("member")]
        [SwaggerResponse((int)HttpStatusCode.OK, ok, typeof(Response<object>))]
        public async Task<IActionResult> InsertMember([FromBody] InsertMemberRequest request)
        {
            await _service.InsertMember(request);

            return Ok(Response200);
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpPut("member")]
        [SwaggerResponse((int)HttpStatusCode.OK, ok, typeof(Response<object>))]
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberRequest request)
        {
            var model = new MemberVM
            {
                Id = request.Id,
                Name = request.Name,
                Mobile = request.Mobile,
                Email = request.Email
            };

            await _service.UpdateMember(model);

            return Ok(Response200);
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpGet("members")]
        [SwaggerResponse((int)HttpStatusCode.OK, ok, typeof(Response<PaginationList<MemberVM>>))]
        public IActionResult GetMembers([FromQuery] GetMembersRequest request)
        {
            Response200.Data = _service.GetMembers(request.Page, request.PageSize);

            return Ok(Response200);
        }
    }
}
