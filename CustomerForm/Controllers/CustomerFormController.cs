using Azure.Core;
using CustomerForm.Data;
using CustomerForm.Model;
using CustomerForm.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace CustomerForm.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/CustomerForm")]
    [ApiController]
    public class CustomerFormController : ControllerBase
    {
        private readonly CustomerFormDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerFormController(CustomerFormDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetClientIp()
        {
            var webClient = new WebClient();
            string ipAddress = webClient.DownloadString("http://checkip.dyndns.org");
            ipAddress = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(ipAddress).Value;
            webClient.Dispose();

            return ipAddress;
        }
        [HttpPost]
        public ActionResult<CustomerFormDto> CreateCustomer([FromBody] CustomerFormDto formDto, Guid guid, string TableName, string ResourceUrl)
        {
            if (_db.CustomerForm.FirstOrDefault(u => u.FullName.ToLower() == formDto.FullName.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Customer Already Exist!");
                return BadRequest(ModelState);
            }
            if (formDto == null)
            {
                return BadRequest(formDto);
            }
            if (formDto.CustomerFormId > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //Guid guid = Guid.NewGuid();
            var ipAddress = GetClientIp(); // IpService kullanarak IP adresini al
            CustomerFormTbl model = new()
            {
                CustomerFormId = formDto.CustomerFormId,
                FullName = formDto.FullName ?? string.Empty,
                Tel = formDto.Tel ?? string.Empty,
                Email = formDto.Email ?? string.Empty,
                Explantion = formDto.Explantion ?? string.Empty,
                Guid = guid,
                Kvkk = formDto.Kvkk,
                DateTime = DateTime.Now,
                IpAddress = ipAddress,
                TableName = TableName,
                Active = 1,
                CustomerResultId = 0,
                ResourceUrl = ResourceUrl,
                SpeColumn1 = formDto.SpeColumn1,
                SpeColumn2 = formDto.SpeColumn2,
                SpeColumn3 = formDto.SpeColumn3,
                SpeColumn4 = formDto.SpeColumn4,
                SpeColumn5 = formDto.SpeColumn5,
            };
            _db.CustomerForm.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetCustomerById", new { id = formDto.CustomerFormId }, formDto);
        }


        [HttpGet("{guid:Guid}/{tableName}", Name = "GetCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerFormTbl> GetCustomer(Guid guid, string tableName)
        {
            var customer = _db.CustomerForm.Where(x => x.Guid == guid && x.TableName == tableName).ToList();
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }



        [HttpGet("{id:int}", Name = "GetCustomerById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerFormTbl> GetCustomerById(int id)
        {
            var customer = _db.CustomerForm.Where(x => x.CustomerFormId == id).FirstOrDefault();
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CustomerFormDto>> GetAllCustomer()
        {
            return Ok(_db.CustomerForm.Where(x => x.Active == 1).ToList());
        }



        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{id:int}")]
        public IActionResult DeleteCustomerForm(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingEntity = _db.CustomerForm.FirstOrDefault(u => u.CustomerFormId == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            // Varlığı güncellemeden önce, gerekli alanları güncelleyin
            existingEntity.Active = -1;
            existingEntity.CustomerResultId = 0;

            _db.CustomerForm.Update(existingEntity);
            _db.SaveChanges();

            return NoContent();
        }



        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{guid:Guid}/{TableName}/{CustomerFormId:int}/{CustomerResultId:int}")]
        public IActionResult UpdateCustomerForm([FromBody] CustomerFormDto formDto, Guid guid, string TableName, int CustomerFormId, int CustomerResultId)
        {
            if (guid == Guid.Empty || string.IsNullOrEmpty(TableName) || CustomerFormId <= 0)
            {
                return BadRequest();
            }

            var customerForm = _db.CustomerForm
                .FirstOrDefault(x => x.Guid == guid && x.CustomerFormId == CustomerFormId && x.TableName == TableName);

            if (customerForm == null)
            {
                return NotFound(); // Varsayılan olarak 404 hatası döndürdüm. İstediğiniz duruma göre değiştirebilirsiniz.
            }

            customerForm.FullName = formDto.FullName ?? customerForm.FullName;
            customerForm.Tel = formDto.Tel ?? customerForm.Tel;
            customerForm.Email = formDto.Email ?? customerForm.Email;
            customerForm.Explantion = formDto.Explantion ?? customerForm.Explantion;
            customerForm.Kvkk = formDto.Kvkk ? formDto.Kvkk : customerForm.Kvkk;
            customerForm.IpAddress = formDto.IpAddress ?? customerForm.IpAddress;
            customerForm.ResourceUrl = formDto.ResourceUrl ?? customerForm.ResourceUrl;
            customerForm.SpeColumn1 = formDto.SpeColumn1 ?? customerForm.SpeColumn1;
            customerForm.SpeColumn2 = formDto.SpeColumn2 ?? customerForm.SpeColumn2;
            customerForm.SpeColumn3 = formDto.SpeColumn3 ?? customerForm.SpeColumn3;
            customerForm.SpeColumn4 = formDto.SpeColumn4 ?? customerForm.SpeColumn4;
            customerForm.SpeColumn5 = formDto.SpeColumn5 ?? customerForm.SpeColumn5;

            // Diğer özellikleri de güncellediğinizden emin olun.

            _db.CustomerForm.Update(customerForm);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
