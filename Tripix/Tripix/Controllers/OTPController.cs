using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tripix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPController : ControllerBase
    {
        private readonly HttpClient client;


        public OTPController ( HttpClient client )
        {
            this.client = client;
        }
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp ( [FromBody] OtpRequest request )
        {
            if (!IsValidEmail(request.Email))
                return BadRequest(new { message = "Invalid email address" });

            var otp = GenerateOtp(); // دالة لتوليد الـ OTP
            await SendEmailAsync(request.Email, otp); // دالة لإرسال الإيميل

            return Ok(new { otp });
        }

        private bool IsValidEmail ( string email )
        {
            return new EmailAddressAttribute().IsValid(email);
        }

        private string GenerateOtp ( int length = 4 )
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // حروف وأرقام
            Random random = new();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }




        private async Task SendEmailAsync ( string email, string otp )
        {
            var apiKey = Environment.GetEnvironmentVariable("Brevo_API_KEY");
            Console.WriteLine(apiKey);// استبدلها بالمفتاح اللي في الصورة
            var requestUrl = "https://api.brevo.com/v3/smtp/email";

            var emailData = new
            {
                sender = new { name = "Tripix Support", email = "tripixv911@gmail.com" },
                to = new[] { new { email = email } },
                subject = "Your OTP Code",
                htmlContent = $@"
                          <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; text-align: center;'>
                              <div style='background: white; padding: 20px; border-radius: 10px; box-shadow: 0px 0px 10px rgba(0,0,0,0.1); max-width: 400px; margin: auto;'>
                                  <img src='https://i.postimg.cc/x8PrvCN8/1741451120457.png' alt='Trabbix Logo' style='width: 120px; margin-bottom: 20px;'>
                                  <h2 style='color: #333;'>Your OTP Code</h2>
                                  <p style='font-size: 18px; color: #666;'>Use the following OTP to verify your identity:</p>
                                  <p style='font-size: 24px; font-weight: bold; color: #ff6b6b; margin: 10px 0;'>{otp}</p>
                                  <p style='color: #666; font-size: 14px;'>This OTP is valid for 10 minutes. Do not share it with anyone.</p>
                                  <hr style='margin: 20px 0; border: none; border-top: 1px solid #ddd;'>
                                  <p style='font-size: 12px; color: #999;'>© 2025 Tripix. All rights reserved.</p>
                              </div>
                          </div>
                      "
            };

            var json = JsonConvert.SerializeObject(emailData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("api-key", apiKey);

            var response = await client.PostAsync(requestUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);

        }
    }
    public class OtpRequest
    {
        public string Email { get; set; }
    }
}
