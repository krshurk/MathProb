using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using System.Security.Claims;

namespace WebApplication.Controllers
{
    public class MaterialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Theory()
        {
            var materials = await _context.LearningMaterials
                .Include(m => m.Topic)
                .Include(m => m.Type)
                .OrderBy(m => m.TopicId)
                .ToListAsync();

            return View(materials);
        }

        public async Task<IActionResult> Details(int id)
        {
            var material = await _context.LearningMaterials
                .Include(m => m.Topic)
                .Include(m => m.Type)
                .Include(m => m.Comments.Where(c => c.UserId == GetCurrentUserId()))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int materialId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Комментарий не может быть пустым");
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                UserId = userId,
                LearningMaterialId = materialId,
                Text = text
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = materialId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = comment.LearningMaterialId });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return 0;
            }

            return int.Parse(userIdClaim.Value);
        }
    }
}