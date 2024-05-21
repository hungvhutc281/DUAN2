using BTL.Data;
using BTL.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private MyDbContext _context;

        public NhanVienController(MyDbContext context)
        {
            _context = context;
        }

//        Sử dụng Select() để chuyển đổi mỗi đối tượng nhân viên sang một đối tượng DTO(NhanVienDTO).
//Trong đó, từng thuộc tính của đối tượng nhân viên được gán cho từng thuộc tính tương ứng của đối tượng DTO.
//Đối tượng DTO sẽ chỉ chứa các thuộc tính cần thiết để truyền dữ liệu về cho client, không bao gồm các thuộc tính không cần thiết hoặc dữ liệu nhạy cảm.
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var nhanViens = _context.NhanViens
                .Include(nv => nv.CongViec)
                .Include(nv => nv.PhongBan)
                .ToList();

            var nhanVienDTOs = nhanViens.Select(nv => new NhanVienDTO
            {
                MaNV = nv.MaNV,
                HoTen = nv.HoTen,
                DiaChi = nv.DiaChi,
                DienThoai = nv.DienThoai,
                Email = nv.Email,
                NgaySinh = nv.NgaySinh,
                Luong = nv.Luong,
                Anh = nv.Anh,
                MaCongViec = nv.MaCongViec,
                MaPhongBan = nv.MaPhongBan,
                //kiểm tra null hay không toán tử ?
                CongViec = nv.CongViec?.TenCongViec,
                PhongBan = nv.PhongBan?.TenPhongBan
            });

            return Ok(nhanVienDTOs);
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var nv = _context.NhanViens
                .Include(n => n.CongViec)
                .Include(n => n.PhongBan)
                .SingleOrDefault(n => n.MaNV == id);

            if (nv != null)
            {
                var nvDTO = new NhanVienDTO
                {
                    MaNV = nv.MaNV,
                    HoTen = nv.HoTen,
                    DiaChi = nv.DiaChi,
                    DienThoai = nv.DienThoai,
                    Email = nv.Email,
                    NgaySinh = nv.NgaySinh,
                    Luong = nv.Luong,
                    Anh = nv.Anh,
                    CongViec = nv.CongViec?.TenCongViec,
                    PhongBan = nv.PhongBan?.TenPhongBan
                };

                return Ok(nvDTO);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("GetByName/{name}")]
        public IActionResult GetByName(string name)
        {
            var nvs = _context.NhanViens
                .Include(n => n.CongViec)
                .Include(n => n.PhongBan)
                .Where(n => n.HoTen.Contains(name))
                .Select(n => new NhanVienDTO
                {
                    MaNV = n.MaNV,
                    HoTen = n.HoTen,
                    DiaChi = n.DiaChi,
                    DienThoai = n.DienThoai,
                    Email = n.Email,
                    NgaySinh = n.NgaySinh,
                    Luong = n.Luong,
                    Anh = n.Anh,
                    CongViec = n.CongViec != null ? n.CongViec.TenCongViec : null,
                    PhongBan = n.PhongBan != null ? n.PhongBan.TenPhongBan : null
                })
                .ToList();

            if (nvs.Count > 0)
            {
                return Ok(nvs);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("Create")]
        public IActionResult Create(NhanVienModel model)
        {
            try
            {
                var nv = new NhanVien
                {
                    HoTen = model.HoTen,
                    DiaChi = model.DiaChi,
                    DienThoai = model.DienThoai,
                    Email = model.Email,
                    NgaySinh = model.NgaySinh,
                    MaCongViec = model.MaCongViec,
                    MaPhongBan = model.MaPhongBan,
                    Luong = model.Luong,
                    Anh = model.Anh,
                };
                _context.Add(nv);
                _context.SaveChanges();
                return Ok(nv);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("Update/{id}")]
        public IActionResult Edit(int id, NhanVienModel model)
        {
            // single default trả về đối tượng duy nhất
            var nv = _context.NhanViens.SingleOrDefault(l => l.MaNV == id);
            if (nv != null)
            {
                nv.HoTen = model.HoTen;
                nv.DiaChi = model.DiaChi;
                nv.DienThoai = model.DienThoai;
                nv.Email = model.Email;
                nv.NgaySinh = model.NgaySinh;
                nv.MaCongViec = model.MaCongViec;
                nv.MaPhongBan = model.MaPhongBan;
                nv.Luong = model.Luong;
                nv.Anh = model.Anh;
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var nv = _context.NhanViens.SingleOrDefault(l => l.MaNV == id);
            if (nv != null)
            {
                _context.NhanViens.Remove(nv);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _context.Admins.FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);
            if (user != null)
            {
                // Trả về ID của người dùng thành công
                return Ok(user.Id);
            }

            return BadRequest("Đăng nhập không thành công");
        }
        [HttpGet("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            // Ví dụ: Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = _context.Admins.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Trả về thông tin người dùng dưới dạng đối tượng JSON
            var userInfo = new
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return Ok(userInfo);
        }
    }
}
