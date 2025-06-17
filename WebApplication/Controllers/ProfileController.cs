using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using System.Security.Claims;

namespace WebApplication.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем ID текущего пользователя
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Получаем общее количество упражнений
            var totalExercises = await _context.Exercises.CountAsync();

            // Получаем количество уникальных правильно решенных упражнений
            var completedExercises = await _context.ExerciseAttempts
                .Where(ea => ea.UserId == userId && ea.IsCorrect)
                .Select(ea => ea.ExerciseId)
                .Distinct()
                .CountAsync();

            // Вычисляем процент прохождения курса
            var completionPercentage = totalExercises > 0 
                ? Math.Round((decimal)completedExercises / totalExercises * 100, 2) 
                : 0;

            var score = completedExercises * 3;

            var certificate = await _context.Certificates
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.IssueDate)
                .FirstOrDefaultAsync();

            ViewBag.HasCertificate = certificate != null;
            ViewBag.ExamId = certificate?.ExamId;

            var progress = await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (progress == null)
            {
                progress = new Progress
                {
                    UserId = userId,
                    CompletesExercises = completedExercises,
                    Score = score,
                    CourseCompletion = completionPercentage
                };
                _context.Progresses.Add(progress);
            }
            else
            {
                progress.CompletesExercises = completedExercises;
                progress.Score = score;
                progress.CourseCompletion = completionPercentage;
            }

            await _context.SaveChangesAsync();

            return View(progress);
        }
    }
} 