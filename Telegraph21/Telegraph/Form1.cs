using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Telegraph
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            List<string> titles = new List<string> {
                "user",
                "userprofile",
                "profile",
                "member",
                "search",
                "id",
                "go",
                "me",
                "girl"
            };
            List<string> proxys;
            if (File.Exists("proxy.txt"))
            {
                proxys = File.ReadAllLines("proxy.txt").ToList<string>();
            }
            else
            {
                proxys = new List<string>();
            }
            //List<string> names = File.ReadAllLines("name.txt").ToList<string>();
            //Queue<string> captions = new Queue<string>(File.ReadAllLines("caption.txt"));
            List<string> links = File.ReadAllLines("link.txt").ToList<string>();
            Random random = new Random();
            string photo_path;// = Directory.GetFiles("Photo")[random.Next(0, Directory.GetFiles("Photo").Length)];
            string title, link;
            //this.btnStart.Enabled = false;
            (new Task(() =>
            {
                Invoke((Action)(() =>
                {
                    btnStart.Enabled = false;
                }));
                string result_url;
                string proxy = "";
                for (int i = 0; i < numericUpDown1.Value; i++)
                {

                    Ladd((i + 1).ToString() + " - Start", this);
                    //    if (captions.Count == 0)
                    //{
                    //    Ladd("Run out of lines in captions.txt",this);
                    //    break;
                    //}

                    if (proxys.Count != 0)
                    {
                        proxy = proxys[random.Next(0, proxys.Count)];
                    }
                    photo_path = Directory.GetFiles("Photo")[random.Next(0, Directory.GetFiles("Photo").Length)];
                    // caption = captions.Dequeue();
                    title = titles[random.Next(0, titles.Count)] + random.Next(999999, 99999999).ToString();
                    // title = "Подтверждение возраста";
                    //   name = names[random.Next(0, names.Count)];
                    link = links[random.Next(0, links.Count)];
                    Ladd("Title:" + title + "; Link:" + link + "; Proxy:" + proxy + "; Photo:" + photo_path, this);
                    bool flag = OpenTelegraph(proxy);
                    Ladd(flag.ToString(), this);
                    if (!flag)
                    {
                        Ladd("BadProxy", this);
                        continue;
                    }
                    FileStream fs = new FileStream(photo_path, FileMode.Open, FileAccess.Read);
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    fs.Close();

                    // Generate post objects
                    Dictionary<string, object> postParameters = new Dictionary<string, object>();
                    //postParameters.Add("name", "file");
                    //postParameters.Add("filename", "blob");
                    postParameters.Add("file", new FormUpload.FileParameter(data, "blob", "image/jpg"));

                    // Create request and receive response
                    string postURL = "https://telegra.ph/upload";
                    string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                    string fullResponse;
                    string fullHeader;
                    HttpWebResponse webResponse;
                    StreamReader responseReader;
                    try
                    {
                        webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters, proxy);

                        // Process response
                        responseReader = new StreamReader(webResponse.GetResponseStream());
                        fullResponse = responseReader.ReadToEnd();
                        webResponse.Close();
                        Ladd(fullResponse, this);
                    }
                    catch
                    {
                        Ladd("BadProxy", this);
                        continue;
                    }
                    //dynamic json_photo_response_url = JObject.Parse(fullResponse);
                    string photo_response_url = fullResponse.Split('"')[3];
                    Ladd(photo_response_url, this);


                    ///

                    //
                    // Generate post objects
                    //FileStream fs = new FileStream(photo_path, FileMode.Open, FileAccess.Read);
                    Array.Clear(data, 0, data.Length);
                    data = Encoding.ASCII.GetBytes("[{\"tag\":\"figure\",\"children\":[{\"tag\":\"div\",\"attrs\":{\"class\":\"figure_wrapper\"},\"children\":[{\"tag\":\"img\",\"attrs\":{\"src\":\"" + photo_response_url + "\"}}]},{\"tag\":\"figcaption\",\"children\":[\"\"]}]},{\"tag\":\"h3\",\"attrs\":{\"id\":\"*-Да-Нет-*\"},\"children\":[\"      *  \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Да\"]},\"                    \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Нет\"]},\"  *\"]}]");
                    string data2 = "[{\"tag\":\"figure\",\"children\":[{\"tag\":\"div\",\"attrs\":{\"class\":\"figure_wrapper\"},\"children\":[{\"tag\":\"img\",\"attrs\":{\"src\":\"" + photo_response_url + "\"}}]},{\"tag\":\"figcaption\",\"children\":[\"\"]}]},{\"tag\":\"h3\",\"attrs\":{\"id\":\"*-Да-Нет-*\"},\"children\":[\"      *  \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Да\"]},\"                    \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Нет\"]},\"  *\"]}]";
                    // fs.Read(data, 0, data.Length);
                    // fs.Close();
                    //
                    Dictionary<string, object> postParameters2 = new Dictionary<string, object>();
                    //postParameters2.Add("Data", new FormUpload.FileParameter(data, "content.html", "plain/text"));
                    postParameters2.Add("Data\"; filename = \"content.html" + Environment.NewLine + "Content-Type: plain/text", data2);
                    postParameters2.Add("title", title);
                    postParameters2.Add("author", "");
                    postParameters2.Add("author_url", "");
                    postParameters2.Add("save_hash", "");
                    postParameters2.Add("page_id", "0");

                    //postParameters2.Add("file", new FormUpload.FileParameter(data, "blob", "image/jpg"));

                    // Create request and receive response
                    postURL = "https://edit.telegra.ph/save";
                    userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                    try
                    {
                        webResponse = FormUpload.MultipartFormDataPost2(postURL, userAgent, postParameters2, proxy);

                        // Process response
                        responseReader = new StreamReader(webResponse.GetResponseStream());
                        fullResponse = responseReader.ReadToEnd();
                        fullHeader = webResponse.Cookies[0].Value;//.Headers.Keys
                        webResponse.Close();
                    }
                    catch
                    {
                        Ladd("BadProxy", this);
                        continue;
                    }
                    Ladd(fullResponse, this);
                    result_url = fullResponse.Split('"')[7]; //{"page_id":"4e790cc42ea6c660fa72d","path":"title1-07-02-2"}
                    string page_id = fullResponse.Split('"')[3];
                    try
                    {
                        string value = "page_id=" + page_id;
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://edit.telegra.ph/check");
                        //ServicePointManager.ServerCertificateValidationCallback = ((object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
                        ServicePointManager.Expect100Continue = false;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        httpWebRequest.ServicePoint.Expect100Continue = false;
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        //httpWebRequest.Headers.Add("X-tumblr-puppies", tacc.SecureFormKey);
                        httpWebRequest.Headers.Add("Origin", "https://telegra.ph");
                        httpWebRequest.Headers.Add("Sec-Fetch-Site", "same-site");
                        httpWebRequest.Headers.Add("Sec-Fetch-Mode", "cors");
                        httpWebRequest.Headers.Add("Sec-Fetch-Dest", "empty");

                        httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                        httpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Timeout = 10000;
                        httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                        //httpWebRequest.KeepAlive = true;
                        httpWebRequest.Referer = "https://telegra.ph/";
                        httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                        httpWebRequest.Headers.Add("Cookie", "tph_uuid="+fullHeader);
                        /*
                         sec-ch-ua: "Chromium";v="88", "Google Chrome";v="88", ";Not A Brand";v="99"
                        sec-ch-ua-mobile: ?0
                        Accept-Language: ru
                        Cookie: tph_uuid=TtUtaNlIA6lYESUu2Od4R7LBfzlPobtiElJO64vHaQ

                         */

                        if (!string.IsNullOrEmpty(proxy))
                        {
                            if (proxy.Contains(";"))
                            {
                                httpWebRequest.Proxy = new WebProxy(proxy.Split(';')[0])
                                {
                                    Credentials = new NetworkCredential(
                                    proxy.Split(';')[1].Split(':')[0],
                                    proxy.Split(';')[1].Split(':')[1]
                                    )
                                };
                            }
                            else
                            {
                                httpWebRequest.Proxy = new WebProxy(proxy);
                            }
                        }
                        using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(value);
                        }
                        WebResponse response = httpWebRequest.GetResponse();
                        //HttpWebResponse httpWebResponse = (HttpWebResponse)response;
                        Stream responseStream = response.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream);
                        fullResponse = streamReader.ReadToEnd();
                        responseStream.Close();
                        streamReader.Close();

                        response.Close();
                    }
                    catch
                    {
                        Ladd("BadProxy", this);
                        continue;
                    }

                    string hash = fullResponse.Split('"')[15];//{"short_name":"","author_name":"","author_url":"","save_hash":"1392cb17115877b3be8b8e7c6d3a2ce7eab2","can_edit":true}


                    //edit
                    ///
                    title = "Подтверждение возраста";

                    //
                    // Generate post objects
                    //FileStream fs = new FileStream(photo_path, FileMode.Open, FileAccess.Read);
                    //Array.Clear(data, 0, data.Length);
                    //data = Encoding.ASCII.GetBytes("[{\"tag\":\"figure\",\"children\":[{\"tag\":\"div\",\"attrs\":{\"class\":\"figure_wrapper\"},\"children\":[{\"tag\":\"img\",\"attrs\":{\"src\":\"" + photo_response_url + "\"}}]},{\"tag\":\"figcaption\",\"children\":[\"\"]}]},{\"tag\":\"h3\",\"attrs\":{\"id\":\"*-Да-Нет-*\"},\"children\":[\"      *  \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Да\"]},\"                    \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Нет\"]},\"  *\"]}]");
                    //string data2 = "[{\"tag\":\"figure\",\"children\":[{\"tag\":\"div\",\"attrs\":{\"class\":\"figure_wrapper\"},\"children\":[{\"tag\":\"img\",\"attrs\":{\"src\":\"" + photo_response_url + "\"}}]},{\"tag\":\"figcaption\",\"children\":[\"\"]}]},{\"tag\":\"h3\",\"attrs\":{\"id\":\"*-Да-Нет-*\"},\"children\":[\"      *  \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Да\"]},\"                    \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Нет\"]},\"  *\"]}]";
                    // fs.Read(data, 0, data.Length);
                    // fs.Close();
                    //
                    postParameters2 = new Dictionary<string, object>();
                    //postParameters2.Add("Data", new FormUpload.FileParameter(data, "content.html", "plain/text"));
                    data2 = "[{\"tag\":\"figure\",\"children\":[{\"tag\":\"div\",\"attrs\":{\"class\":\"figure_wrapper\"},\"children\":[{\"tag\":\"img\",\"attrs\":{\"src\":\"https://telegra.ph" + photo_response_url + "\"}}]},{\"tag\":\"figcaption\",\"children\":[\"\"]}]},{\"tag\":\"h3\",\"attrs\":{\"id\":\"*-Да-Нет-*\"},\"children\":[\"      *  \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Да\"]},\"                    \",{\"tag\":\"a\",\"attrs\":{\"href\":\"" + link + "\",\"target\":\"_blank\"},\"children\":[\"Нет\"]},\"  *\"]}]";
                    postParameters2.Add("Data\"; filename = \"content.html" + Environment.NewLine + "Content-Type: plain/text", data2);
                    postParameters2.Add("title", title);
                    postParameters2.Add("author", "");
                    postParameters2.Add("author_url", "");
                    postParameters2.Add("save_hash", hash);
                    postParameters2.Add("page_id", page_id);

                    //postParameters2.Add("file", new FormUpload.FileParameter(data, "blob", "image/jpg"));

                    // Create request and receive response
                    postURL = "https://edit.telegra.ph/save";
                    userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                    try
                    {
                        webResponse = FormUpload.MultipartFormDataPost3(postURL, userAgent, postParameters2, proxy, fullHeader);

                        // Process response
                        responseReader = new StreamReader(webResponse.GetResponseStream());
                        fullResponse = responseReader.ReadToEnd();
                        webResponse.Close();
                    }
                    catch
                    {
                        Ladd("BadProxy", this);
                        continue;
                    }
                    Ladd(fullResponse, this);
                    //edit
                    //result_url = fullResponse.Split('"')[7]; //{"page_id":"4e790cc42ea6c660fa72d","path":"title1-07-02-2"}
                    Ladd(result_url, this);
                    List<string> domains = new List<string>();
                    domains.Add("https://graph.org/");
                    domains.Add("https://te.legra.ph/");
                    domains.Add("https://telegra.ph/");

                    File.AppendAllText("resultURL.txt", domains[random.Next(0, domains.Count)] + result_url + Environment.NewLine);
                    Thread.Sleep(random.Next((Int32)numericUpDown2.Value * 1000, (Int32)numericUpDown3.Value * 1000));
                    ////
                }
                Invoke((Action)(() =>
                {
                    btnStart.Enabled = true;
                }));
            })).Start();

        }

        public bool CreatePost(string photo_response_url, string title, string name, string caption, string link, bool chbox, string proxy)
        {

            return true;
        }
        public async Task<string> MultiPostAsync(string file_path)
        {
            string result = "";
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            var file_bytes = new ByteArrayContent(File.ReadAllBytes(file_path));
            // form.Add(new StringContent(username), "username");
            // form.Add(new StringContent(useremail), "email");
            // form.Add(new StringContent(password), "password");
            form.Add(file_bytes, "name=\"file\"", "filename=\"blob\""); //Content-Disposition: form-data; name="file"; filename="blob"
            /*
             POST https://telegra.ph/upload HTTP/1.1
                Host: telegra.ph
                Connection: keep-alive
                Content-Length: 486972
//                Accept: application/json, text/javascript, *\/*; q=0.01
                //X-Requested-With: XMLHttpRequest
                //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36
                //Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryBtDF10BD3okrU9xa
                //Origin: https://telegra.ph
                //Sec-Fetch-Site: same-origin
                //Sec-Fetch-Mode: cors
                //Sec-Fetch-Dest: empty
                //Referer: https://telegra.ph/
                //Accept-Encoding: gzip, deflate, br
                //Accept-Language: ru,en-US;q=0.9,en;q=0.8

             */
            //  httpClient.he
            HttpResponseMessage response = await httpClient.PostAsync("https://telegra.ph/upload", form);

            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
            string sd = response.Content.ReadAsStringAsync().Result;
            result = sd;
            return result;
        }
        public bool OpenTelegraph(string proxy)
        {
            bool result = true;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://telegra.ph/");
                ServicePointManager.ServerCertificateValidationCallback = ((object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                httpWebRequest.Headers.Add("Accept-Language", "ru,en-US;q=0.9,en;q=0.8");
                httpWebRequest.ContentType = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWebRequest.Headers.Add("Upgrade-Insecure-Request", "1");
                httpWebRequest.Headers.Add("Sec-Fetch-Site", "none");
                httpWebRequest.Headers.Add("Sec-Fetch-Mode", "navigate");
                httpWebRequest.Headers.Add("Sec-Fetch-User", "?1");
                httpWebRequest.Headers.Add("Sec-Fetch-Dest", "document");
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 10000;
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                httpWebRequest.KeepAlive = true;
                if (!string.IsNullOrEmpty(proxy))
                {
                    if (proxy.Contains(";"))
                    {
                        httpWebRequest.Proxy = new WebProxy(proxy.Split(new char[]
                        {
                            ';'
                        })[0])
                        {
                            Credentials = new NetworkCredential(proxy.Split(new char[]
                        {
                            ';'
                        })[1].Split(new char[]
                        {
                            ':'
                        })[0], proxy.Split(new char[]
                        {
                            ';'
                        })[1].Split(new char[]
                        {
                            ':'
                        })[1])
                        };
                    }
                    else
                    {
                        httpWebRequest.Proxy = new WebProxy(proxy);
                    }
                }
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string text = streamReader.ReadToEnd();
                responseStream.Close();
                streamReader.Close();
                response.Close();

            }
            catch
            {
                result = false;
            }
            return result;
        }
        public void TextToRichBox(string text)
        {
            Invoke((Action)(() =>
            {
                richTextBox1.AppendText(text + Environment.NewLine);
            }));
        }
        private void Ladd(string msg, Form1 form)
        {
            form.TextToRichBox(msg);
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.ScrollToCaret();
        }
    }
}
