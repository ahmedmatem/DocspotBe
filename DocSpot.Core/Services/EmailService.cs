using DocSpot.Core.Contracts;
using DocSpot.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Globalization;
using static DocSpot.Core.Constants;

namespace DocSpot.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> options) 
            => emailSettings = options.Value;

        public async Task SendAppointmentConfirmationAsync(AppointmentDto appointmentDto, CancellationToken ct)
        {
            var bg = CultureInfo.GetCultureInfo("bg-BG");

            var confirmUrl = $"{emailSettings.BaseUrl}/appointment/confirm?token={appointmentDto.PublicToken}&id={appointmentDto.Id}";
            var cancelUrl = $"{emailSettings.BaseUrl}/appointment/public?token={appointmentDto.CancelToken}&id={appointmentDto.Id}";
            //var rescheduleUrl = $"{FrontendBaseUrl}appointment/reschedule?token={appointmentDto.PublicToken}&id={appointmentDto.Id}";

            var subject = $"Потвърждение за записан час: {appointmentDto.AppointmentDate.ToString("dd MMMM yyyy")} {appointmentDto.AppointmentTime}";
            appointmentDto.VisitType = appointmentDto.VisitType.ToLower() switch
            {
                "paid" => "Платен преглед",
                "nhi_first" => "Първичен преглед",
                "nhi_followup" => "Вторичен преглед",
                _ => "неопределен преглед"
            };

            var bodyHtml = $@"
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                    <tr>
                      <td align=""center"" style=""padding:24px 12px;"">

                        <table width=""560"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background:#fafaf9;border:1px solid #e7e5e4;border-radius:20px;padding:0"">
                            <tbody><tr>
                              <td style=""padding:40px 32px""> 

                                <h1 style=""margin:0;text-align:center;font-size:28px;color:#666666;font-weight:700"">
                                  Благодарим за доверието!
                                </h1>    

                                <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                                  <tbody><tr>
                                    <td align=""center"" style=""padding-top:16px;padding-bottom:24px"">
                                      <div style=""width:100px;height:4px;background:#198754;border-radius:8px""></div>
                                    </td>
                                  </tr>
                                </tbody></table>
            
                                <p style=""text-align:center;font-size:18px;color:#666666;margin-top:0;margin-bottom:32px;line-height:1.5"">
                                  Вашият час беше успешно регистриран в нашата система.
                                </p>
            
                                <table width=""100%"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""font-size:16px;color:#666666;line-height:1.6"">
                                  <tbody>
              	                    <tr>
	                                    <td>
	                                      <p style=""margin:0 0 12px""><strong>Дата:</strong>
	                                        <span style=""text-transform:capitalize"">
	                                          {appointmentDto.AppointmentDate.ToString("dd MMMM yyyy", bg)}
	                                        </span>
	                                      </p>

	                                      <p style=""margin:0 0 12px""><strong>Час:</strong>
	                                        <span>{appointmentDto.AppointmentTime.ToString("HH:mm")}</span>
	                                      </p>

	                                      <p style=""margin:0 0 12px""><strong>Тип:</strong>
	                                        <span>{appointmentDto.VisitType}</span>
	                                      </p>

	                                      <p style=""margin:0 0 12px""><strong>Имена:</strong>
	                                        <span>{appointmentDto.PatientName}</span>
	                                      </p>

	                                      <p style=""margin:0 0 12px""><strong>Телефон:</strong>
	                                        <span>{appointmentDto.PatientPhone}</span>
	                                      </p>

	                                  </td>
                                  </tr>
                                </tbody></table>
            
                                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""margin-top:32px"">
                                  <tbody><tr>
                                    <td align=""center"">
                                      <a href=""{cancelUrl}"" style=""background:#c83131;color:white;text-decoration:none;padding:10px 22px;font-size:16px;border-radius:8px;font-weight:600;display:inline-block"" target=""_blank"">
                                        Откажете час
                                      </a>
                                    </td>
                                  </tr>
                                </tbody></table>
            
                                <p style=""margin-top:20px;font-size:14px;color:#9b9b9b;text-align:center;line-height:1.5"">
                                  До {CancelTokenExpireHours} часа преди прегледа можете да го отмените от бутона или на следния линк:<br>
                                  <a href=""{cancelUrl}"" style=""color:#337ca4;text-decoration:none"" target=""_blank"">
                                    {cancelUrl}
                                  </a>
                                </p>
            
                                <p style=""text-align:center;font-size:13px;color:#6f7072;margin-top:40px"">
                                  С уважение, <br>
                                  <strong>Д-р Мария Илиева</strong><br>
                                  © 2025 <a href=""https://docspot.com"" style=""color:#6f7072;text-decoration:none"" target=""_blank"">
                                    https://docspot.com
                                  </a>
                                </p>

                              </td>
                            </tr>
                          </tbody></table>

                        </td>
                    </tr>
                </table>";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(appointmentDto.PatientEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = bodyHtml };
            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            // Connect with TLS (STARTTLS on 587 is typical)
            var socketOpt = emailSettings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect;
            await smtp.ConnectAsync(emailSettings.SmtpHost, emailSettings.SmtpPort, socketOpt, ct);

            await smtp.AuthenticateAsync(emailSettings.Username, emailSettings.AppPassword, ct);
            await smtp.SendAsync(message, ct);
            await smtp.DisconnectAsync(true, ct);
        }
    }
}
