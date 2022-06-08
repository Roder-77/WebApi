using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.Request;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/v1")]
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
        [HttpGet("member/{id}")]
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
        /// 新增會員
        /// </summary>
        /// <returns></returns>
        [HttpPost("member")]
        public IActionResult InsertMember([FromBody] InsertMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage);

                _logger.LogError(string.Join(", ", errors));

                return BadRequest();
            }

            var model = new MemberModel
            {
                Name = request.Name,
                Mobile = request.Mobile,
                Email = request.Email
            };

            _service.InsertMember(model);

            return Ok();
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpPut("member")]
        public IActionResult UpdateMember([FromBody] UpdateMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage);

                _logger.LogError(string.Join(", ", errors));

                return BadRequest();
            }

            var model = new MemberModel
            {
                Id = request.Id,
                Name = request.Name,
                Mobile = request.Mobile,
                Email = request.Email
            };

            _service.UpdateMember(model);

            return Ok();
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <returns></returns>
        [HttpPost("members")]
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

            var result = members.Select(m => new MemberModel(m)).ToList();

            return Ok(result);
        }
    }
}
