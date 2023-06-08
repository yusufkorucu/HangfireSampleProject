using HangfireSample.Services;

namespace HangfireSample.Job
{
    public class CargoJob
    {
        private readonly ICargoService _cargoService;

        public CargoJob(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        public bool SendToCargo()
        {
            return _cargoService.SendToCargo();
        }

        public bool UpdateCargoStatus()
        {
            return _cargoService.UpdateCargoStatus();
        }
    }
}
