using Dapper;
using Microsoft.AspNetCore.Connections;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingInfrastructureData;
using System.Data;

namespace MovieTicketBooking.Infrastructure.Repositories;

public class PasswordResetRepository : IPasswordResetRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public PasswordResetRepository(
        SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateOtpAsync(PasswordResetOtp otp)
    {
        const string sql = @"
        INSERT INTO PasswordResetOtps
        (
            UserId,
            OtpHash,
            ExpiryTime,
            IsUsed,
            CreatedAt
        )
        VALUES
        (
            @UserId,
            @OtpHash,
            @ExpiryTime,
            @IsUsed,
            @CreatedAt
        );";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, otp);
    }

    public async Task<PasswordResetOtp?> GetLatestOtpAsync(int userId)
    {
        const string sql = @"
        SELECT TOP (1)
            Id,
            UserId,
            OtpHash,
            ExpiryTime,
            IsUsed,
            CreatedAt
        FROM PasswordResetOtps
        WHERE UserId = @UserId
          AND IsUsed = 0
        ORDER BY CreatedAt DESC;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<PasswordResetOtp>(
            sql,
            new
            {
                UserId = userId
            });
    }

    public async Task MarkOtpAsUsedAsync(int otpId)
    {
        const string sql = @"
        UPDATE PasswordResetOtps
        SET IsUsed = 1
        WHERE Id = @OtpId;";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            OtpId = otpId
        });
    }

    public async Task InvalidatePreviousOtpsAsync(int userId)
    {
        const string sql = @"
        UPDATE PasswordResetOtps
        SET IsUsed = 1
        WHERE UserId = @UserId
          AND IsUsed = 0;";

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            UserId = userId
        });
    }


}