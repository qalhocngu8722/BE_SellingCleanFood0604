using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.Repository;
using Microsoft.AspNetCore.Hosting;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.DTOs;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUsersRepo _usersRepo;
        private const string table_name = "users";
        public UsersController(IUsersRepo usersRepo, IWebHostEnvironment webHostEnvironment)
        {
            _usersRepo = usersRepo;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("Users/getbyid/{id}")]
        public async Task<ActionResult<users>> GetByIDAsync(int id)
        {
            var user = await _usersRepo.GetByIDAsync(id);
            if(user == null)
            {
                return NotFound("Không tìm thấy user");
            }
            return Ok(user);
        }
        [HttpGet("Users/Getall")]
        public async Task<ActionResult<List<users>>> GetAllAsync()
        {
            var listuser = await _usersRepo.GetAllAsync();
            if (listuser.Count == 0)
            {
                return NotFound("Danh sách user rỗng");
            }
            return Ok(listuser);
        }
        [HttpPost("Users/Addnew")]
        public async Task<ActionResult> AddnewAsync(users u)
        {
            var iduser = await _usersRepo.AddnewAsync(u);
            Console.WriteLine(u.role.ToString());
            if(iduser > 0)
            {
                u.id = iduser;
                return Ok("Thêm mới thành công");
            }
            return BadRequest("Thêm mới ng dùng không thành công");
        }
        [HttpPut("Users/UpdateUser")]
        public async Task<ActionResult<int>> UpdateAsync(UpdUserDTO u, int id)
        {
            users u1 = new users();
            u1.email = u.email ?? u1.email;
            u1.password = u.password ?? u1.password;
            u1.name = u.name ?? u1.name;
            u1.role = u.role ?? u1.role;
            var affected_row = await _usersRepo.UpdateAsync(u1, id);
            if (affected_row > 0)
            {
                return Ok("Sửa thành công");
            }
            return BadRequest("Sửa thông tin ng dùng không thành công");
        }
        [HttpDelete("Users/DeleteUser")]
        public async Task<ActionResult<int>> DeleteAsync(int id)
        {
            var affected_row = await _usersRepo.DeleteAsync(id);
            if (affected_row > 0)
            {
                return Ok("Xóa thành công");
            }
            return BadRequest("Xóa ng dùng không thành công");
        }

        [HttpPost("Users/Login")]
        public async Task<ActionResult<users>> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var user = await _usersRepo.Login(dto.email, dto.password);
                if (user == null)
                {
                    return Unauthorized("Email hoặc mật khẩu không đúng");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Users/SignUp")]
        public async Task<ActionResult> SignUp([FromBody] SignUpDTO dto)
        {
            try
            {
                await _usersRepo.SignUp(dto.name, dto.email, dto.password, dto.repeatPassword);
                return Ok("Đăng ký thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
