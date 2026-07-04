using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class LocationManager : ILocationManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<GeneralResult<List<CountryWithCitiesDto>>> GetCountriesWithCities()
        //{
        //    //var countries = await _unitOfWork.CountryRepository.GetAllWithCitiesAsync();

        //    //var result = countries.Select(c => new CountryWithCitiesDto
        //    //{
        //    //    Id = c.Id,
        //    //    Name = c.Name,
        //    //    Code = c.Code,
        //    //    Cities = c.Cities
        //    //        .OrderBy(city => city.Name)
        //    //        .Select(city => new CityDto { Id = city.Id, Name = city.Name })
        //    //        .ToList()
        //    //}).ToList();

        //    //return GeneralResult<List<CountryWithCitiesDto>>.SuccessResult(result);
        //}
    }
}