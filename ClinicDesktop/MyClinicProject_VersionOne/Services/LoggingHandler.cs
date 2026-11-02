using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyClinicProject_VersionOne.Services
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. فحص وعرض البيانات المرسلة
            if (request.Content != null)
            {
                string requestBody = await request.Content.ReadAsStringAsync(cancellationToken);

                try
                {
                    using var doc = JsonDocument.Parse(requestBody);
                    requestBody = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                }
                catch { }

                Debug.WriteLine($"\n🚀 ---> إرسال طلب: {request.Method} {request.RequestUri}");
                Debug.WriteLine($"📦 محتوى الطلب:\n{requestBody}");

                // تم تحديد System.Windows.Application بالاسم الكامل لحل التعارض
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        $"الرابط المستهدف:\n{request.Method} {request.RequestUri}\n\n" +
                        $"البيانات المرسلة (JSON Payload):\n" +
                        $"--------------------------------------------------\n" +
                        $"{requestBody}",
                        "🚀 فحص البيانات المرسلة إلى السيرفر",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }

            // 2. إرسال الطلب للسيرفر
            var response = await base.SendAsync(request, cancellationToken);

            // 3. فحص وعرض رد السيرفر
            if (response.Content != null)
            {
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    responseBody = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                }
                catch { }

                Debug.WriteLine($"\n📥 <--- استلام الرد: {(int)response.StatusCode} {response.ReasonPhrase}");
                Debug.WriteLine($"📄 محتوى الرد:\n{responseBody}");

                // تم تحديد System.Windows.Application بالاسم الكامل لحل التعارض
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBoxImage icon = response.IsSuccessStatusCode ? MessageBoxImage.Information : MessageBoxImage.Warning;

                    MessageBox.Show(
                        $"رمز الاستجابة: {(int)response.StatusCode} {response.ReasonPhrase}\n\n" +
                        $"محتوى رد السيرفر (JSON Response):\n" +
                        $"--------------------------------------------------\n" +
                        $"{responseBody}",
                        "📥 رد خادم FastAPI",
                        MessageBoxButton.OK,
                        icon);
                });
            }

            return response;
        }
    }
}