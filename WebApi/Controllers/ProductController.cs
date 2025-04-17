using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Exceptions;
using Models.Responses;
using Models.ViewModels;
using Services;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers
{
    [SwaggerTag("商品")]
    public class ProductController : BaseController
    {
        private readonly CallApiService _callApiservice;
        private readonly ProductService _service;

        public ProductController(
            CallApiService callApiService,
            ProductService service)
        {
            _callApiservice = callApiService;
            _service = service;
        }

        /// <summary>
        /// 取得商品
        /// </summary>
        /// <param name="id">商品代碼</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        [HttpGet("{id}")]
        [SwaggerSuccessResponse(typeof(Response<ProductVM>))]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _service.Get(id) ?? throw new NotFoundException("查無資料");
            var (isSuccess, response) = await _callApiservice.Get<Response<IEnumerable<string>>>($"{Request.Scheme}://{Request.Host}/api/v1/products/{id}/tags");

            if (isSuccess)
            {
                product.Tags = response?.Data ?? [];
            }

            Response200.Data = product;
            return Ok(Response200);
        }

        /// <summary>
        /// 取得商品標籤
        /// </summary>
        /// <param name="id">商品代碼</param>
        /// <returns></returns>
        [HttpGet("{id}/tags")]
        [SwaggerSuccessResponse(typeof(Response<IEnumerable<string>>))]
        public IActionResult GetTags(int id)
        {
            Response200.Data = new [] { "標籤1", "標籤2", "標籤3" };
            return Ok(Response200);
        }
    }
}
