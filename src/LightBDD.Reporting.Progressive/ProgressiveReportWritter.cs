using System.IO;
using System.Text;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Reporting.Progressive
{
    public class ProgressiveReportWritter : IReportWriter
    {
        private FileStream _fileStream;
        public JsonlProgressNotifier Notifier { get; }

        public ProgressiveReportWritter()
        {
            _fileStream = File.OpenWrite("output.html");
            WriteHeader();
            Notifier = new JsonlProgressNotifier(_fileStream);
        }

        private void WriteHeader()
        {
            using var writer = new StreamWriter(_fileStream, Encoding.UTF8, 1024, true);
            writer.WriteLine(@"<!DOCTYPE html>
<html style=""height: 100%;"">

<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"" />
    <title>LightBDD.Reporting.Progressive.UI</title>
</head>

<body style=""margin:0px;padding:0px;overflow:hidden;height:100%"">
    <script>
        var data=[/*DATA-START*/");
        }

        public void Save(params IFeatureResult[] results)
        {
            Notifier.Dispose();
            WriteFooter();
            _fileStream.Dispose();
        }

        private void WriteFooter()
        {
            using var writer = new StreamWriter(_fileStream, Encoding.UTF8, 1024, true);
            writer.WriteLine(@"
        /*DATA-END*/];
    </script>
    <iframe src=""https://localhost:5001/"" id=""child"" frameborder=""0"" style=""overflow:hidden;height:100%;width:100%"" height=""100%"" width=""100%""></iframe>
    <script>
        function loadData(){
        var child = document.getElementById(""child"");
        var childWindow = child.contentWindow;
        childWindow.postMessage({m:""import"",t:""jsonl"",v:1,d:JSON.stringify(data)},'*');
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