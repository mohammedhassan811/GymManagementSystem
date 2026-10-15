using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerServices : ITrainerServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if ( await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email, ct)) 
            return Result.Fail("Email already Exist");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone, ct)) 
            return Result.Fail("Phone already Exist");
            var trainer = new Trainer()
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Specialty = model.Specialties,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    City = model.City,
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street
                }
            };

            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Trainer");

        }

        public async Task<Result> DeleteTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if(trainer is null) return Result.NotFound("Trainer Not Found");

            var hasFutureSessions =await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now);
            if (hasFutureSessions)
                return Result.Validation("Cannot Delete Trainer has Future Sessions");
             _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To  Trainer");
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers =await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return trainers.Select(t => new TrainerViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Phone = t.Phone,
                Email = t.Email,
                Specialties = t.Specialty.ToString()
            });

        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer == null)
                return null;
            else return new TrainerViewModel
            {
                Name = trainer.Name,
                Phone = trainer.Phone,
                Email = trainer.Email,
                Specialties = trainer.Specialty.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} -{trainer.Address.City}"

            }; 
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer == null)
                return null;
            else return new TrainerToUpdateViewModel()
            {
                Name = trainer.Name,
                Phone = trainer.Phone,
                Email = trainer.Email,
                BuildingNumber = trainer.Address.BuildingNumber,
                City=trainer.Address.City,
                Street=trainer.Address.Street,
                Specialties=trainer.Specialty
            };
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer Not Found");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email && x.Id != trainerId, ct))
                return Result.Validation("Email Already Exist");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone && x.Id != trainerId, ct))
                return Result.Validation("Phone Already Exist");

            trainer.Email = model.Email;
            trainer.Phone=model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Specialty = model.Specialties;
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Trainer");


            



        }
    }
}
