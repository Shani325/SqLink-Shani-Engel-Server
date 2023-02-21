using Services;

namespace WebAPI.Models
{
    public class ModelsLists
    {
        public List<UserCorrect> Corrects { get; set; }
        public List<UserDTO> InCorrects { get; set; }

        public ModelsLists()
        {
            Corrects = new List<UserCorrect>();
            InCorrects = new List<UserDTO>();   
        }
    }
}
