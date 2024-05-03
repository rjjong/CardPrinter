using CardPinter.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CardPinter.Database;

public class LocalRepositoryBase : IDisposable
{
    bool _disposed = false;
    SafeHandle? _handle = new SafeFileHandle(IntPtr.Zero, true);

    public readonly CardContext DatabaseContext;

    public LocalRepositoryBase()
    {
        DatabaseContext = new CardContext();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            DatabaseContext?.Dispose();
            _handle?.Dispose();
            _handle = null;
        }

        _disposed = true;
    }
}

public class CardContext : DbContext
{
    public CardContext()
    {
        if (Database.EnsureCreated())
        {
            SaveChanges();
        }
    }

    public DbSet<CardInfo> Cards { get; set; }
    public DbSet<CardDetails> Details { get; set; }
    public DbSet<CardImageInfo> Images { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"FileName=carddata.sqlite3", options =>
        {
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });

        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CardInfo>()
            .HasMany(c => c.CardImages)
            .WithOne(c => c.CardInfo)
            .HasForeignKey(c => c.CardInfoId);

        modelBuilder.Entity<CardInfo>()
            .HasMany(c => c.CardDetails)
            .WithOne(c => c.CardInfo)
            .HasForeignKey(c => c.CardInfoId);
    }
}
