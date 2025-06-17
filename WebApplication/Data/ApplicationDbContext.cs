using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Material> LearningMaterials { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ExerciseAttempt> ExerciseAttempts { get; set; }
        public DbSet<MaterialType> Types { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("Topics");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("Learning_materials");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TopicId).HasColumnName("topic_id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Content).HasColumnName("content").IsRequired();
                entity.Property(e => e.TypeId).HasColumnName("type").IsRequired();

                entity.HasOne(m => m.Topic)
                    .WithMany(t => t.Materials)
                    .HasForeignKey(m => m.TopicId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Type)
                    .WithMany()
                    .HasForeignKey(m => m.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(m => m.Comments)
                    .WithOne(c => c.LearningMaterial)
                    .HasForeignKey(c => c.LearningMaterialId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.ToTable("Exercises");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TopicId).HasColumnName("topic_id");
                entity.Property(e => e.Question).HasColumnName("question").IsRequired().HasMaxLength(255);
                entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer").IsRequired().HasMaxLength(255);
                entity.Property(e => e.DescriptionCorrectAnswer).HasColumnName("description_correct_answer").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Options).HasColumnName("options");

                entity.HasOne(e => e.Topic)
                    .WithMany(t => t.Exercises)
                    .HasForeignKey(e => e.TopicId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExerciseAttempt>(entity =>
            {
                entity.ToTable("Exercise_attempts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ExerciseId).HasColumnName("exercise_id");
                entity.Property(e => e.IsCorrect).HasColumnName("is_correct");

                entity.HasOne(ea => ea.User)
                    .WithMany()
                    .HasForeignKey(ea => ea.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ea => ea.Exercise)
                    .WithMany()
                    .HasForeignKey(ea => ea.ExerciseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("Exams");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.TakenAt).HasColumnName("taken_at");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.ToTable("Certificates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CertificateCode).IsRequired().HasMaxLength(255);
                entity.Property(e => e.IssueDate).HasColumnType("date");

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Exam)
                    .WithMany()
                    .HasForeignKey(c => c.ExamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
                entity.Property(e => e.LearningMaterialId).HasColumnName("learning_material_id").IsRequired();
                entity.Property(e => e.Text).HasColumnName("text").IsRequired().HasMaxLength(255);

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Progress>(entity =>
            {
                entity.ToTable("Progresses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CourseCompletion).HasColumnType("numeric(10,0)");

                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MaterialType>(entity =>
            {
                entity.ToTable("Types");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.NameOfType).HasColumnName("name_of_type");
            });

            modelBuilder.Entity<ExamQuestion>(entity =>
            {
                entity.ToTable("Exam_questions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Question).HasColumnName("question");
                entity.Property(e => e.Options).HasColumnName("options");
                entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
                entity.Property(e => e.Explanation).HasColumnName("explanation");
            });
        }
    }
} 