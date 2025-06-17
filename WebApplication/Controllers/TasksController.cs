using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;

namespace WebApplication.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CertificateService _certificateService;

        public TasksController(ApplicationDbContext context, CertificateService certificateService)
        {
            _context = context;
            _certificateService = certificateService;
        }

        public async Task<IActionResult> Index()
        {
            var topics = await _context.Topics
                .Include(t => t.Exercises)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return View(topics);
        }

        public async Task<IActionResult> Topic(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Exercises)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            var firstExercise = topic.Exercises.FirstOrDefault();
            if (firstExercise == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Exercise), new { topicId = id, exerciseId = firstExercise.Id });
        }

        public async Task<IActionResult> Exercise(int topicId, int exerciseId)
        {
            var exercise = await _context.Exercises
                .Include(e => e.Topic)
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.TopicId == topicId);

            if (exercise == null)
            {
                return NotFound();
            }

            var viewModel = new ExerciseViewModel
            {
                Exercise = exercise,
                IsLastExercise = !await _context.Exercises
                    .AnyAsync(e => e.TopicId == topicId && e.Id > exerciseId)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int topicId, int exerciseId, string answer)
        {
            var exercise = await _context.Exercises
                .Include(e => e.Topic)
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.TopicId == topicId);

            if (exercise == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Проверяем правильность ответа
            var isCorrect = answer.Trim().ToLower() == exercise.CorrectAnswer.Trim().ToLower();

            // Сохраняем попытку
            var attempt = new ExerciseAttempt
            {
                UserId = userId,
                ExerciseId = exerciseId,
                IsCorrect = isCorrect,
            };

            _context.ExerciseAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            // Получаем следующее упражнение
            var nextExercise = await _context.Exercises
                .Where(e => e.TopicId == topicId && e.Id > exerciseId)
                .OrderBy(e => e.Id)
                .FirstOrDefaultAsync();

            var viewModel = new ExerciseResultViewModel
            {
                Exercise = exercise,
                UserAnswer = answer,
                IsCorrect = isCorrect,
                NextExerciseId = nextExercise?.Id,
                IsLastExercise = nextExercise == null
            };

            return View(viewModel);
        }

        public IActionResult Theory()
        {
            return RedirectToAction("Theory", "Material");
        }

        public async Task<IActionResult> Exam()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Получаем прогресс пользователя
            var progress = await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (progress == null || progress.CourseCompletion < 70)
            {
                ViewBag.Message = "Экзамен пока недоступен. Для доступа к экзамену необходимо пройти более 70% курса.";
                ViewBag.CourseCompletion = progress?.CourseCompletion ?? 0;
                return View("ExamUnavailable");
            }

            return View();
        }

        public async Task<IActionResult> StartExam()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Проверяем прогресс пользователя
            var progress = await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (progress == null || progress.CourseCompletion < 70)
            {
                return RedirectToAction(nameof(Exam));
            }

            var questions = await _context.ExamQuestions
                .OrderBy(q => Guid.NewGuid())
                .Take(10)
                .ToListAsync();

            if (!questions.Any())
            {
                return RedirectToAction(nameof(Exam));
            }

            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(Dictionary<int, string> answers)
        {
            if (answers == null || !answers.Any())
            {
                return RedirectToAction(nameof(Exam));
            }

            // Получаем ID текущего пользователя
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            var questions = await _context.ExamQuestions
                .Where(q => answers.Keys.Contains(q.Id))
                .ToListAsync();

            int score = 0;
            foreach (var question in questions)
            {
                if (answers.TryGetValue(question.Id, out string userAnswer) &&
                    userAnswer.Trim().ToLower() == question.CorrectAnswer.Trim().ToLower())
                {
                    score += 10;
                }
            }

            // Сохраняем результат экзамена
            var exam = new Exam
            {
                UserId = userId,
                Score = score,
                TakenAt = DateTime.UtcNow
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            if (score >= 70)
            {
                var certificate = new Certificate
                {
                    UserId = userId,
                    ExamId = exam.Id,
                    IssueDate = DateTime.UtcNow,
                    CertificateCode = GenerateCertificateCode()
                };

                _context.Certificates.Add(certificate);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ExamResult), new { examId = exam.Id });
        }

        private string GenerateCertificateCode()
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                return BitConverter.ToString(hash).Replace("-", "").Substring(0, 16);
            }
        }

        public async Task<IActionResult> DownloadCertificate(int examId)
        {
            var certificate = await _context.Certificates
                .FirstOrDefaultAsync(c => c.ExamId == examId);

            if (certificate == null)
                return NotFound();

            var pdfBytes = await _certificateService.GenerateCertificateAsync(certificate.Id);

            return File(pdfBytes, "application/pdf", $"certificate_{certificate.CertificateCode}.pdf");
        }

        public async Task<IActionResult> ExamResult(int examId)
        {
            var exam = await _context.Exams
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
            {
                return RedirectToAction(nameof(Exam));
            }

            return View(exam);
        }
    }

    public class ExerciseViewModel
    {
        public Exercise Exercise { get; set; }
        public bool IsLastExercise { get; set; }
    }

    public class ExerciseResultViewModel
    {
        public Exercise Exercise { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public int? NextExerciseId { get; set; }
        public bool IsLastExercise { get; set; }
    }
} 