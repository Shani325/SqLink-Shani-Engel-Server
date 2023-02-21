using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Services;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadExcelController : ControllerBase
    {
        // GET: api/<ReadExcel>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ReadExcel>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ReadExcel>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ReadExcel>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ReadExcel>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpPost("import")]
        public async Task<Response<ModelsLists>> Import(IFormFile formFile, CancellationToken cancellationToken)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                return Response<ModelsLists>.GetResult(-1, "formfile is empty");
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return Response<ModelsLists>.GetResult(-1, "Not Support file extension");
            }
            ModelsLists model = new ModelsLists();
            var list = new List<UserDTO>();

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream, cancellationToken);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        UserDTO user = new UserDTO
                        {
                            fullName = worksheet.Cells[row, 1].Value.ToString().Trim() + ' ' + (worksheet.Cells[row, 2].Value.ToString().Trim()),
                            tz = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            yearOfBirth = DateTime.Now.Year - int.Parse(worksheet.Cells[row, 4].Value.ToString().Trim())                            
                        };
                        if (user.Check())
                        {
                            string url = "https://api-sq.azurewebsites.net/People";
                            HttpClient client = new HttpClient();
                            HttpResponseMessage httpResponse =await client.PostAsJsonAsync(url,user);
                            Console.WriteLine(httpResponse.ToString());
                            model.Corrects.Add(new UserCorrect() { fullName=user.fullName,tz=user.tz,yearOfBirth=user.yearOfBirth,id=1});
                        }
                        else
                        {
                            model.InCorrects.Add(user);
                        }
                    }
                }
            }

            // add list to db ..  
            // here just read and return  

            return Response<ModelsLists>.GetResult(0, "OK", model);
        }
    }
}
