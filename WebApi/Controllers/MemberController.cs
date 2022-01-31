using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.Request;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _service;
        private readonly ILogger<MemberController> _logger;

        public MemberController(IMemberService service, ILogger<MemberController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        [HttpGet("/api/v1/member/{id}")]
        public IActionResult GetMember([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage);

                _logger.LogError(string.Join(", ", errors));

                return BadRequest();
            }

            var member = _service.GetMember(id);

            if (member == null)
                return NotFound();

            var result = new MemberModel(member);

            return Ok(result);
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpPost("/api/v1/members")]
        public IActionResult GetMembers([FromBody] GetMembersRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage);

                _logger.LogError(string.Join(", ", errors));

                return BadRequest();
            }

            var members = _service.GetMembers(request.Page, request.Quantity);

            if (!members.Any())
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpPut("/api/v1/member")]
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage);

                _logger.LogError(string.Join(", ", errors));

                return BadRequest();
            }

            await _service.UpdateMember(request);

            return Ok();
        }
    }
}
