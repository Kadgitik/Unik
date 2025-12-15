using Microsoft.EntityFrameworkCore;
using TravelTracker.Models;

namespace TravelTracker.Data
{
    public class TripRepository
    {
        // CREATE - Додати подорож
        public void AddTrip(Trip trip)
        {
            using var context = new TravelDbContext();
            trip.CreatedAt = DateTime.Now;
            context.Trips.Add(trip);
            context.SaveChanges();
        }

        // READ - Отримати всі подорожі з пагінацією
        public List<Trip> GetTrips(int page = 1, int pageSize = 10)
        {
            using var context = new TravelDbContext();
            return context.Trips
                .Include(t => t.Country)
                .Include(t => t.TripType)
                .Include(t => t.Accommodation)
                .OrderByDescending(t => t.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // READ - Отримати одну подорож по ID
        public Trip? GetTripById(int id)
        {
            using var context = new TravelDbContext();
            return context.Trips
                .Include(t => t.Country)
                .Include(t => t.TripType)
                .Include(t => t.Accommodation)
                .FirstOrDefault(t => t.TripId == id);
        }

        // UPDATE - Оновити подорож
        public void UpdateTrip(Trip trip)
        {
            using var context = new TravelDbContext();
            context.Trips.Update(trip);
            context.SaveChanges();
        }

        // DELETE - Видалити подорож
        public void DeleteTrip(int id)
        {
            using var context = new TravelDbContext();
            var trip = context.Trips.Find(id);
            if (trip != null)
            {
                context.Trips.Remove(trip);
                context.SaveChanges();
            }
        }

        // Загальна кількість записів (для пагінації)
        public int GetTotalCount()
        {
            using var context = new TravelDbContext();
            return context.Trips.Count();
        }

        // СКЛАДНА ФІЛЬТРАЦІЯ (LINQ to Entities)
        public List<Trip> SearchTrips(int? countryId, int? tripTypeId, 
            DateTime? startDate, DateTime? endDate, string? searchText)
        {
            using var context = new TravelDbContext();
            
            var query = context.Trips
                .Include(t => t.Country)
                .Include(t => t.TripType)
                .Include(t => t.Accommodation)
                .AsQueryable();

            // Фільтр по країні
            if (countryId.HasValue && countryId.Value > 0)
                query = query.Where(t => t.CountryId == countryId.Value);

            // Фільтр по типу подорожі
            if (tripTypeId.HasValue && tripTypeId.Value > 0)
                query = query.Where(t => t.TripTypeId == tripTypeId.Value);

            // Фільтр по даті початку
            if (startDate.HasValue)
                query = query.Where(t => t.StartDate >= startDate.Value);
            
            // Фільтр по даті кінця
            if (endDate.HasValue)
                query = query.Where(t => t.EndDate <= endDate.Value);

            // Пошук по назві міста
            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(t => t.Destination.Contains(searchText));

            return query.OrderByDescending(t => t.StartDate).ToList();
        }

        // СОРТУВАННЯ (виправлено для роботи з SQLite)
        public List<Trip> GetSortedTrips(string sortBy, bool ascending = true)
        {
            using var context = new TravelDbContext();
            
            // Спочатку завантажуємо всі дані в пам'ять
            var trips = context.Trips
                .Include(t => t.Country)
                .Include(t => t.TripType)
                .Include(t => t.Accommodation)
                .ToList(); // ← ВАЖЛИВО! Завантажуємо в пам'ять перед сортуванням

            // Тепер сортуємо в пам'яті (LINQ to Objects)
            IEnumerable<Trip> sortedTrips = sortBy switch
            {
                "Date" => ascending 
                    ? trips.OrderBy(t => t.StartDate) 
                    : trips.OrderByDescending(t => t.StartDate),
                
                "Destination" => ascending 
                    ? trips.OrderBy(t => t.Destination) 
                    : trips.OrderByDescending(t => t.Destination),
                
                "Cost" => ascending 
                    ? trips.OrderBy(t => t.ActualCost) 
                    : trips.OrderByDescending(t => t.ActualCost),
                
                "Rating" => ascending 
                    ? trips.OrderBy(t => t.Rating) 
                    : trips.OrderByDescending(t => t.Rating),
                
                _ => trips.OrderByDescending(t => t.StartDate)
            };

            return sortedTrips.ToList();
        }

        // СТАТИСТИКА
        public TripStatistics GetStatistics()
        {
            using var context = new TravelDbContext();
            
            var stats = new TripStatistics
            {
                TotalTrips = context.Trips.Count(),
                
                CountriesVisited = context.Trips
                    .Select(t => t.CountryId)
                    .Distinct()
                    .Count(),
                
                TotalSpent = context.Trips.Any() ? context.Trips.Sum(t => t.ActualCost) : 0,
                
                AverageRating = context.Trips.Any() 
                    ? context.Trips.Average(t => (double)t.Rating) 
                    : 0,
                
                MostVisitedCountry = context.Trips
                    .GroupBy(t => t.Country.Name)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "Немає даних"
            };

            return stats;
        }

        // КАСКАДНЕ ВИДАЛЕННЯ типу подорожі
        public void DeleteTripTypeWithTrips(int tripTypeId)
        {
            using var context = new TravelDbContext();
            
            var tripType = context.TripTypes
                .Include(tt => tt.Trips)
                .FirstOrDefault(tt => tt.TripTypeId == tripTypeId);
            
            if (tripType != null)
            {
                context.TripTypes.Remove(tripType);
                context.SaveChanges();
            }
        }

        // Допоміжні методи для завантаження довідників
        public List<Country> GetAllCountries()
        {
            using var context = new TravelDbContext();
            return context.Countries.OrderBy(c => c.Name).ToList();
        }

        public List<TripType> GetAllTripTypes()
        {
            using var context = new TravelDbContext();
            return context.TripTypes.OrderBy(t => t.Name).ToList();
        }

        public List<Accommodation> GetAllAccommodations()
        {
            using var context = new TravelDbContext();
            return context.Accommodations.OrderBy(a => a.Name).ToList();
        }
    }

    // Клас для статистики
    public class TripStatistics
    {
        public int TotalTrips { get; set; }
        public int CountriesVisited { get; set; }
        public decimal TotalSpent { get; set; }
        public double AverageRating { get; set; }
        public string MostVisitedCountry { get; set; } = string.Empty;
    }
}