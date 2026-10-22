using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public string DbPath { get; }

    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var blogModel = modelBuilder.Entity<Blog>();
        var postModel = modelBuilder.Entity<Post>();

        #region Blog
        blogModel
            .HasKey(e => e.BlogId);
        
        blogModel
            .Property(e => e.Url)
            .IsRequired();
        
        blogModel
            .HasMany(e => e.Posts)
            .WithOne(e => e.Blog)
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey(e => e.BlogId);
        #endregion
        
        #region Post
        postModel
            .HasKey(e => e.PostId);
        
        postModel
            .Property(e => e.Title)
            .IsRequired();

        postModel
            .Property(e => e.Content)
            .IsRequired();
        #endregion
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    public List<Post> Posts { get; } = new();
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}