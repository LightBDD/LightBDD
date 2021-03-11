using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Reporting.Progressive
{
    public class ProgressiveReportWriter : IReportWriter
    {
        private readonly StreamWriter _writer;
        internal JsonlProgressNotifier Notifier { get; }

        public ProgressiveReportWriter()
        {
            _writer = new StreamWriter(File.Create("output.html"), Encoding.UTF8);
            WriteHeader();
            Notifier = new JsonlProgressNotifier(OnLineWrite);
        }

        private async Task OnLineWrite(string line)
        {
            await _writer.WriteLineAsync($"\"{HttpUtility.JavaScriptStringEncode(line)}\",");
        }

        private void WriteHeader()
        {
            _writer.WriteLine(@"<!DOCTYPE html>
<html style=""height: 100%;"">

<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"" />
    <title>LightBDD.Reporting.Progressive.UI</title>
</head>

<body style=""margin:0px;padding:0px;overflow:hidden;height:100%"">
    <script>
        var data=[/*DATA-START:jslines*/");
        }

        public void Save(params IFeatureResult[] results)
        {
            Notifier.Dispose();
            WriteFooter();
            _writer.Dispose();
        }

        private void WriteFooter()
        {
            _writer.WriteLine(@"
        /*DATA-END*/];
    </script>
    <iframe src=""https://localhost:5001/"" id=""child"" frameborder=""0"" style=""overflow:hidden;height:100%;width:100%"" height=""100%"" width=""100%""></iframe>
    <script>
        function loadData(){
        var child = document.getElementById(""child"");
        var childWindow = child.contentWindow;
        childWindow.postMessage({m:""import"",t:""jsonl"",v:1,d:data},'*');
    }
    function onMessage(e) {
        if (e.data.m === ""initialized""){
            loadData();
        }
    }

    if (window.addEventListener) {
        window.addEventListener(""message"", onMessage, false);
    } else {
        window.attachEvent(""onmessage"", onMessage);
    }
      </script>
</body>
</html>");
        }
    }
}