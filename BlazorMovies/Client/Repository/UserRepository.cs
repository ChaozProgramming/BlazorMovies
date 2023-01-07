using BlazorMovies.Client.Helpers;
using BlazorMovies.Shared.DTO;

namespace BlazorMovies.Client.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IHttpService httpService;
        private readonly string baseURL = "api/user";

        public UserRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<PaginatedResponse<List<UserDTO>>> GetUsers(PaginationDTO paginationDTO)
        {
            return await httpService.GetExtension<List<UserDTO>>(baseURL, paginationDTO);
        }

        public async Task<List<RoleDTO>> GetRoles()
        {
            return await httpService.GetExtension<List<RoleDTO>>($"{baseURL}/roles");
        }

        public async Task AssignRole(EditRoleDTO editRole)
        {
            var response = await httpService.Post($"{baseURL}/assignRole", editRole);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

        }

        public async Task RemoveRole(EditRoleDTO editRole)
        {
            var response = await httpService.Post($"{baseURL}/removeRole", editRole);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

        }
    }
}
