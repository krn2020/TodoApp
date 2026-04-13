using Microsoft.EntityFrameworkCore;
using TodoApp.DAL.Entities;
namespace TodoApp.DAL.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<SubTask> SubTasks => Set<SubTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasOne(t => t.User)
                  .WithMany(u => u.TodoItems)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.TodoList)
                  .WithMany(l => l.TodoItems)
                  .HasForeignKey(t => t.TodoListId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TodoList>(entity =>
        {
            entity.HasOne(l => l.User)
                  .WithMany() 
                  .HasForeignKey(l => l.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubTask>(entity =>
        {
            entity.HasOne(s => s.TodoItem)
                  .WithMany(t => t.SubTasks)
                  .HasForeignKey(s => s.TodoItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}