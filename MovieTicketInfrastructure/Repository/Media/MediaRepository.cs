using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingInfrastructureData;
using System.Data;

namespace MovieTicketBooking.Infrastructure.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly IWebHostEnvironment _environment;


    public MediaRepository(SqlConnectionFactory connectionFactory, IWebHostEnvironment environment)
    {
        _connectionFactory = connectionFactory;
        _environment = environment;
        ;
    }

    public async Task<Media> SaveImageAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new Exception("No file selected.");

        string uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            folderName);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string extension = Path.GetExtension(file.FileName);

        string fileName = $"{Guid.NewGuid()}{extension}";

        string filePath = Path.Combine(
            uploadsFolder,
            fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return new Media
        {
            FileName = fileName,
            FilePath = $"/uploads/{folderName}/{fileName}"
        };
    }

    public async Task<int> CreateImageAsync(Media media)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql = @"
        INSERT INTO Media
        (
            FilePath
        )
        VALUES
        (
            @FilePath 
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await connection.ExecuteScalarAsync<int>(sql, media);
    }

    public async Task<int> CreateAsync(IDbConnection connection, IDbTransaction transaction, Media media)
    {

        string sql = @"
        INSERT INTO Media
        (
            FilePath
        )
        VALUES
        (
            @FilePath 
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await connection.ExecuteScalarAsync<int>(sql, media, transaction);
    }

    public async Task<Media?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Media>(
            "SELECT * FROM Media WHERE Id=@Id",
            new { Id = id });
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "DELETE FROM Media WHERE Id=@Id",
            new { Id = id });
    }
}