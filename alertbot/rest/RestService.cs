using alertbot.logger;
using System.Net;
using System.Text;

namespace servicecontrolhub.rest
{
    public class RestService : IRestService
    {
        #region const
        string TAG = "RST";
        #endregion

        #region vars        
        ILogger logger;
        #endregion

        #region properties
        public List<IRequestProcessor> RequestProcessors { get; set; } = new();
        #endregion

        public RestService()
        {
            logger = new Logger("rest");
        }

        #region private
        async Task<string> processGetRequest(HttpListenerContext context)
        {
            string res = string.Empty;
            await Task.CompletedTask;
            return res;
        }

        async Task<(HttpStatusCode, string)> processPostRequest(HttpListenerContext context)
        {            
            HttpStatusCode code = HttpStatusCode.NotFound;
            string text = code.ToString();

            await Task.Run(async () =>
            {

                var request = context.Request;
                string path = request.Url.AbsolutePath;

                using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                var requestBody = await reader.ReadToEndAsync();
                var splt = path.Split('/');               

                try
                {
                    switch (splt[1])
                    {
                        default:
                            break;
                    }
                } catch (Exception ex)
                {
                }

            });
            return (code, text);
        }
        async Task processRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            HttpStatusCode code = HttpStatusCode.MethodNotAllowed;
            string responseText = code.ToString();

            switch (request.HttpMethod)
            {
                case "GET":
                    responseText = await processGetRequest(context);
                    break;

                case "POST":
                    (code, responseText) = await processPostRequest(context);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    responseText = response.StatusCode.ToString();
                    break;
            }

            response.StatusCode = (int)code;
                        

            var buffer = Encoding.UTF8.GetBytes(responseText);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);

            var m = $"TX:\n{code}";
            logger.dbg(TAG, m);

        }
        #endregion

        #region public
        public async void Listen()
        {
            var listener = new HttpListener();
#if DEBUG            
            listener.Prefixes.Add($"http://*:5050/diagnostics/");
#elif DEBUG_TG_SERV
            listener.Prefixes.Add($"http://localhost:5050/diagnostics/");            
#else
            listener.Prefixes.Add($"http://*:5000/diagnostics/");            
#endif
            try
            {
                logger.inf(TAG, "Starting rest server...");
                listener.Start();
            }
            catch (Exception ex)
            {
                logger.err(TAG, $"Rest server not started {ex.Message}");
            }

            logger.inf(TAG, "Rest server started");

            while (true)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    await processRequest(context);
                } catch (Exception ex)
                {
                    logger.err(TAG, ex.Message);
                }
            }
        }
#endregion
    }
}
