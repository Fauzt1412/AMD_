using Microsoft.EntityFrameworkCore;
using ShortenURLService.Data;
using ShortenURLService.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShortenURLService.Services
{
    public class GenerateShortenURL
    {   //frontend looking for userUrl
        private readonly ShortenURLServiceContext _context;
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public GenerateShortenURL(ShortenURLServiceContext context)
        {
            _context = context;
        }

        private async Task<string> GenerateShortCodeAsync()
        {
            Random random = new Random(); // Fixed syntax error (was using - instead of =)
            string shortCode;
            do
            {
                shortCode = new string(Enumerable.Repeat(Characters, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

            } while (await _context.URL.AnyAsync(u => u.ShortenedUrl == shortCode));
            return shortCode;
        }

        public async Task<URL> ShortenUrlAsync(string originalUrl, string? customCode = null)
        {
            // Check if this URL already exists in our database
            var existingUrl = await _context.URL.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);
            if (existingUrl != null)
            {
                // Return existing URL entity
                return existingUrl;
            }

            string shortCode;
            bool isCustom = false;

            if (!string.IsNullOrWhiteSpace(customCode))
            {
                // Validate custom code
                if (!IsValidCustomCode(customCode))
                {
                    throw new ArgumentException("Custom code must be 3-20 characters long and contain only letters and numbers.");
                }

                // Check if custom code is already taken
                if (await _context.URL.AnyAsync(u => u.ShortenedUrl == customCode))
                {
                    throw new ArgumentException("This custom code is already taken.");
                }

                shortCode = customCode;
                isCustom = true;
            }
            else
            {
                // Generate a new random short code
                shortCode = await GenerateShortCodeAsync();
            }

            // Create new URL entity
            var urlEntity = new URL
            {
                OriginalUrl = originalUrl,
                ShortenedUrl = shortCode,
                CreatedAt = DateTime.UtcNow,
                IsCustom = isCustom
            };

            // Save to database
            _context.URL.Add(urlEntity);
            await _context.SaveChangesAsync();
            return urlEntity;
        }

        private bool IsValidCustomCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            if (code.Length < 3 || code.Length > 20)
                return false;

            // Only allow letters and numbers
            return code.All(c => char.IsLetterOrDigit(c));
        }
    }
}